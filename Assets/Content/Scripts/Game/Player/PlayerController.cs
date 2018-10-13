using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum MoveDirection
    {
        None,
        Left,
        Right
    }

    private bool m_isActive;
    private Vector3 m_playerPosition;

    private MoveDirection m_moveDirection = MoveDirection.None;
    private Vector3 m_playerRotation;
    private Vector3 m_playerOriginalRotation;
    private Vector3 m_playerSourceRotation;
    private Vector3 m_playerTargetRotation;
    private float m_startRotationTime;

    private float m_lastSpeedReset;
    private SpringJoint[] m_limbJoints;

    public float HorizontalSpeed = 1.0f;
    public float StartFallSpeed = 2.0f;
    public float MaxFallSpeed = 10.0f;
    public float TimeToTerminalVelocity = 2.0f;
    public float LeanRotationScale = 1.0f;

    public void Start()
    {
        m_limbJoints = GetComponentsInChildren<SpringJoint>();
    }

    public void Update()
    {
        if (!m_isActive)
        {
            return;
        }

        HandleUserInput();
        HandleGravity();
        transform.localPosition = m_playerPosition;
        transform.localEulerAngles = m_playerRotation;
    }

    public void Activate(Vector3 startPosition)
    {
        m_isActive = true;
        m_playerPosition = startPosition;
        m_lastSpeedReset = Time.time;
        transform.localPosition = m_playerPosition;
        m_playerRotation = transform.localEulerAngles;
        m_playerOriginalRotation = m_playerRotation;
        m_playerSourceRotation = m_playerRotation;
        m_playerTargetRotation = m_playerRotation;
        m_startRotationTime = Time.time;
    }

    public void KillPlayer()
    {
        m_isActive = false;

        var limbBodies = new HashSet<Rigidbody>();
        foreach (var joint in m_limbJoints)
        {
            if (joint == null)
            {
                // Already broken limbs.
                continue;
            }

            var limbBody = joint.GetComponent<Rigidbody>();
            if (limbBody != null)
            {
                limbBodies.Add(limbBody);
            }

            Destroy(joint);
        }

        const float killForce = 4.0f;
        foreach (var limbBody in limbBodies)
        {
            limbBody.AddForce(
                new Vector3(
                    Random.Range(-killForce, killForce), 
                    Random.Range(-killForce, killForce), 
                    Random.Range(-killForce, killForce)), 
                ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Apply all user input to the position vectors stored within
    /// the character controller, but don't assign them to the players
    /// actual transform.
    /// </summary>
    private void HandleUserInput()
    {
        var horizontalInput = Input.GetAxis("Horizontal");

        var moveDirection = MoveDirection.None;
        var rotateTarget = Vector3.zero;
        if (horizontalInput < 0.0f)
        {
            moveDirection = MoveDirection.Left;
            rotateTarget = new Vector3(0.0f, -10.0f, 0.0f);
        }
        else if (horizontalInput > 0.0f)
        {
            moveDirection = MoveDirection.Right;
            rotateTarget = new Vector3(0.0f, 10.0f, 0.0f);
        }

        var moveHorizontal = Input.GetAxis("Horizontal") * HorizontalSpeed;
        m_playerPosition += new Vector3(0.0f, 0.0f, moveHorizontal * Time.deltaTime);

        if (moveDirection != m_moveDirection)
        {
            m_startRotationTime = Time.time;
            m_playerSourceRotation = m_playerRotation;
            m_playerTargetRotation = m_playerOriginalRotation + rotateTarget;
            m_moveDirection = moveDirection;
        }

        var rotationTime = Time.time - m_startRotationTime;
        m_playerRotation = Vector3.Lerp(m_playerSourceRotation, m_playerTargetRotation, rotationTime * LeanRotationScale);

        Debug.Log("Direction: " + moveDirection + " - Time: " + rotationTime + " - Target: " + m_playerTargetRotation);

        if (Input.GetButtonDown("Jump"))
        {
            KillPlayer();
        }
    }

    private void HandleGravity()
    {
        var fallTime = Time.time - m_lastSpeedReset;
        var fallScalar = Mathf.Min(1.0f, fallTime / TimeToTerminalVelocity);
        var fallSpeed = Mathf.Lerp(StartFallSpeed, MaxFallSpeed - StartFallSpeed, fallScalar) * Time.deltaTime;
        m_playerPosition -= new Vector3(0.0f, fallSpeed, 0.0f);
    }
}
