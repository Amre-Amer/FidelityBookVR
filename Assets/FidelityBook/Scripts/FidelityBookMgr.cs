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
            Debug.Log("<color=red>Not in editor or Hands not available</color>\n");
            return;
        }
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
        g.pagesMgr.SetNPageSelectedByDistFromTarget();
        g.pagesMgr.AdjustPages();
        switch (g.trainInfer)
        {
            case TrainInferType.train:
                g.trainMgr.UpdateTrain();
                g.scoresMgr.HighlightScoresForTraining();
                break;
            case TrainInferType.infer:
                float[] outputNormalized = g.inferMgr.Infer();
                g.pagesMgr.SetNPageSelectedForInference(outputNormalized);
                g.scoresMgr.HighlightScoresForInference(outputNormalized);
                break;
        }
    }
}
