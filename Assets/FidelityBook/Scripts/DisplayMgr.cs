using UnityEngine;
using TMPro;

public class DisplayMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public TMP_Text textFps;
    public GameObject goProgress;
    public GameObject body;
    public TMP_Text textCount;

    public void SetFps(int fps)
    {
        textFps.text = "fps: " + fps;
    }

    public void SetCount(string txt)
    {
        textCount.text = txt;
    }

    public void SetProgress(float progress)
    {
        Vector3 sca = new Vector3(progress, 1, 1);
        goProgress.transform.localScale = sca;
    }

    public void ShowProgressDone()
    {
        g.toolsMgr.SetColor(body, Color.green);        
    }
}
