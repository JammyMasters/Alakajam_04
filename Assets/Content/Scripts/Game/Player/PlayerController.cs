using System.Collections;
using System.Linq;
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

    private Vector2 m_mapBounds;

    private MoveDirection m_moveDirection = MoveDirection.None;
    private Vector3 m_playerRotation;
    private Vector3 m_playerOriginalRotation;
    private Vector3 m_playerSourceRotation;
    private Vector3 m_playerTargetRotation;
    private float m_startRotationTime;

    private float m_lastSpeedReset;
    private PlayerLimb[] m_limbs;

    public float HorizontalSpeed = 1.0f;
    public float StartFallSpeed = 2.0f;
    public float MaxFallSpeed = 10.0f;
    public float TimeToTerminalVelocity = 2.0f;
    public float LeanRotationScale = 1.0f;
    public bool KillPlayerOnBodyCollision;

    public void Start()
    {
        m_limbs = GetComponentsInChildren<PlayerLimb>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isActive)
        {
            return;
        }

        if (!KillPlayerOnBodyCollision)
        {
            return;
        }

        KillPlayer();
    }

    public void Spawn(Vector3 startPosition, float mapWidth)
    {
        m_isActive = true;

        m_playerPosition = startPosition;
        m_lastSpeedReset = Time.time;

        mapWidth -= 2.0f;
        m_mapBounds = new Vector2(startPosition.z - (mapWidth * 0.5f), startPosition.z + (mapWidth * 0.5f));

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

        foreach (var limb in m_limbs)
        {
            limb.Detatch();
            limb.EnableBloodParticles(false);
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

        var moveHorizontal = horizontalInput * HorizontalSpeed * GetLimbModifier();
        var oldPosition = m_playerPosition;
        m_playerPosition += new Vector3(0.0f, 0.0f, moveHorizontal * Time.deltaTime);
        if (m_playerPosition.z <= m_mapBounds.x || m_playerPosition.z >= m_mapBounds.y)
        {
            m_playerPosition = oldPosition;
            moveDirection = MoveDirection.None;
            rotateTarget = Vector3.zero;
        }

        if (moveDirection != m_moveDirection)
        {
            m_startRotationTime = Time.time;
            m_playerSourceRotation = m_playerRotation;
            m_playerTargetRotation = m_playerOriginalRotation + rotateTarget;
            m_moveDirection = moveDirection;
        }

        var rotationTime = Time.time - m_startRotationTime;
        m_playerRotation = Vector3.Lerp(m_playerSourceRotation, m_playerTargetRotation, rotationTime * LeanRotationScale);

        if (Input.GetButtonDown("Jump"))
        {
            KillPlayer();
        }
    }

    private float GetLimbModifier()
    {
        return (m_limbs.Count(x => x.IsAttched) + 2.0f) / m_limbs.Length;
    }

    private void HandleGravity()
    {
        var fallTime = Time.time - m_lastSpeedReset;
        var fallScalar = Mathf.Min(1.0f, fallTime / TimeToTerminalVelocity);
        var fallSpeed = Mathf.Lerp(StartFallSpeed, MaxFallSpeed - StartFallSpeed, fallScalar) * Time.deltaTime;
        m_playerPosition -= new Vector3(0.0f, fallSpeed, 0.0f);
    }
}
