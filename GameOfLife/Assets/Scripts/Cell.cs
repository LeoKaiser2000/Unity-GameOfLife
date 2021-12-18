using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    public CellData data = new CellData();

    public bool isAliveNext;

    public Color ColorNext;

    public int neighbourCounter = 0;

    public CellsMapController controller;

    public void OnMouseDown()
    {
        controller.OnCellPressed(this);
    }
    
    public void OnMouseOver()
    {
        controller.MouseHoverCell(this);
    }

    public void OnMouseExit()
    {
        controller.MouseLeaveCell(this);
    }

    // Start is called before the first frame update
    // void Start() { }

    // Update is called once per frame
    // void Update() { }
}
