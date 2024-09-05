using UnityEngine;
using UnityEngine.UI;

public class UIPlaceSystem : MonoBehaviour
{
    [SerializeField] private Button _rotBtn;
    [SerializeField] private Button _moveBtn;
    [SerializeField] private Button _okBtn;
    [SerializeField] private Transform _utilsTransform;
    [SerializeField] private Transform _rotImageTransform;
    [SerializeField] private Transform _moveImageTransform;
    public Transform TargetTransform;
    private Vector2Int _targetObjectTransformSize;


    private Camera _mainCam;
    private void Awake()
    {
        _mainCam = Camera.main;
        _moveImageTransform.gameObject.SetActive(false);
        _rotImageTransform.gameObject.SetActive(false);
        _okBtn.gameObject.SetActive(false);
        _utilsTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        ObjectInteractionManager.OnInteractObjectHasSet += OnInteractObjectHasSet;
        ObjectInteractionManager.OnInteractObjectReleased += SetTargetReleased;
        PlaceObjectSystem.Instance.OnObjectPlaced += PlaceObjectEvent;


        _moveBtn.onClick.AddListener(() =>
        {
            _moveImageTransform.gameObject.SetActive(true);
             _utilsTransform.gameObject.SetActive(false);
             ObjectInteractionManager.Instance.CanMove = true;
        });

        _rotBtn.onClick.AddListener(() =>
        {
            _rotImageTransform.gameObject.SetActive(true);
            _utilsTransform.gameObject.SetActive(false);
            ObjectInteractionManager.Instance.CanRotate = true;
            RotateObjectSystem.Instance.SetRotatedObject(ObjectInteractionManager.Instance.InteractiveObject.GetComponent<ObjectRotation>());
            _okBtn.gameObject.SetActive(true);
        });

        _okBtn.onClick.AddListener(() =>
        {
            ObjectInteractionManager.Instance.CanRotate = false;
             _rotImageTransform.gameObject.SetActive(false);
             _okBtn.gameObject.SetActive(false);

            // SetTargetReleased();
            RotateObjectSystem.Instance.ReleaseRotatedObject();
            ObjectInteractionManager.Instance.OnInteractiveObjectReleaseTriggered();
        });
    }


    private void OnDestroy()
    {
        ObjectInteractionManager.OnInteractObjectHasSet -= OnInteractObjectHasSet;
        ObjectInteractionManager.OnInteractObjectReleased -= SetTargetReleased;
        PlaceObjectSystem.Instance.OnObjectPlaced -= PlaceObjectEvent;

        _moveBtn.onClick.RemoveAllListeners();
        _rotBtn.onClick.RemoveAllListeners();
        _okBtn.onClick.RemoveAllListeners();
    }
    private void Update()
    {
        // update ui positions
        if (TargetTransform == null) return;
        _moveImageTransform.transform.position = TargetTransform.position + new Vector3(_targetObjectTransformSize.x / 2.0f, 1, _targetObjectTransformSize.y / 2.0f);
        _rotImageTransform.transform.position = TargetTransform.position + new Vector3(_targetObjectTransformSize.x / 2.0f, 1, _targetObjectTransformSize.y / 2.0f);
        _okBtn.transform.position = TargetTransform.position + new Vector3(_targetObjectTransformSize.x / 2.0f, 2.5f, _targetObjectTransformSize.y / 2.0f);

        _utilsTransform.transform.position = TargetTransform.position + new Vector3(_targetObjectTransformSize.x / 2.0f, 2.5f, _targetObjectTransformSize.y / 2.0f);
        FaceToCamera(_utilsTransform);
        FaceToCamera(_okBtn.transform);
    }



    private void OnInteractObjectHasSet(PlacedObject obj)
    {
        TargetTransform = obj.transform;
        _targetObjectTransformSize = TargetTransform.GetComponent<PlacedObject>().Size;
        // _moveImageTransform.gameObject.SetActive(true);
        // _rotImageTransform.gameObject.SetActive(false);
        // _rotBtn.gameObject.SetActive(false);

        _utilsTransform.transform.position = TargetTransform.position + new Vector3(_targetObjectTransformSize.x / 2.0f, 2.5f, _targetObjectTransformSize.y / 2.0f);
        FaceToCamera(_utilsTransform);
        _utilsTransform.gameObject.SetActive(true);
    }


    private void PlaceObjectEvent(PlacedObject obj)
    {
        // TargetTransform = null;
        _moveImageTransform.gameObject.SetActive(false);
        _rotBtn.gameObject.SetActive(true);
    }

    private void FaceToCamera(Transform targetTransform)
    {
        Vector3 directionToCamera = _mainCam.transform.position - targetTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
        targetTransform.rotation = targetRotation;
    }

    private void SetTargetReleased()
    {
        _moveImageTransform.gameObject.SetActive(false);
        _rotImageTransform.gameObject.SetActive(false);
        // _rotBtn.gameObject.SetActive(false);
        // _okBtn.gameObject.SetActive(false);

        _utilsTransform.gameObject.SetActive(false);
    }

}
