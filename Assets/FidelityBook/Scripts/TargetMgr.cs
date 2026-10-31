using UnityEngine;

public class TargetMgr : MonoBehaviour
{
    public GlobalsMgr g;
    float distMove;
    float dirMove;

    void Start()
    {
        distMove = .01f;
        dirMove = 1;
        
    }

    public void UpdateTargetInEditor()
    {
        MoveTarget();
        float z = g.goTarget.transform.position.z;
        if (dirMove == 1 && z > g.goRight.transform.position.z)
        {
            dirMove  = -1;
        }
        if (dirMove == -1 && z < g.goLeft.transform.position.z)
        {
            dirMove  = 1;
        }
    }

    void MoveTarget()
    {
        Vector3 pos = g.goTarget.transform.position;
        pos.z += distMove * dirMove;
        g.goTarget.transform.position = pos;
    }
}
