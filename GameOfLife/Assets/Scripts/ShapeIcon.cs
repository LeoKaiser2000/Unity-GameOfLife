using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShapeIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private Vector3 _startPosition;
    public Shape Shape;
    public CellsMapController GameCellsController;
    [SerializeField] private Vector2 DistanceFromCursor;

    // Start is called before the first frame update
    void Start()
    {
        Shape = GetComponent<Shape>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - DistanceFromCursor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameCellsController.SetCursor(Shape.Bitmap);
        _startPosition = transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameCellsController.OnShapeDrop();
        GameCellsController.RemoveCursor();
        transform.position = _startPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
