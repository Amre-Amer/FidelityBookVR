using UnityEngine;

public class FidelityBookMgr : MonoBehaviour
{
    public GlobalsMgr g;

    void Update()
    {
        UpdateAll();
    }

    void UpdateAll()
    {
        bool ynOk = Application.isEditor || g.handsMgr.SetPosLeftIfAvailable();
        if (!ynOk) {
            Debug.Log("<color=red>Not in editor or Hands not available</color>\n");
            return;
        }
        if (Application.isEditor) { // both train and infer
            g.targetMgr.UpdateTargetInEditor();
        } else
        {
            g.goTarget.transform.position = g.posLeftHand;    
        }
        switch (g.trainInfer)
        {
            case TrainInferType.train:
                g.pagesMgr.SetNPageSelectedByDistFromTargetForTraining();
                g.trainMgr.UpdateTrain();
                g.scoresMgr.HighlightScoresForTraining();
                break;
            case TrainInferType.infer:
                float[] outputNormalized = g.inferMgr.Infer();
                g.pagesMgr.SetNPageSelectedForInference(outputNormalized);
                g.scoresMgr.HighlightScoresForInference(outputNormalized);
                break;
        }
        g.pagesMgr.AdjustPages();
    }
}
