using System.IO;
using UnityEngine;

public class TrainMgr : MonoBehaviour
{
    public GlobalsMgr g;   
    int cntSave;
    int numPages;
    float progress;

    void Start()
    {
        numPages = g.pages.Count - 1;
        ResetFolder(g.pathFolder);
        AddClassFolders();
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

    public void UpdateTrain()
    {
        if (cntSave == g.cntMaxTraining)
        {
            ShowProgressDone();
            Debug.Log("Done\n");
        }
        if (cntSave > g.cntMaxTraining) return;
        Texture2D tex = g.toolsMgr.RenderTextureToTexture2D(g.renderTexture);
        byte[] bytes = tex.EncodeToPNG();
        string pathClass = "class_" + g.nPageSelected;
        string filename = "image_" + cntSave + "_" + g.nPageSelected + ".png"; 
        string filespec = Path.Combine(g.pathFolder, pathClass, filename);
        File.WriteAllBytes(filespec, bytes);
        ShowProgress();
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
