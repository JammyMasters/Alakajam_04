using UnityEngine;

public class KillVolume : MonoBehaviour
{
    public GameStateMachine GameStateMachine;

    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.KillPlayer();
            GameStateMachine.Transition(GameState.NewspaperFlash);
        }
    }
}
