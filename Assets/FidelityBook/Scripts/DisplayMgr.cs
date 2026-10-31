using UnityEngine;
using TMPro;

public class DisplayMgr : MonoBehaviour
{
    public GlobalsMgr g;
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
        g.toolsMgr.SetColor(goProgress, Color.green);        
    }
}
