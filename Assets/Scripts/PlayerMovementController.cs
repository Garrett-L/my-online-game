using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    
    [SerializeField] private InputAction playerControls;
    private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Camera cam;

    void OnEnable() => playerControls.Enable();

    void OnDisable() => playerControls.Disable();

    void Update() {
        float newMoveSpeed = moveSpeed * GetComponentInParent<CameraController>().TargetZoom * Time.deltaTime;
        moveDirection = playerControls.ReadValue<Vector2>();
        transform.position = new Vector3(transform.position.x + moveDirection.x * newMoveSpeed, transform.position.y + moveDirection.y * newMoveSpeed, transform.position.y * 0.1f + 3f);
    }
}