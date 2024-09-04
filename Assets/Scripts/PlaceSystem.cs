using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlaceSystem : MonoBehaviour
{
    public static PlaceSystem Instance { get; private set; }
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _maxRayDistance = 100f;
    public PlacedObject _currentPlaceObject;
    private Camera _mainCam;


    private Vector3Int _lastestPlacedObjectIntPosition;

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



    private void OnDisable()
    {
        Debug.Log("Disable");
    }


    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.N))
        // {
        //     var placeObjectPrefab = Resources.Load<PlacedObject>("PlacedObject");
        //     if (placeObjectPrefab != null)
        //     {
        //         Debug.Log("A");
        //         if (GetTargetMousePosition(out Vector3 point))
        //         {
        //             Instantiate(placeObjectPrefab, point, quaternion.identity);
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log("C");
        //     }
        // }



        if (_currentPlaceObject != null)
        {
            if (GetTargetMousePosition(out Vector3 point))
            {
                _currentPlaceObject.transform.position = point;

                if (_lastestPlacedObjectIntPosition != _currentPlaceObject.GetIntPosition())
                {
                    GridSystem.Instance.ClearGridAt(_currentPlaceObject);

                    bool canPlace = GridSystem.Instance.CanPlaceObject(_currentPlaceObject);
                    Debug.Log($"canplace: {canPlace}");
                    if (canPlace)
                    {
                        GridSystem.Instance.SetPlaceObject(_currentPlaceObject);
                    }

                    _lastestPlacedObjectIntPosition = _currentPlaceObject.GetIntPosition();
                    _currentPlaceObject.LastestPosition = _lastestPlacedObjectIntPosition;
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
            point = default;
            return false;
        }
    }

    public void SetCurrentPlaceObject(PlacedObject placeObject)
    {
        this._currentPlaceObject = placeObject;
    }
}

[System.Flags]
public enum ObjectPlaceState
{
    Empty = 0,
    Occupied = 1,
    Hover = 2,
}