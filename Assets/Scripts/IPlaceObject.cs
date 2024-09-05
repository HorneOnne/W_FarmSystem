using UnityEngine;

public interface IPlaceObject
{
    public Transform Transform { get; set; }
    public Vector3Int LastestIntPosition{get;set;}
    public Vector2Int Size{get;set;}
    public int[] OccupiedIndices {get;set;}
}
