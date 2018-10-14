using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFade : MonoBehaviour
{
    public float FadeSpeed;
    public Texture2D FadeOutTex;

    private int m_drawDepth = -1000;
    private float m_alpha = 1.0f;
    private float m_fadeDir = -1.0f;

    private void OnGUI()
    {
        m_alpha += m_fadeDir * FadeSpeed * Time.deltaTime;
        m_alpha = Mathf.Clamp01(m_alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, m_alpha);
        GUI.depth = m_drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeOutTex);
    }

    public float BeginFade(float dir)
    {
        m_fadeDir = dir;
        return FadeSpeed;
    }

    private void OnLevelWasLoaded()
    {
        BeginFade(-1.0f);
    }
}
