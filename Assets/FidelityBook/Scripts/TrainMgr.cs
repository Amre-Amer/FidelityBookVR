using System.IO;
using UnityEngine;

public class TrainMgr : MonoBehaviour
{
    public Globals g;   
    int cntSave;
    int numPages;
    float progress;

    void Start()
    {
        numPages = g.pages.Count - 1;
        ResetFolder(g.pathFolder);
        AddClassFolders();
    }

    void Update()
    {
        if (g.trainInfer == TrainInferType.infer) return;
        UpdateTrain();
    }

    void AddClassFolders()
    {
        for(int n = 0; n < numPages; n++)
        {
            string folder = "class_" + n;
            string folderSpec = Path.Combine(g.pathFolder, folder);
            ResetFolder(folderSpec);
        }
    }

    void UpdateTrain()
    {
        if (cntSave == g.cntMaxTraining)
        {
            ShowProgressDone();
            Debug.Log("Done\n");
        }
        if (cntSave > g.cntMaxTraining) return;
        Texture2D tex = g.textureConverter.RenderTextureToTexture2D(g.renderTexture);
        byte[] bytes = tex.EncodeToPNG();
        string pathClass = "class_" + g.nPageSelected;
        string filename = "image_" + cntSave + "_" + g.nPageSelected + ".png"; 
        string filespec = Path.Combine(g.pathFolder, pathClass, filename);
        File.WriteAllBytes(filespec, bytes);
        ShowProgress();
        // Debug.Log(filespec + "\n");
        cntSave++;
    }

    void ShowProgress()
    {
        progress = cntSave / (float) g.cntMaxTraining;
        g.displaysMgr.SetProgress(progress);
    }

    void ShowProgressDone()
    {
        g.displaysMgr.ShowProgressDone();
    }

    public void ResetFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true); 
        }
        Directory.CreateDirectory(folderPath);
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}
