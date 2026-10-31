using System.Collections.Generic;
using UnityEngine;

public class ScoresMgr : MonoBehaviour
{
    public GlobalsMgr g;
    public List<ScoreMgr>scores = new();

    void Start()
    {
       LoadScores();
    }

    public void HighlightScoresForTraining()
    {
        for(int n = 0; n < scores.Count; n++)
        {
            ScoreMgr score = scores[n];
            Color color = Color.black;
            if (n == g.nPageSelected) color = Color.green;
            score.SetColor(color);
        }
    }

    public void HighlightScoresForInference(float[] outputNormalized)
    {
        for(int n = 0; n < scores.Count; n++)
        {
            ScoreMgr score = scores[n];
            float value = outputNormalized[n];
            score.SetValue(value);
            Color color = Color.white * value;
            score.SetColor(color);
        }
    }

    void LoadScores()
    {
        scores.Clear();
        foreach(Transform t in transform)
        {
            scores.Add(t.GetComponent<ScoreMgr>());
        }
    }
}
