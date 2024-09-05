using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectSystem : MonoBehaviour
{
    public static event System.Action<ObjectRotation> OnRotateToLeft;
    public static event System.Action<ObjectRotation> OnRotateToRight;
    public ObjectRotation ObjectRot;
    private bool _isDragging;
    private Vector3 _previousMousePosition;


    private void OnEnable()
    {
        PlaceObjectSystem.Instance.OnObjectPlaced += OnObjectPlacedEventTriggered;
    }



    private void OnDisable()
    {
        PlaceObjectSystem.Instance.OnObjectPlaced -= OnObjectPlacedEventTriggered;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReleaseRotatedObject();
        }

        // Start dragging
        if (Input.GetMouseButtonDown(0) && ObjectRot != null) // 0 is the left mouse button
        {
            _previousMousePosition = Input.mousePosition;
            _isDragging = true;
        }

        // Dragging
        if (Input.GetMouseButton(0) && _isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 dragDelta = currentMousePosition - _previousMousePosition;

            // Check the direction of the drag
            if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y)) // More horizontal than vertical
            {
                if (dragDelta.x > 0)
                {
                    ObjectRot.RotateToRight();
                    OnRotateToRight?.Invoke(ObjectRot);
                }
                else if (dragDelta.x < 0)
                {
                    ObjectRot.RotateToLeft();
                    OnRotateToLeft?.Invoke(ObjectRot);
                }
            }

            // Update the previous mouse position for the next frame
            _previousMousePosition = currentMousePosition;
        }

        // End dragging
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false; // Reset dragging state
        }
    }

    public void SetRotatedObject(ObjectRotation objectRotation)
    {
        this.ObjectRot = objectRotation;
    }

    public void ReleaseRotatedObject()
    {
        this.ObjectRot = null;
    }

    private void OnObjectPlacedEventTriggered(PlacedObject obj)
    {
        SetRotatedObject(obj.GetComponent<ObjectRotation>());
    }
}
