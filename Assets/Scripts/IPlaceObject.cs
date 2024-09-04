using UnityEngine;

public interface IPlaceObject
{
    public Vector3 Position { get; set; }
    public Vector3Int LastestIntPosition{get;set;}
    public int Width { get; set; }
    public int Depth { get; set; }
    public int[] OccupiedIndices {get;set;}
}
