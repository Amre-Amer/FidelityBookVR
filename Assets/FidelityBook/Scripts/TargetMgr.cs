using UnityEngine;

public class TargetMgr : MonoBehaviour
{
    public GlobalsMgr g;

    public void MoveTargetInEditor()
    {        
        MoveTarget();
        SetDirMoveTarget();
    }

    void SetDirMoveTarget()
    {
        float z = g.goTarget.transform.position.z;
        if (g.dirMoveTarget == 1 && z > g.goRight.transform.position.z)
        {
            g.dirMoveTarget  = -1;
        }
        if (g.dirMoveTarget == -1 && z < g.goLeft.transform.position.z)
        {
            g.dirMoveTarget  = 1;
        }
    }

    void MoveTarget()
    {
        Vector3 pos = g.goTarget.transform.position;
        pos.z += g.distMoveTarget * g.dirMoveTarget;
        g.goTarget.transform.position = pos;
    }
}
