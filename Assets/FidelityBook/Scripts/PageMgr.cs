using UnityEngine;

public class PageMgr : MonoBehaviour
{
    float angDelta = 15;

    public void TurnLeft()
    {
        Turn(-1 * angDelta);
    }

    public void TurnRight()
    {
        Turn(angDelta);
    }

    void Turn(float ang)
    {
        Vector3 eul = transform.localEulerAngles;
        eul.x = ang;
        transform.localEulerAngles = eul;
    }
}
