using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlacedObject : MonoBehaviour, IPlaceObject
{
    public Transform Model{get; private set;}
    public Transform Transform { get; set; }
    public Vector3Int LastestIntPosition{get;set;}

    [field: SerializeField] public Vector2Int Size { get; set; }

     public int[] OccupiedIndices {get;set;}
     private BoxCollider _boxCollider;


    private void Awake()
    {
        this._boxCollider = GetComponent<BoxCollider>();
        _boxCollider.center = new Vector3(0.5f, 0.5f, 0.5f);
        this.Transform = this.transform;
        Model = transform.Find("Model");
        OccupiedIndices = new int[Size.x * Size.y];
        for(int i = 0; i < OccupiedIndices.Length; i++)
            OccupiedIndices[i] = -1;
    }


    public Vector3Int GetIntPosition()
    {
        return new Vector3Int(Mathf.FloorToInt(transform.position.x),
                              Mathf.FloorToInt(transform.position.y + 0.01f),
                              Mathf.FloorToInt(transform.position.z));
    }
}
