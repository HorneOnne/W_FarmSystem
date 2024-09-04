using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlacedObject : MonoBehaviour, IPlaceObject
{
    public Transform Transform { get; set; }
    public Vector3Int LastestIntPosition{get;set;}
    [field: SerializeField] public int Width { get; set; }
    [field: SerializeField] public int Depth { get; set; }
     public int[] OccupiedIndices {get;set;}


     private BoxCollider _boxCollider;


    private void Awake()
    {
        this._boxCollider = GetComponent<BoxCollider>();
        _boxCollider.center = new Vector3(0.5f, 0.5f, 0.5f);
        this.Transform = this.transform;
        OccupiedIndices = new int[Width * Depth];
        for(int i = 0; i < OccupiedIndices.Length; i++)
            OccupiedIndices[i] = -1;
    }


    public Vector3Int GetIntPosition()
    {
        return new Vector3Int(Mathf.FloorToInt(transform.position.x),
                              Mathf.FloorToInt(0),
                              Mathf.FloorToInt(transform.position.z));
    }

   
    private void OnMouseDown()
    {
        if(PlaceObjectSystem.Instance.CurrentPlaceObject == null)
            PlaceObjectSystem.Instance.SetCurrentPlaceObject(this);


    }

    private void OnMouseUp()
    {
        if(PlaceObjectSystem.Instance.CurrentPlaceObject == this)
            PlaceObjectSystem.Instance.ReleaseCurrentPlaceObject();
    }
}
