using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public PlayerController FollowTarget;

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
        var speed = 0.0f;
        var offset = Vector3.Lerp(CameraOffsetMin, CameraOffsetMax, speed);
        transform.position = FollowTarget.transform.position + offset;
    }
}
