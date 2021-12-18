using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shape : MonoBehaviour
{
    [SerializeField] private UICell OriginalCell;
    private RectTransform _originalCellRect;
    private UICell[,] Cells = new UICell[0,0];
    public bool[,] Bitmap = new bool[0,0];
    [SerializeField] private Vector2 CellScale;
    [SerializeField] private Color AliveColor;
    [SerializeField] private Color DeadColor;
    
    // Start is called before the first frame update
    public void ShapeFromBitmap(bool[,] bitmap)
    {
        var height = bitmap.GetLength(0);
        var width = bitmap.GetLength(1);
        var oldHeight = Cells.GetLength(0);
        var oldWidth = Cells.GetLength(1);

        var temporaryTab = new UICell[height, width];
        
        for (int Y = 0; Y < oldHeight; ++Y)
        {
            for (int X = 0; X < oldWidth; ++X)
            {
                if (Y >= height || X >= width)
                {
                    Destroy(Cells[Y, X].gameObject);
                }
            }
        }

        for (int Y = 0; Y < height; ++Y)
        {
            for (int X = 0; X < width; ++X)
            {
                temporaryTab[Y, X] = (Y < oldHeight && X < oldWidth) ? Cells[Y, X] : CellInstantiate(X, Y, bitmap[Y, X]);
            }
        }

        Bitmap = bitmap;
    }
    
    private UICell CellInstantiate(int X, int Y, bool alive)
    {
        _originalCellRect = OriginalCell.GetComponent<RectTransform>();
        UICell cell = Instantiate(
            OriginalCell,
            new Vector2(
                X * _originalCellRect.rect.width + transform.position.x,
                Y * _originalCellRect.rect.height + transform.position.y),
            Quaternion.identity,
            transform
        );
        cell.GetComponent<Image>().color = alive ? AliveColor : DeadColor;
        return cell;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
