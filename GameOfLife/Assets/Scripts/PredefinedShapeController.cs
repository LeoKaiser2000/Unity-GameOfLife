using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedShapeController : MonoBehaviour
{
    [SerializeField] private ShapeIcon OriginalShape;
    private List<ShapeIcon> PredefinedShapes = new List<ShapeIcon>();
    [SerializeField] private Vector2 ShapeDistance;
    [SerializeField] private CellsMapController GameCellsController;

    // Start is called before the first frame update
    void Start()
    {
        var bitmapTab = new bool[2][,];
        bitmapTab[0] = new bool[3,3];
        for (int Y = 0; Y < bitmapTab[0].GetLength(0); ++Y)
            for (int X = 0; X < bitmapTab[0].GetLength(1); ++X)
                bitmapTab[0][Y, X] = !(Y == 1 && X == 1);
        bitmapTab[1] = new bool[3,1];
        for (int Y = 0; Y < bitmapTab[1].GetLength(0); ++Y)
            bitmapTab[1][Y, 0] = true;
        
        for (int bitmapIt = 0; bitmapIt < bitmapTab.Length; ++bitmapIt)
        {
            var shape = InitShape(bitmapIt, bitmapTab[bitmapIt]);
            PredefinedShapes.Add(shape);
        }
    }

    private ShapeIcon InitShape(int X, bool[,] bitmap)
    {
        ShapeIcon shapeIcon = Instantiate(
            OriginalShape,
            new Vector2(
                X * ShapeDistance.x + transform.position.x,
                transform.position.y
            ),
            Quaternion.identity,
            transform
        );
        shapeIcon.GetComponent<Shape>().ShapeFromBitmap(bitmap);
        shapeIcon.GameCellsController = GameCellsController;
        return shapeIcon;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
