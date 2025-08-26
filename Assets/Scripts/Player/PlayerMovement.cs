using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Player Movement

    public InputActionAsset InputActions;

    private InputAction m_moveAction;
    private Vector2 m_moveAmt;
    private CharacterController m_characterController;
    private Animator m_animator;

    public Transform cameraTransform;

    private PlayerAttack m_playerAttack;

    private PlayerStatistics stats;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_moveAction = InputActions.FindActionMap("Player").FindAction("Move");
        m_characterController = GetComponent<CharacterController>();
        m_playerAttack = GetComponent<PlayerAttack>();
    }

    private void Start()
    {
        GameObject statManager = GameObject.FindGameObjectWithTag("Stat Manager");
        if (statManager != null)
        {
            stats = statManager.GetComponent<PlayerStatistics>();
        }
    }

    private void Update()
    {
        m_moveAmt = m_moveAction.ReadValue<Vector2>();
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

        // obrót postaci tylko gdy nie atakuje
        if (move.magnitude > 0.1f && (m_playerAttack == null || !m_playerAttack.IsTargetingEnemy))
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        m_characterController.SimpleMove(move * stats.moveSpeed);

        m_animator.SetFloat("Speed", m_moveAmt.magnitude);
        m_animator.speed = move.magnitude > 0.1f ? move.magnitude : 1f;
    }

    #endregion
}
