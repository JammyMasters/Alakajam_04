using UnityEngine;

public class PlayerLimb : MonoBehaviour
{
    public PlayerBloodParticles Particles;
    public bool IsAttched { get; private set; }

    private Rigidbody m_rigidbody;
    private SpringJoint[] m_joints;
    private PlayerController m_playerController;

    public void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_joints = GetComponents<SpringJoint>();
        m_playerController = GetComponentInParent<PlayerController>();
        IsAttched = true;
        EnableBloodParticles(false);
    }

    public void OnJointBreak(float breakForce)
    {
        Detatch();
        m_playerController.StartTimeSlowDown(1.0f);
    }

    public void Detatch()
    {
        foreach (var joint in m_joints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        const float killForce = 4.0f;
        m_rigidbody.AddForce(
            new Vector3(
                Random.Range(-killForce, killForce),
                Random.Range(-killForce, killForce),
                Random.Range(-killForce, killForce)),
            ForceMode.Impulse);

        EnableBloodParticles(true);
        IsAttched = false;
    }

    public void EnableBloodParticles(bool enable)
    {
        if (Particles != null)
        {
            Particles.gameObject.SetActive(enable);
        }
    }
}
