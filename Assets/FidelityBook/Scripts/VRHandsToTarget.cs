using UnityEngine;

public class VRHandsToTarget : MonoBehaviour
{
    public Globals g;
    void Update()
    {
        if (Application.isEditor == false) 
        {
            g.goTarget.transform.position = g.handsMgr.posLeft;    
        }
    }
}

