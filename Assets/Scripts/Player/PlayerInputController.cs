using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
  private PlayerController _playerController;
  private PlayerInput _playerInput;

  [Header("Input Settings")]
  [SerializeField] private float lookScale = 1f; // scales raw look input before sending to PlayerController

  private Vector2 _moveInput = Vector2.zero;
  private Vector2 _lookInput = Vector2.zero;
  private Camera _cam;

  private void Awake()
  {
    _cam = Camera.main;
    _playerController = GetComponent<PlayerController>();
    _playerInput = new PlayerInput();
  }

  private void OnEnable()
  {
    // Enable input and subscribe to actions. We subscribe to both performed and canceled
    // for continuous value-style actions so we can cache the current value and apply
    // it every frame from Update().
    _playerInput.Enable();
    _playerInput.Human.Move.performed += OnMove;
    _playerInput.Human.Move.canceled += OnMove;
    _playerInput.Human.Look.performed += OnLook;
    _playerInput.Human.Look.canceled += OnLook;
    _playerInput.Human.Jump.performed += OnJump;
    _playerInput.Human.Fire.performed += OnFire;
    _playerInput.Human.Interact.performed += OnInteract;
  }

  private void OnDisable()
  {
    _playerInput.Human.Move.performed -= OnMove;
    _playerInput.Human.Move.canceled -= OnMove;
    _playerInput.Human.Look.performed -= OnLook;
    _playerInput.Human.Look.canceled -= OnLook;
    _playerInput.Human.Jump.performed -= OnJump;
    _playerInput.Human.Fire.performed -= OnFire;
    _playerInput.Human.Interact.performed -= OnInteract;

    _playerInput.Disable();
  }

  private void OnMove(InputAction.CallbackContext ctx)
  {
    // Cache the current move value (performed/canceled will provide the current Vector2)
    _moveInput = ctx.ReadValue<Vector2>();
  }

  private void OnLook(InputAction.CallbackContext ctx)
  {
    // Cache look input. We'll scale and apply it in Update so Look is consistent per-frame.
    _lookInput = ctx.ReadValue<Vector2>();
  }

  private void OnJump(InputAction.CallbackContext ctx)
  {
    _playerController.Jump();
  }

  private void OnFire(InputAction.CallbackContext ctx)
  {
    _playerController.Fire();
  }

  private void OnInteract(InputAction.CallbackContext ctx)
  {
    _playerController.Interact();
  }

  private void Update()
  {
    if (_playerController == null) return;

    // Convert 2D move input into world-space direction relative to the camera's yaw
    Vector3 moveDir = new Vector3(_moveInput.x, 0f, _moveInput.y);
    if (_cam != null)
    {
      // Use only the camera's yaw (rotation around Y) to compute forward/right.
      // This avoids problems when the camera looks straight up/down (pitch) and
      // ensures forward/back input always maps to ground-forward.
      float camYaw = _cam.transform.eulerAngles.y;
      Quaternion yawRot = Quaternion.Euler(0f, camYaw, 0f);
      Vector3 camForwardYaw = yawRot * Vector3.forward;
      Vector3 camRightYaw = yawRot * Vector3.right;

      // Map input: X = strafe (left/right), Y = forward/back
      float dx = _moveInput.x;
      float dy = _moveInput.y;
      Vector3 raw = camRightYaw * dx + camForwardYaw * dy;
      
      // Clamp magnitude to 1 to avoid faster diagonal speed while preserving analog magnitude.
      float rawSqr = raw.sqrMagnitude;
      moveDir = (rawSqr > 1f) ? raw.normalized : raw;
    }

    // Apply movement (WASD moves character relative to camera direction)
    _playerController.Move(moveDir);

    // Apply look (Mouse rotates camera and player yaw)
    if (_lookInput != Vector2.zero)
    {
      _playerController.Look(_lookInput * lookScale);
    }
  }
}