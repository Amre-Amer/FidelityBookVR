using UnityEngine;

public class DisplaysMgr : MonoBehaviour
{
    public Globals g;

    void Awake()
    {
        LoadDisplays();    
    }

    void LoadDisplays()
    {
        g.displays.Clear();
        foreach(Transform t in transform)
        {
            g.displays.Add(t.GetComponent<DisplayMgr>());
        }
    }

    public void SetFps(int fps)
    {
        foreach(DisplayMgr display in g.displays)
        {
            display.SetFps(fps);
        }
    }

    public void SetProgress(float progress)
    {
        foreach(DisplayMgr display in g.displays)
        {
            display.SetProgress(progress);
        }
    }

    public void ShowProgressDone()
    {
        foreach(DisplayMgr display in g.displays)
        {
            display.ShowProgressDone();
        }
    }


}
