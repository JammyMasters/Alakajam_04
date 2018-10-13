using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool m_isActive;
    private Vector3 m_playerPosition;
    private float m_lastSpeedReset;
    private SpringJoint[] m_limbJoints;

    public float HorizontalSpeed = 1.0f;
    public float StartFallSpeed = 2.0f;
    public float MaxFallSpeed = 10.0f;
    public float TimeToTerminalVelocity = 2.0f;
    
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
    }

    public void Activate(Vector3 startPosition)
    {
        m_isActive = true;
        m_playerPosition = startPosition;
        m_lastSpeedReset = Time.time;
        transform.localPosition = m_playerPosition;
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
        var moveHorizontal = Input.GetAxis("Horizontal") * HorizontalSpeed;
        m_playerPosition += new Vector3(0.0f, 0.0f, moveHorizontal * Time.deltaTime);

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
