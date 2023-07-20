using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private InputAction zoomControls;
    [SerializeField] private float zoomFactor;
    [SerializeField] private Vector2 zoomClamp;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed;
    private float _targetZoom;

    public float TargetZoom {
        get {
            return _targetZoom;
        }
        set {
            _targetZoom = Mathf.Clamp(value, zoomClamp.x, zoomClamp.y);
        }
    }

    void OnEnable() => zoomControls.Enable();
    void OnDisable() => zoomControls.Disable();

    void Start() {
        TargetZoom = cam.orthographicSize;
    }

    void Update() {
        TargetZoom -= zoomControls.ReadValue<Vector2>().y * zoomFactor * 0.001f;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, TargetZoom, Time.deltaTime * zoomSpeed);
        cam.transform.position = new Vector3(
            Mathf.Lerp(cam.transform.position.x, followTarget.position.x, Time.deltaTime * followSpeed), 
            Mathf.Lerp(cam.transform.position.y, followTarget.position.y, Time.deltaTime * followSpeed), 
            cam.transform.position.z);
    }
}
