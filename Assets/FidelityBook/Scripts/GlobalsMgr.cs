using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif

public class GlobalsMgr : MonoBehaviour
{
    [Header("Neural Network")]
    public TrainInferType trainInfer = TrainInferType.infer;
    public BackendType backend = BackendType.GPUCompute; // Use GPUCompute or CPU
    public ModelAsset modelAsset;
    public int height;
    public int width;
    public int channels;
    public int cntMaxTraining;
    public string pathFolder;
    [Header("Parameters")]
    public bool ynSlowTarget;
    public int nPageSelected;
    public float distMoveTarget;
    public float dirMoveTarget;
    public float angDeltaPage;
    public float scoreScale;  // ?
    [Header("Classes")]
    public FpsMgr fpsMgr;
    public RenderTexture renderTexture;
    public PagesMgr pagesMgr;
    public HandsMgr handsMgr;
    public AICameraMgr aICameraMgr;
    public DisplaysMgr displaysMgr;
    public TrainMgr trainMgr;
    public InferMgr inferMgr;
    public ToolsMgr toolsMgr;
    public TargetMgr targetMgr;
    public ScoresMgr scoresMgr;
    public GameObject goTarget;
    public GameObject goLeft;
    public GameObject goRight;
    public Vector3 posLeftHand;
    public Vector3 posRightRight;
    public List<DisplayMgr> displays;
    public List<PageMgr> pages;

    void Awake()
    {
        // if (!Application.isPlaying)
        // {
        //     // myEditorValue = "Updated in Awake!";
        //     // 6144 floats 

        //     #if UNITY_EDITOR
        //     // Mark the object as dirty so Unity saves the change to the scene/prefab
        //     EditorUtility.SetDirty(this);
        //     #endif
        // }
        // 6144 floats 
        height = 32;
        width = 64;
        channels = 1; //3
        cntMaxTraining = 1000;
        pathFolder = "C:\\Users\\Amre\\FidelityBook\\Assets\\FidelityBook\\Python\\image_data";
        distMoveTarget = .05f;
        dirMoveTarget = 1;
        angDeltaPage = 15;    
        ynSlowTarget = true;
        scoreScale = 2;        
    }
}
public enum TrainInferType
{
    train,
    infer        
}

