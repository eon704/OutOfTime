using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float speed;
  [SerializeField] private CinemachineCamera playerCamera;
  [SerializeField] private Transform cameraPivot; // pivot used for pitch rotation (child of player)
  [SerializeField] private float lookSensitivity = 1f;
  [SerializeField] private float minPitch = -30f;
  [SerializeField] private float maxPitch = 60f;

  [Header("Jump / Gravity")]
  [SerializeField] private float gravity = -9.81f;
  [SerializeField] private float jumpHeight = 1.5f;

  private CharacterController _characterController;

  private float _pitch;
  private float _verticalVelocity;

  private void Awake()
  {
    // Try to find a CharacterController on this object or its children
    _characterController = GetComponent<CharacterController>();
    if (_characterController == null)
    {
      _characterController = GetComponentInChildren<CharacterController>();
    }

    if (_characterController == null)
    {
      Debug.LogWarning("PlayerController: No CharacterController found on the player or its children.");
    }
  }

  public void Move(Vector3 direction)
  {
    // If we don't have a CharacterController, fall back to simple transform movement
    float step = speed * Time.deltaTime;
    if (_characterController == null)
    {
      transform.Translate(direction * step, Space.World);
      return;
    }

    // Horizontal movement
    Vector3 move = direction * step;

    // Grounded check and gravity
    if (_characterController.isGrounded && _verticalVelocity < 0f)
    {
      // small negative to keep controller grounded
      _verticalVelocity = -2f;
    }

    // Apply gravity
    _verticalVelocity += gravity * Time.deltaTime;

    // Apply vertical movement
    float vStep = _verticalVelocity * Time.deltaTime;
    move.y = vStep;

    _characterController.Move(move);
  }

  public void Look(Vector2 delta)
  {
    // delta is expected to be (deltaX, deltaY) in pixels or normalized input.
    // Yaw: rotate the player around Y axis. Pitch: rotate the camera pivot around X axis and clamp.
    if (cameraPivot == null)
    {
      // Fallback: rotate camera if available, otherwise rotate the transform yaw only
      float yawOnly = delta.x * lookSensitivity;
      transform.Rotate(Vector3.up, yawOnly);
      return;
    }

    float yaw = delta.x * lookSensitivity;
    float pitchDelta = -delta.y * lookSensitivity; // invert Y for typical look controls

    // Rotate player (yaw)
    transform.Rotate(Vector3.up, yaw);

    // Adjust and clamp pitch
    _pitch = Mathf.Clamp(_pitch + pitchDelta, minPitch, maxPitch);
    cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
  }

  public void Jump()
  {
    // Only allow jump when grounded (CharacterController path)
    if (_characterController == null) return;

    if (_characterController.isGrounded)
    {
      // v = sqrt(2 * g * h) ; gravity is negative so multiply by -1
      _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
  }

  public void Fire()
  {
    // Implement firing logic here
  }

  public void Interact()
  {
    // Implement interaction logic here
  }
}