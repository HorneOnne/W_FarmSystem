using UnityEngine;

public interface IPlaceObject
{
    public Transform Transform { get; set; }
    public Vector3Int LastestIntPosition{get;set;}
    public int Width { get; set; }
    public int Depth { get; set; }
    public int[] OccupiedIndices {get;set;}
}
