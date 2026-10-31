using UnityEngine;

public class FidelityBookMgr : MonoBehaviour
{
    public Globals g;
    int cntFps;

    void Start()
    {
        InvokeRepeating(nameof(Fps), 1, 1);
    }

    void Fps()
    {
        g.displaysMgr.SetFps(cntFps);
        cntFps = 0;
    }
}

