using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    public void StartButton_OnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
