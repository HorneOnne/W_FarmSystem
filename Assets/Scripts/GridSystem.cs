using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
  public static GridSystem Instance { get; private set; }

  public Vector3Int Origin;
  public int Width = 10;
  public int Depth = 10;

  private ObjectPlaceState[] _placedMask;
  [SerializeField] private Transform _visualizeMesh;

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

    // update grid position
    transform.position = Origin;

    // update visual
    Vector3 position = new Vector3(Width / 2.0f, 0f, Depth / 2.0f);
    Vector3 scale = new Vector3(Width, Depth, 1);
    _visualizeMesh.localPosition = position;
    _visualizeMesh.localScale = scale;
  }


  public bool CanPlaceObject(IPlaceObject obj)
  {
    int startX = Mathf.FloorToInt(obj.Transform.position.x) - Origin.x;
    int startZ = Mathf.FloorToInt(obj.Transform.position.z) - Origin.z;
    int endX = startX + obj.Width;
    int endZ = startZ + obj.Depth;

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
  int startX = Mathf.FloorToInt(obj.Transform.position.x) - Origin.x;
    int startZ = Mathf.FloorToInt(obj.Transform.position.z) - Origin.z;
    int endX = startX + obj.Width;
    int endZ = startZ + obj.Depth;

    int index = 0;
    for (int z = startZ; z < endZ; z++)
    {
      for (int x = startX; x < endX; x++)
      {
        _placedMask[x + z * Width] |= ObjectPlaceState.Hover;
        obj.OccupiedIndices[index] = x + z * Width;
        index++;
      }
    }
}

  public void SetPlaceObject(IPlaceObject obj)
  {
    int startX = Mathf.FloorToInt(obj.Transform.position.x) - Origin.x;
    int startZ = Mathf.FloorToInt(obj.Transform.position.z) - Origin.z;
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
    for (int i = 0; i < obj.OccupiedIndices.Length; i++)
    {
      if (obj.OccupiedIndices[i] == -1) return;
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


    Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f) + Origin;
    for (int z = 0; z < Depth; z++)
    {
      for (int x = 0; x < Width; x++)
      {
        if ((_placedMask[x + z * Width] & ObjectPlaceState.Occupied) == ObjectPlaceState.Occupied)
        {
          Gizmos.color = Color.red;
          Gizmos.DrawCube(new Vector3(x, 0, z) + offset, new Vector3(1,0,1));
        }
        else if ((_placedMask[x + z * Width] & ObjectPlaceState.Hover) == ObjectPlaceState.Hover)
        {
          Gizmos.color = Color.green;
          Gizmos.DrawCube(new Vector3(x, 0, z) + offset, new Vector3(1,0,1));
        }
        else 
        {
          Gizmos.color = Color.white;
          Gizmos.DrawWireCube(new Vector3(x, 0, z) + offset, new Vector3(1,0,1));
        }

      }
    }
  }
}
