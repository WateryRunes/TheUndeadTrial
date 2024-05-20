using UnityEngine;

public class FaderScript : MonoBehaviour
{
    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;
    private float alpha = 1.0f;
    private int drawDepth = -1000;
    private int fadeDir = -1;

    void OnGUI()
    {
        //Debug.Log("ongui");
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        Rect dimension = new Rect(0,0, Screen.width, Screen.height);
        GUI.DrawTexture(dimension, fadeOutTexture);
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return 1.0f / fadeSpeed;
    }

    void OnLevelWasLoaded(int level)
    {
        BeginFade(-1);
    }
}
