using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction m_moveAction;

    private Vector2 m_moveAmt;
    private CharacterController m_characterController;

    private Animator m_animator;

    public Transform cameraTransform;

    public float walkSpeed = 5f;

    //private void OnEnable()
    //{
    //    InputActions.FindActionMap("Player").Enable();
    //}

    //private void OnDisable()
    //{
    //    InputActions.FindActionMap("Player").Disable();
    //}

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();

        m_moveAction = InputSystem.actions.FindAction("Move");

        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();

        Debug.Log("Player Speed: " + m_moveAmt.magnitude);
    }

    private void FixedUpdate()
    {
        Walking();
    }

    private void Walking()
    {
        Vector3 inputDir = new Vector3(m_moveAmt.x, 0, m_moveAmt.y);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputDir.z + camRight * inputDir.x;

        if (move.magnitude > 0.1f) // obrót kamery wzglêdem obrotu postaci
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        m_characterController.SimpleMove(move * walkSpeed);

        m_animator.SetFloat("Speed", m_moveAmt.magnitude);

        m_animator.speed = move.magnitude > 0.1f ? move.magnitude : 1f;

    }

}