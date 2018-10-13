using UnityEngine;
using UnityEngine.SceneManagement;

public class FallingSceneController : MonoBehaviour
{
    public PlayerController PlayerController;

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return) && PlayerController.IsDead)
        {
            SceneManager.LoadScene("NewspaperFlash");
        }
    }
}
