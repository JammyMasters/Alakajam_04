using UnityEngine;

public class PasserBy : MonoBehaviour
{
    public float DirectionChangeInterval = 1;

    public float Speed = 1;

    private Vector3 m_direction;

    private float m_directionChangeElapsedTime;

    public void Start()
    {
        DefineNewDirection();
    }

    public void Update()
    {
        m_directionChangeElapsedTime += Time.deltaTime;
        if (m_directionChangeElapsedTime >= DirectionChangeInterval)
        {
            DefineNewDirection();
            m_directionChangeElapsedTime = 0;
        }
        transform.Translate(m_direction * Time.deltaTime * Speed);
    }

    private void DefineNewDirection()
    {
        var angle = Random.Range(0, 360);
        var transform = Matrix4x4.Rotate(Quaternion.Euler(0, angle, 0));
        m_direction = transform.MultiplyVector(Vector3.forward);
    }
}
