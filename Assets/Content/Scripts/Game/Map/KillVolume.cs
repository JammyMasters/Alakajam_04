using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.KillPlayer();
            enabled = false;
        }
    }
}
