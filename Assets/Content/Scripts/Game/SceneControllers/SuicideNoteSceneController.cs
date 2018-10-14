using UnityEngine;
using UnityEngine.SceneManagement;

public class SuicideNoteSceneController : MonoBehaviour
{
    private bool m_loadingLevel = false;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!m_loadingLevel)
            {
                m_loadingLevel = true;
                ChangeLevel();
            }
        }
    }

    System.Collections.IEnumerator ChangeLevel()
    {
        float fadeTime = gameObject.GetComponent<SceneFade>().BeginFade(1.0f);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("Falling");
    }
}
