using UnityEngine;

public class PageMgr : MonoBehaviour
{
    float angDelta = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
