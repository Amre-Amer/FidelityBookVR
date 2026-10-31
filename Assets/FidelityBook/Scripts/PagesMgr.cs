using UnityEngine;

public class PagesMgr : MonoBehaviour
{
    public GlobalsMgr g;

    void Awake()
    {
        LoadPages();
    }

    void LoadPages()
    {
        g.pages.Clear();
        foreach(Transform t in transform)
        {
            g.pages.Add(t.GetComponent<PageMgr>());
        }    
    }

    public void AdjustPages()
    {
        for(int n = 0; n < g.pages.Count; n++)
        {
            PageMgr page = g.pages[n];            
            if (n <= g.nPageSelected){
                page.TurnLeft();
            } else
            {
                page.TurnRight();
            }
        }
    }

    public void SetNPageSelectedForInference(float [] outputNormalized)
    {
        g.nPageSelected = -1;
        float max = -1;
        for(int n = 0; n < outputNormalized.Length; n++)
        {
            float value = outputNormalized[n];
            if (value > max || n == 0)
            {
                g.nPageSelected = n;
                max = value;
            }
        }  
    }

    public void SetNPageSelectedByDistFromTargetForTraining()
    {
        float distMin = -1;
        g.nPageSelected = -1;
        for(int n = 0; n < g.scoresMgr.scores.Count; n++) // amre
        {
            ScoreMgr score = g.scoresMgr.scores[n];
            float dist = Vector3.Distance(g.goTarget.transform.position, score.transform.position);
            if (dist < distMin || n == 0)
            {
                distMin = dist;
                g.nPageSelected = n;
            }
        }    
        if (g.nPageSelected == -1)
        {
            Debug.Log("<color=red>g.nPageSelected == -1</color>\n");
        }
    }
}
