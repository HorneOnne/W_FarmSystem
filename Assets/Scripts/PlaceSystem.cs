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
    public event System.Action<PlacedObject> OnMouseDownPlacedObject;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _maxRayDistance = 100f;
    public PlacedObject _currentPlaceObject;
    private Camera _mainCam;

    #region Properties
    public PlacedObject CurrentPlaceObject { get => _currentPlaceObject; }

    #endregion


    // Test
    public Transform HitObjectTesting;

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
        if (_currentPlaceObject != null)
        {
            bool hitOnPlane = GetTargetMousePosition(out Vector3 point);
             _currentPlaceObject.transform.position = point - new Vector3(_currentPlaceObject.Size.x / 2.0f, 0, _currentPlaceObject.Size.y / 2.0f);
            if (hitOnPlane)
            {
                if (_currentPlaceObject.LastestIntPosition != _currentPlaceObject.GetIntPosition())
                {
                    _currentPlaceObject.LastestIntPosition = _currentPlaceObject.GetIntPosition();
                    GridSystem.Instance.ClearGridObject(_currentPlaceObject);
                    bool canPlace = GridSystem.Instance.CanPlaceObject(_currentPlaceObject);
                    if (canPlace)
                    {
                        GridSystem.Instance.SetHoverObject(_currentPlaceObject);
                        OnPlacedObjectMove?.Invoke(_currentPlaceObject);
                    }
                }
            }
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

    public void SetCurrentPlaceObject(PlacedObject placeObject)
    {
        if (placeObject == null)
        {
            Debug.LogError("placeObject should not null.");
        }
        this._currentPlaceObject = placeObject;
        OnMouseDownPlacedObject?.Invoke(_currentPlaceObject);
    }

    public void ReleaseCurrentPlaceObject()
    {
        if (this._currentPlaceObject == null)
        {
            Debug.LogError("_currentPlaceObject should not null.");
        }

        if (GridSystem.Instance.CanPlaceObject(_currentPlaceObject) == false) return;

        GridSystem.Instance.SetPlaceObject(_currentPlaceObject);
        OnObjectPlaced?.Invoke(_currentPlaceObject);
        // update position
        _currentPlaceObject.transform.position = _currentPlaceObject.LastestIntPosition;
        this._currentPlaceObject = null;
    }
}

[System.Flags]
public enum ObjectPlaceState
{
    Empty = 1 << 0,
    Occupied = 1 << 1,
    Hover = 1 << 2,
}