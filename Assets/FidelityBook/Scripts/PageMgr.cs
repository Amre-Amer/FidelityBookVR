using UnityEngine;

public class PageMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public GameObject panel;
    public GameObject panelOther;

    public void TurnLeft()
    {
        Turn(-1 * g.angDeltaPage);
    }

    public void TurnRight()
    {
        Turn(g.angDeltaPage);
    }

    void Turn(float ang)
    {
        Vector3 eul = transform.localEulerAngles;
        eul.x = ang;
        transform.localEulerAngles = eul;
    }

    public void Highlight(bool ynOnLeft, bool ynOnRight)
    {
        panel.GetComponent<Renderer>().enabled = ynOnLeft;
        panelOther.GetComponent<Renderer>().enabled = ynOnRight;
    }
}
