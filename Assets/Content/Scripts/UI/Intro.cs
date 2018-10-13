using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public void StartButton_OnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
