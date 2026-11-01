using UnityEngine;

public class FpsMgr : MonoBehaviour
{
    public GlobalsMgr g;
    int cntFps;

    void Start()
    {
        InvokeRepeating(nameof(Fps), 1, 1);
    }

    void Update()
    {
        cntFps++;
    }

    void Fps()
    {
        g.displaysMgr.SetFps(cntFps);
        cntFps = 0;
    }
}

