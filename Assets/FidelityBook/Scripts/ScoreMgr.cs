using UnityEngine;

public class ScoreMgr : MonoBehaviour
{
    public GlobalsMgr g;
    
    public void SetValue(float value)
    {
        transform.localScale = Vector3.one * value * g.scoreScale;        
    }

    public void SetColor(Color color)
    {
        g.toolsMgr.SetColorInChildren(gameObject, color);      
    }
}
