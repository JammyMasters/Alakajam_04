using UnityEngine;
using UnityEngine.SceneManagement;

public class NewspaperFlashSceneController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("SuicideNote");
        }
    }
}
