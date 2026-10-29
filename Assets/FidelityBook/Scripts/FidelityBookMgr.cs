using UnityEngine;

public class FidelityBookMgr : MonoBehaviour
{
    public HandsMgr handsMgr;
    public GameObject goTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor) return;
        goTarget.transform.position = handsMgr.posLeft;    
    }
}
