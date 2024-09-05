using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteractionManager : MonoBehaviour
{
    public static ObjectInteractionManager Instance { get; private set; }
    public static event System.Action<PlacedObject> OnInteractObjectHasSet;
    public static event System.Action OnInteractObjectReleased;
    public PlacedObject InteractiveObject;
    private Camera _mainCam;
    [SerializeField] private float _maxRayDistance = 100f;
    [SerializeField] private Material _outlineMat;


    // interact
    private float _interactwaitTime = 0.1f;
    private float _interactwaitTimeCounter = 0.0f;

    #region Properties
    [field: SerializeField] public bool CanInteract { get; set; }
    [field: SerializeField] public bool CanMove { get; set; } = false;
    [field: SerializeField] public bool CanRotate { get; set; } = false;
    #endregion

    private void Awake()
    {
        Instance = this;
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (CanInteract == false &&
            HasInteractiveObject())
        {
            _interactwaitTimeCounter += Time.deltaTime;
            if (_interactwaitTimeCounter > _interactwaitTime)
            {
                CanInteract = true;
                _interactwaitTimeCounter = 0;
            }
        }

        if (Input.GetMouseButtonDown(0) &&
           !IsPointerOverUIElement() &&
           CanRotate == false)
        {
            Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance))
            {
                var hitObj = hit.collider.GetComponent<PlacedObject>();

                if (InteractiveObject != hitObj)
                {
                    // Remove outline from the previous object
                    if (InteractiveObject != null)
                    {
                        RemoveMaterial(InteractiveObject.Model.GetComponent<Renderer>(), _outlineMat);
                        OnInteractObjectReleased?.Invoke();
                        ResetInteractState();
                    }

                    // Set and outline the new object
                    InteractiveObject = hitObj;
                    if (InteractiveObject != null)
                    {
                        AddMaterial(InteractiveObject.Model.GetComponent<Renderer>(), _outlineMat);
                        OnInteractObjectHasSet?.Invoke(InteractiveObject);
                        ResetInteractState();
                    }
                }
                else
                {
                    Debug.Log("Equal");
                }
            }
            else
            {
                // If raycast does not hit, remove outline from the current object
                if (InteractiveObject != null)
                {
                    RemoveMaterial(InteractiveObject.Model.GetComponent<Renderer>(), _outlineMat);
                    InteractiveObject = null;
                    OnInteractObjectReleased?.Invoke();
                    ResetInteractState();
                }
            }
        }
    }


    public void OnInteractiveObjectReleaseTriggered()
    {
        RemoveMaterial(InteractiveObject.Model.GetComponent<Renderer>(), _outlineMat);
        InteractiveObject = null;
        OnInteractObjectReleased?.Invoke();
        ResetInteractState();
    }

    public void AddMaterial(Renderer renderer, Material materialToAdd)
    {
        if (renderer == null || materialToAdd == null) return;
        Material[] currentMaterials = renderer.materials;
        Material[] newMaterials = new Material[currentMaterials.Length + 1];
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            newMaterials[i] = currentMaterials[i];
        }
        newMaterials[currentMaterials.Length] = materialToAdd;
        renderer.materials = newMaterials;
    }

    public void RemoveMaterial(Renderer renderer, Material materialToRemove)
    {
        if (renderer == null || materialToRemove == null) return;
        Material[] currentMaterials = renderer.materials;
        Material[] newMaterials = new Material[currentMaterials.Length - 1];
        int newIndex = 0;
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            if (currentMaterials[i] != materialToRemove)
            {
                if (newIndex < newMaterials.Length)
                {
                    newMaterials[newIndex] = currentMaterials[i];
                    newIndex++;
                }
            }
        }
        renderer.materials = newMaterials;
    }


    public bool HasInteractiveObject()
    {
        return InteractiveObject != null;
    }

    private void ResetInteractState()
    {
        CanInteract = false;
        CanMove = false;
        CanRotate = false;
        _interactwaitTimeCounter = 0;
    }

    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Debug.Log("Clicked UI");
            return true;
        }
        else
        {
            // Debug.Log("Clicked Not UI");
            return false;
        }
    }
}
