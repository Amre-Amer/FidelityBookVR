using UnityEngine;
using TMPro;

public class DisplayMgr : MonoBehaviour
{
    public TMP_Text textFps;
    public GameObject goProgress;

    public void SetFps(int fps)
    {
        textFps.text = "fps: " + fps;
    }

    public void SetProgress(float progress)
    {
        Vector3 sca = new Vector3(progress, 1, 1);
        goProgress.transform.localScale = sca;
    }

    public void ShowProgressDone()
    {
        SetColor(goProgress, Color.green);        
    }

    void SetColor(GameObject go, Color color)
    {
        go.GetComponentInChildren<Renderer>().material.color = color;
    }
}
