using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlacedObject : MonoBehaviour, IPlaceObject
{
    public Vector3 Position { get; set; }
    public Vector3Int LastestIntPosition{get;set;}
    [field: SerializeField] public int Width { get; set; }
    [field: SerializeField] public int Depth { get; set; }
     public int[] OccupiedIndices {get;set;}


    private void Awake()
    {
        OccupiedIndices = new int[Width * Depth];
        for(int i = 0; i < OccupiedIndices.Length; i++)
            OccupiedIndices[i] = -1;
        Position = transform.position;
    }

    private void Update()
    {
        Position = transform.position;
    }


    public Vector3Int GetIntPosition()
    {
        return new Vector3Int(Mathf.FloorToInt(transform.position.x),
                              Mathf.FloorToInt(1),
                              Mathf.FloorToInt(transform.position.z));
    }

   
    private void OnMouseDown()
    {
        if(PlaceSystem.Instance.CurrentPlaceObject == null)
            PlaceSystem.Instance.SetCurrentPlaceObject(this);


    }

    private void OnMouseUp()
    {
        if(PlaceSystem.Instance.CurrentPlaceObject == this)
            PlaceSystem.Instance.ReleaseCurrentPlaceObject();
    }
}
