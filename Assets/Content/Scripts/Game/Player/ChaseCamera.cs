using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public Rigidbody FollowTarget;

    /// <summary>
    /// 
    /// </summary>
    public Vector3 CameraOffsetMin;

    /// <summary>
    /// 
    /// </summary>
    public Vector3 CameraOffsetMax;

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        var speed = FollowTarget.velocity.magnitude;
        var offset = Vector3.Lerp(CameraOffsetMin, CameraOffsetMax, speed * 0.02f);
        transform.position = FollowTarget.transform.position + offset;
    }
}
