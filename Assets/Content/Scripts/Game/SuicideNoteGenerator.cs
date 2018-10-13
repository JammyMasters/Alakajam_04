using UnityEngine;
using UnityEngine.UI;

public class SuicideNoteGenerator : MonoBehaviour, IGameStateObserver
{
    public Text Text;

    public void OnEnterState(GameState state)
    {
        if (state != GameState.SUICIDE_NOTE)
        {
            return;
        }

        Text.text = "TEST";
    }

    public void OnLeaveState(GameState state)
    {
    }
}
