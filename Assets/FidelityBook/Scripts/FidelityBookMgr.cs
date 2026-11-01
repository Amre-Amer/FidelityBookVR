using UnityEngine;

public class FidelityBookMgr : MonoBehaviour
{
    public GlobalsMgr g;
    float delay = 1;
    float elapsed;
    float timeLast;

    void Start()
    {
        elapsed = 1;
        if (g.ynSlowTarget)
        {
            timeLast = Time.realtimeSinceStartup;
        }
    }

    void Update()
    {
        UpdateAll();
    }

    void UpdateAll()
    {
        bool ynOk = Application.isEditor || g.handsMgr.SetPosLeftIfAvailable();
        if (ynOk == false) {
            g.toolsMgr.ErrorMessage("Not in editor or Hands not available");
            return;
        }
        MoveTarget();
        g.pagesMgr.SetNPageSelectedByDistFromTarget();
        g.pagesMgr.AdjustPages();
        switch (g.trainInfer)
        {
            case TrainInferType.train:
                g.trainMgr.SaveImageForTraining();
                g.scoresMgr.HighlightScoresForTraining();
                break;
            case TrainInferType.infer:
                float[] outputNormalized = g.inferMgr.Infer();
                g.pagesMgr.SetNPageSelectedForInference(outputNormalized);
                g.scoresMgr.HighlightScoresForInference(outputNormalized);
                break;
        }
    }

    void MoveTarget()
    {
        if (Application.isEditor) { 
            elapsed = Time.realtimeSinceStartup - timeLast;
            if (g.trainInfer == TrainInferType.train || g.ynSlowTarget == false || elapsed > delay)
            {
                timeLast = Time.realtimeSinceStartup;
                g.targetMgr.MoveTargetInEditor();
            }
        }
        else
        {
            g.goTarget.transform.position = g.posLeftHand;    
        }
    }
}
