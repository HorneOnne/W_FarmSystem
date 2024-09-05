using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceObjectSystem : MonoBehaviour
{
    public static PlaceObjectSystem Instance { get; private set; }
    public event System.Action<PlacedObject> OnObjectPlaced;
    public event System.Action<PlacedObject> OnPlacedObjectMove;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _maxRayDistance = 100f;
    //  public PlacedObject _currentPlaceObject;
    private Camera _mainCam;
    private bool _isDragging;
    #region Properties
    // public PlacedObject CurrentPlaceObject { get => _currentPlaceObject; }
    [field: SerializeField] public bool CanDrag { get; set; } = false;

    #endregion



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        _mainCam = Camera.main;
    }


    private void Update()
    {
        if (ObjectInteractionManager.Instance.CanInteract == false) return;
        if (ObjectInteractionManager.Instance.CanMove == false) return;
        // Start dragging
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance))
                {
                    if (hit.collider.TryGetComponent<PlacedObject>(out var hitObj))
                    {
                        if (hitObj == ObjectInteractionManager.Instance.InteractiveObject)
                        {
                            //Debug.Log("same");
                            _isDragging = true;
                            var currentPlaceObject = ObjectInteractionManager.Instance.InteractiveObject;
                            bool hitOnPlane = GetTargetMousePosition(out Vector3 point);
                            currentPlaceObject.transform.position = point - new Vector3(currentPlaceObject.Size.x / 2.0f, 0, currentPlaceObject.Size.y / 2.0f);
                            if (hitOnPlane)
                            {
                                currentPlaceObject.LastestIntPosition = currentPlaceObject.GetIntPosition();
                                GridSystem.Instance.ClearGridObject(currentPlaceObject);
                                bool canPlace = GridSystem.Instance.CanPlaceObject(currentPlaceObject);
                                if (canPlace)
                                {
                                    GridSystem.Instance.SetHoverObject(currentPlaceObject);
                                    OnPlacedObjectMove?.Invoke(currentPlaceObject);
                                }
                            }
                        }
                        else
                        {
                            //Debug.Log("not same");
                        }
                    }
                }
            }


        }

        // Dragging
        if (Input.GetMouseButton(0) && _isDragging)
        {
            var currentPlaceObject = ObjectInteractionManager.Instance.InteractiveObject;
            bool hitOnPlane = GetTargetMousePosition(out Vector3 point);
            currentPlaceObject.transform.position = point - new Vector3(currentPlaceObject.Size.x / 2.0f, 0, currentPlaceObject.Size.y / 2.0f);
            if (hitOnPlane)
            {
                if (currentPlaceObject.LastestIntPosition != currentPlaceObject.GetIntPosition())
                {
                    currentPlaceObject.LastestIntPosition = currentPlaceObject.GetIntPosition();
                    GridSystem.Instance.ClearGridObject(currentPlaceObject);
                    bool canPlace = GridSystem.Instance.CanPlaceObject(currentPlaceObject);
                    if (canPlace)
                    {
                        GridSystem.Instance.SetHoverObject(currentPlaceObject);
                        OnPlacedObjectMove?.Invoke(currentPlaceObject);
                    }
                }
            }
        }

        // End dragging
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false; // Reset dragging state
            ReleaseCurrentPlaceObject();
        }

    }


    public bool GetTargetMousePosition(out Vector3 point)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = _mainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _maxRayDistance, _groundLayer))
        {
            point = hit.point;
            return true;
        }
        else
        {
            // get point hit based on mouse and look direction
            point = Camera.main.transform.position + ray.direction * _maxRayDistance;
            return false;
        }
    }


    public void ReleaseCurrentPlaceObject()
    {
        Debug.Log("release");
        var currentPlaceObject = ObjectInteractionManager.Instance.InteractiveObject;
        if (GridSystem.Instance.CanPlaceObject(currentPlaceObject))
        {
            GridSystem.Instance.SetPlaceObject(currentPlaceObject);
            OnObjectPlaced?.Invoke(currentPlaceObject);
            // update position
            currentPlaceObject.transform.position = currentPlaceObject.LastestIntPosition;
        }

    }
}

[System.Flags]
public enum ObjectPlaceState
{
    Empty = 1 << 0,
    Occupied = 1 << 1,
    Hover = 1 << 2,
}