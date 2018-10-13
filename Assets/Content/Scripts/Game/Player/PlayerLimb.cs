using UnityEngine;

public class PlayerLimb : MonoBehaviour
{
    public PlayerBloodParticles Particles;
    public bool IsAttched { get; private set; }

    private Rigidbody m_rigidbody;
    private SpringJoint[] m_joints;

    public void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_joints = GetComponents<SpringJoint>();
        IsAttched = true;
        EnableBloodParticles(false);
    }

    public void OnJointBreak(float breakForce)
    {
        Detatch();
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
