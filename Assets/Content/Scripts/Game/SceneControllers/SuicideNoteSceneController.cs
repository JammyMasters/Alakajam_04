using UnityEngine;
using UnityEngine.SceneManagement;

public class SuicideNoteSceneController : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Falling");
        }
    }
}
