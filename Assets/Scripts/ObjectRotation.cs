using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    private Transform _model;
    private const float _rotationSpeed = 200f; // Degrees per second
    private void Awake()
    {
        _model = transform.Find("Model").transform;
        if (_model == null)
        {
            Debug.LogError("Missing Model reference.");
        }
    }

    public void RotateToLeft()
    {
         _model.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    public void RotateToRight()
    {
        _model.Rotate(Vector3.down, _rotationSpeed * Time.deltaTime);
    }
}
