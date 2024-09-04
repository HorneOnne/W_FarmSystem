using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
  public static GridSystem Instance { get; private set; }

  public int Width = 10;
  public int Depth = 10;

  private ObjectPlaceState[] _placedMask;



  // editor
  [SerializeField] private bool _showGizmos = true;

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
    _placedMask = new ObjectPlaceState[Width * Depth];
    for (int i = 0; i < _placedMask.Length; i++)
    {
      _placedMask[i] |= ObjectPlaceState.Empty;
    }
  }

  private void Start()
  {

  }


  private void Update()
  {

  }




  public bool CanPlaceObject(IPlaceObject obj)
  {
    Debug.Log($"{obj.Position.x}  {obj.Position.x - obj.Width / 2f}");
    int startX = Mathf.FloorToInt(obj.Position.x);
    int startZ = Mathf.FloorToInt(obj.Position.z);
    int endX = startX + obj.Width;
    int endZ = startZ + obj.Depth;


Debug.Log($"Start: {startX} {startZ}");
Debug.Log($"End: {endX} {endZ}");
    for (int z = startZ; z < endZ; z++)
    {
      for (int x = startX; x < endX; x++)
      {
        if (IsValid(x, z) == false)
        {
          return false;
        }

        bool isOccupied = (_placedMask[x + z * Width] & ObjectPlaceState.Occupied) == ObjectPlaceState.Occupied;
        if (isOccupied)
          return false;
      }
    }
    return true;
  }
 

public void SetHoverObject(IPlaceObject obj)
{
  int startX = Mathf.FloorToInt(obj.Position.x);
    int startZ = Mathf.FloorToInt(obj.Position.z);
    int endX = startX + obj.Width;
    int endZ = startZ + obj.Depth;

    int count = 0;
    for (int z = startZ; z < endZ; z++)
    {
      for (int x = startX; x < endX; x++)
      {
        _placedMask[x + z * Width] |= ObjectPlaceState.Hover;
        obj.OccupiedIndices[count] = x + z * Width;
        count++;
      }
    }
}

  public void SetPlaceObject(IPlaceObject obj)
  {
    int startX = Mathf.FloorToInt(obj.Position.x);
    int startZ = Mathf.FloorToInt(obj.Position.z);
    int endX = startX + obj.Width;
    int endZ = startZ + obj.Depth;

    int count = 0;
    for (int z = startZ; z < endZ; z++)
    {
      for (int x = startX; x < endX; x++)
      {
        _placedMask[x + z * Width] |= ObjectPlaceState.Occupied;
        obj.OccupiedIndices[count] = x + z * Width;
        count++;
      }
    }
  }

  public void ResetGrid()
  {
    for (int i = 0; i < _placedMask.Length; i++)
    {
      _placedMask[i] &= ~ObjectPlaceState.Occupied;
    }
  }

  public void ClearGridObject(IPlaceObject obj)
  {
    // return;
    for (int i = 0; i < obj.OccupiedIndices.Length; i++)
    {
      if (obj.OccupiedIndices[i] == -1) continue;
      _placedMask[obj.OccupiedIndices[i]] &= ~ObjectPlaceState.Hover;
      _placedMask[obj.OccupiedIndices[i]] &= ~ObjectPlaceState.Occupied;
      obj.OccupiedIndices[i] = -1;
    }
  }


  private bool IsValid(int x, int z)
  {
    return !(x < 0 || x >= Width || z < 0 || z >= Depth);
  }

  private void OnDrawGizmos()
  {
    if (!_showGizmos) return;
    if (_placedMask == null || _placedMask.Length != Width * Depth) return;


    Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);
    for (int z = 0; z < Depth; z++)
    {
      for (int x = 0; x < Width; x++)
      {
        if ((_placedMask[x + z * Width] & ObjectPlaceState.Hover) == ObjectPlaceState.Hover)
        {
          Gizmos.color = Color.green;
          Gizmos.DrawCube(new Vector3(x, 0, z) + offset, Vector3.one);
        }
        else
        {
          Gizmos.color = Color.white;
          Gizmos.DrawWireCube(new Vector3(x, 0, z) + offset, Vector3.one);
        }

      }
    }
  }
}
