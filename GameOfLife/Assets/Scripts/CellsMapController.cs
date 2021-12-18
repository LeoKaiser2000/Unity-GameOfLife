using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using B83.Image.BMP;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

enum ColorMode
{
    Default = 0,
    Random = 1,
    Migration = 2,
    Rainbow = 3
}

public class CellsMapController : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private Color HoverColor;
    [SerializeField] private Cell OriginalCell;
    [SerializeField] private Vector3 CellScale;
    [SerializeField] private  Color ColorDead;
    [SerializeField] private Camera GameCamera;
    [SerializeField] private float RenderScale;
    [SerializeField] private Slider RandomRateSlider;
    [SerializeField] public Toggle RandomAlphaToggle;
    [SerializeField] private InputField WidthInputField;
    [SerializeField] private InputField HeightInputField;
    [SerializeField] private Dropdown ColorModeDropdown;
    [SerializeField] private Slider DefaultRedSlider;
    [SerializeField] private Slider DefaultGreenSlider;
    [SerializeField] private Slider DefaultBlueSlider;
    [SerializeField] private Slider DefaultAlphaSlider;
    [SerializeField] private Image DefaultColorPreviewImage;

    [SerializeField] private int MaxWidth;
    [SerializeField] private int MaxHeight;
    [SerializeField] private int MinWidth;
    [SerializeField] private int MinHeight;
    [SerializeField] private int AlphaMinForPixel;
    [SerializeField] private int BlackMinForPixel;


    private ColorMode _colorMode;
    private float _randomRate;
    private bool _randomAlpha;
    private bool _continuousRun;
    private bool _singleRun;
    private Cell[,] _cells;
    private int _width;
    private int _height;
    private bool[,] _defaultCursor;
    private bool[,] _currentCursor;
    private Cell _hoveredCell;
    private Color _colorAlive;

    void CreateDefaultCursor()
    {
        _defaultCursor = new bool[1,1];
        _defaultCursor[0, 0] = true;
        _currentCursor = _defaultCursor;
    }

    void Start()
    {
        if (Enum.IsDefined(typeof(ColorMode), ColorModeDropdown.value))
            _colorMode = (ColorMode)ColorModeDropdown.value;
        _randomAlpha = RandomAlphaToggle.isOn;
        _randomRate = RandomRateSlider.value;
        SetRandomDefaultColor();
        CreateDefaultCursor();
        PlaceCells();
    }

    private void UpdateCellStatus(Cell cell)
    {
        switch (cell.neighbourCounter)
        {
            case 0:
            case 1:
                if (cell.data.IsAlive)
                    CellSetChanges(cell, _colorMode,false);
                break;
            case 2:
                break;
            case 3:
                if (!cell.data.IsAlive)
                    CellSetChanges(cell, _colorMode, true);
                break;
            default:
                if (cell.data.IsAlive)
                    CellSetChanges(cell, _colorMode,false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlayContinuous();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            PlaySingle();
        
        if (_singleRun || _continuousRun)
        {
            for (int Y = 0; Y < _height; ++Y)
            {
                for (int X = 0; X < _width; ++X)
                {
                    UpdateCellStatus(_cells[Y, X]);
                }
            }
            for (int Y = 0; Y < _height; ++Y) 
            {
                for (int X = 0; X < _width; ++X)
                {
                    CellApplyChanges(_cells[Y, X]);
                }
            }
            _singleRun = false;
        }
    }

    private bool Resize(int newWidth, int newHeight)
    {
        if (newHeight > MaxHeight || newHeight < MinHeight || newWidth > MaxWidth || newWidth < MinWidth)
        {
            HeightInputField.text = _height.ToString();
            WidthInputField.text = _width.ToString();
            return false;
        }

        var temporaryTab = new Cell[newHeight, newWidth];
        
        for (int Y = 0; Y < _height; ++Y)
        {
            for (int X = 0; X < _width; ++X)
            {
                if (Y >= newHeight || X >= newWidth)
                {
                    var cell = _cells[Y, X];
                    CellSetChanges(cell, _colorMode, false);
                    CellApplyChanges(cell);
                    Destroy(cell.gameObject);
                }
            }
        }

        for (int Y = 0; Y < newHeight; ++Y)
        {
            for (int X = 0; X < newWidth; ++X)
            {
                temporaryTab[Y, X] = (Y < _height && X < _width)
                    ? _cells[Y, X]
                    : CellInstantiate(X, Y);
            }
        }
        
        _cells = temporaryTab;
        _height = newHeight;
        _width = newWidth;
        HeightInputField.text = _height.ToString();
        WidthInputField.text = _width.ToString();

        GameCamera.transform.position = new Vector3(
            newWidth * CellScale.x / 2,
            newHeight * CellScale.y / 2,
            GameCamera.transform.position.z
        ) + transform.position;
        GameCamera.orthographicSize = Mathf.Max(newWidth * CellScale.x, newHeight * CellScale.y) * RenderScale;
        return true;
    }

    private Cell CellInstantiate(int X, int Y)
    {
        Cell cell = Instantiate(
            OriginalCell,
            new Vector2(
                X * CellScale.x,
                Y * CellScale.y),
            Quaternion.identity,
            transform
        );
        cell.transform.localScale = CellScale;
        cell.data.IsAlive = false;
        cell.isAliveNext = false;
        cell.ColorNext = ColorDead;
        cell.data.X = X;
        cell.data.Y = Y;
        cell.neighbourCounter = 0;
        cell.controller = this;
        cell.data.Color = ColorDead;
        cell.GetComponent<SpriteRenderer>().color = cell.data.Color;
        return cell;
    }
    
    private void PlaceCells()
    {
        _cells = new Cell[0, 0];
        Resize(int.Parse(WidthInputField.text), int.Parse(HeightInputField.text));
        Randomize();
    }

    private List<Color> CellGetNeighboursColor(Cell cell)
    {
        List<Color> neighboursColor = new List<Color>();

        for (int Y = (cell.data.Y == 0) ? cell.data.Y : cell.data.Y - 1,
            YMax = (cell.data.Y == _height - 1) ? cell.data.Y : cell.data.Y + 1;
            Y <= YMax;
            ++Y)
        {
            for (int X = (cell.data.X == 0) ? cell.data.X : cell.data.X - 1,
                XMax = (cell.data.X == _width - 1) ? cell.data.X : cell.data.X + 1;
                X <= XMax;
                ++X)
            {
                if (X == cell.data.X && Y == cell.data.Y) continue;
                Cell neighbourCell = _cells[Y, X];

                if (neighbourCell.data.IsAlive)
                {
                    neighboursColor.Add(neighbourCell.data.Color);
                }
            }
        }
        return neighboursColor;
    }

    public void Randomize()
    {
        _continuousRun = false;
        for (int Y = 0; Y < _height; ++Y)
        {
            for (int X = 0; X < _width; ++X)
            {
                Cell cell = _cells[Y, X];
                CellSetChanges(cell, _colorMode, Random.Range(1, 256) <= _randomRate);
                CellApplyChanges(cell);
            }
        }
    }
    
    public void Fill(bool fill)
    {
        _continuousRun = false;
        for (int Y = 0; Y < _height; ++Y)
        {
            for (int X = 0; X < _width; ++X)
            {
                Cell cell = _cells[Y, X];
                CellSetChanges(cell, _colorMode, fill);
                CellApplyChanges(cell);
            }
        }
    }

    private Color CellComputeColor(Cell cell, ColorMode colorMode)
    {
        switch (colorMode)
        {
            case ColorMode.Default:
                return _colorAlive;
            case ColorMode.Rainbow:
                List<Color> neighboursColorRainbow = CellGetNeighboursColor(cell);
                return neighboursColorRainbow.Count == 0
                    ? _colorAlive
                    : new Color (
                        neighboursColorRainbow.Average(color => color.r),
                        neighboursColorRainbow.Average(color => color.g),
                        neighboursColorRainbow.Average(color => color.b),
                        neighboursColorRainbow.Average(color => color.a)
                    );
            case ColorMode.Migration:
                List<Color> neighboursColorMigration = CellGetNeighboursColor(cell);
                return neighboursColorMigration.Count == 0
                    ? _colorAlive
                    : neighboursColorMigration
                        .GroupBy(i => i)
                        .OrderByDescending(grp => grp.Count())
                        .Select(grp => grp.Key)
                        .First();
            case ColorMode.Random:
                return GenerateRandomColor();
            default:
                return _colorAlive;
        }
    }

    private void CellSetChanges(Cell cell, ColorMode colorMode, bool status)
    {
        cell.isAliveNext = status;
        cell.ColorNext = cell.isAliveNext
            ? CellComputeColor(cell, colorMode)
            : ColorDead;
    }

    private void CellApplyChanges(Cell cell)
    {
        if (cell.data.IsAlive != cell.isAliveNext)
        {
            cell.data.IsAlive = cell.isAliveNext;
            for (int Y = (cell.data.Y == 0) ? cell.data.Y : cell.data.Y - 1,
                YMax = (cell.data.Y == _height - 1) ? cell.data.Y : cell.data.Y + 1; 
                Y <= YMax;
                ++Y)
            {
                for (int X = (cell.data.X == 0) ? cell.data.X : cell.data.X - 1,
                    XMax = (cell.data.X == _width - 1) ? cell.data.X : cell.data.X + 1;
                    X <= XMax;
                    ++X)
                {
                    if (X == cell.data.X && Y == cell.data.Y)
                        continue;
                    _cells[Y, X].neighbourCounter += cell.data.IsAlive ? 1 : -1;
                }
            }
        }
        if (cell.data.Color != cell.ColorNext)
        {
            cell.data.Color = cell.ColorNext;
            cell.GetComponent<SpriteRenderer>().color = cell.data.Color;
        }
    }
    
    private void SetDefaultColor(Color newColor)
    {
        _colorAlive = newColor;
        DefaultRedSlider.value = _colorAlive.r;
        DefaultGreenSlider.value = _colorAlive.g;
        DefaultBlueSlider.value = _colorAlive.b;
        DefaultAlphaSlider.value = _colorAlive.a;
        DefaultColorPreviewImage.color = _colorAlive;
    }

    private Color GenerateRandomColor()
    {
        return new Color(
            Random.value,
            Random.value,
            Random.value,
            _randomAlpha
                ? Random.value
                : 255
        );
    }
    
    public void PlaySingle()
    {
        _singleRun = !_continuousRun;
        _continuousRun = false;
    }

    public void PlayContinuous()
    {
        _continuousRun = !_continuousRun;
    }
    
    public void SetRandomRate(float newRate)
    {
        _randomRate = newRate;
        RandomRateSlider.value = _randomRate;
    }
    
    public void SetRandomAlpha(bool randomAlpha)
    {
        _randomAlpha = randomAlpha;
        RandomAlphaToggle.isOn = _randomAlpha;
    }
    
    public void SetColorMode(int intColorMode)
    {
        if (Enum.IsDefined(typeof(ColorMode), intColorMode))
            _colorMode = (ColorMode)intColorMode;
        ColorModeDropdown.value = intColorMode;
    }

    public void SetWidth(string stringWidth)
    {
        if (int.TryParse(stringWidth, out var newWidth))
            Resize(newWidth, _height);
        else
            WidthInputField.text = _width.ToString();
    }
    
    public void SetHeight(string stringHeight)
    {
        if (int.TryParse(stringHeight, out var newHeight))
            Resize(_width, newHeight);
        else
            HeightInputField.text = _height.ToString();
    }

    public void SetDefaultColorRed(float r)
    {
        SetDefaultColor(new Color(r, _colorAlive.g, _colorAlive.b, _colorAlive.a));
    }
    
    public void SetDefaultColorGreen(float g)
    {
        SetDefaultColor(new Color(_colorAlive.r, g, _colorAlive.b, _colorAlive.a));
    }
    
    public void SetDefaultColorBlue(float b)
    {
        SetDefaultColor(new Color(_colorAlive.r, _colorAlive.g, b, _colorAlive.a));
    }
    
    public void SetDefaultColorAlpha(float a)
    {
        SetDefaultColor(new Color(_colorAlive.r, _colorAlive.g, _colorAlive.b, a));
    }
    
    public void SetRandomDefaultColor()
    {
        SetDefaultColor(GenerateRandomColor());
    }

    public void SetCursor(bool[,] shapeCursor)
    {
        _currentCursor = shapeCursor;
    }

    public void RemoveCursor()
    {
        _currentCursor = _defaultCursor;
    }

    public void MouseHoverCell(Cell hoveredCell)
    {
        var height = Mathf.Min(_currentCursor.GetLength(0), _cells.GetLength(0) - hoveredCell.data.Y);
        var width = Mathf.Min(_currentCursor.GetLength(1), _cells.GetLength(1) - hoveredCell.data.X);

        for (int Y = 0; Y < height; ++Y)
        {
            for (int X = 0; X < width; ++X)
            {
                var cell = _cells[hoveredCell.data.Y + Y, hoveredCell.data.X + X];
                if (_currentCursor[Y, X])
                {
                    cell.GetComponent<SpriteRenderer>().color = cell.data.Color + HoverColor;
                }
            }
        }

        _hoveredCell = hoveredCell;
    }
    
    public void MouseLeaveCell(Cell hoveredCell)
    {
        var height = Mathf.Min(_currentCursor.GetLength(0), _cells.GetLength(0) - hoveredCell.data.Y);
        var width = Mathf.Min(_currentCursor.GetLength(1), _cells.GetLength(1) - hoveredCell.data.X);
        
        for (int Y = 0; Y < height; ++Y)
        {
            for (int X = 0; X < width; ++X)
            {
                var cell = _cells[hoveredCell.data.Y + Y, hoveredCell.data.X + X];
                if (_currentCursor[Y, X])
                {
                    cell.GetComponent<SpriteRenderer>().color = cell.data.Color;
                }
            }
        }

        _hoveredCell = null;
    }
    
    public void OnCellPressed(Cell hoveredCell)
    {
        MouseLeaveCell(hoveredCell);
        var height = Mathf.Min(_currentCursor.GetLength(0), _cells.GetLength(0) - hoveredCell.data.Y);
        var width = Mathf.Min(_currentCursor.GetLength(1), _cells.GetLength(1) - hoveredCell.data.X);
        for (int Y = 0; Y < height; ++Y)
        {
            for (int X = 0; X < width; ++X)
            {
                var cell = _cells[hoveredCell.data.Y + Y, hoveredCell.data.X + X];
                if (_currentCursor[Y, X])
                {
                    CellSetChanges(cell, _colorMode, !cell.data.IsAlive);
                    CellApplyChanges(cell);
                }
            }
        }
    }

    public void OnShapeDrop()
    {
        if (_hoveredCell)
            OnCellPressed(_hoveredCell);
    }

    public void OnBMPImported(BMPImage image)
    {
        if (!Resize(image.info.absWidth, image.info.absHeight))
            return;
        
        for (int Y = 0; Y < _height; ++Y)
        {
            for (int X = 0; X < _width; ++X)
            {
                var pixelColors = image.imageData[Y * _width + X];
                var pixelStatus =  pixelColors.a >= AlphaMinForPixel && (pixelColors.r >= BlackMinForPixel || pixelColors.g >= BlackMinForPixel && pixelColors.b >= BlackMinForPixel);
                var cell = _cells[Y, X];
                if (pixelStatus != cell.data.IsAlive)
                {
                    CellSetChanges(cell, _colorMode, !cell.data.IsAlive);
                    CellApplyChanges(cell);
                }
            }
        }
    }
}
