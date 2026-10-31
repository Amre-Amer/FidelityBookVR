using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine;

public class Globals : MonoBehaviour
{
    public TrainInferType trainInfer = TrainInferType.infer;
    public BackendType backend = BackendType.GPUCompute; // Use GPUCompute or CPU
    
    public FidelityBookMgr mgr;
    public TextureConverter textureConverter;
    public RenderTexture renderTexture;
    public PagesMgr pagesMgr;
    public HandsMgr handsMgr;
    public AICameraMgr aICameraMgr;
    public DisplaysMgr displaysMgr;
    public List<DisplayMgr> displays;
    public GameObject goTarget;
    public ModelAsset modelAsset;
    public int height;
    public int width;
    public int channels;
    public int cntMaxTraining;
    public string pathFolder;
    public List<PageMgr> pages;
    public GameObject markerPrefab;
    public GameObject goLeft;
    public GameObject goRight;
    public int nPageSelected;

    void Awake()
    {
        // 6144 floats 
        height = 32;
        width = 64;
        channels = 3;
        cntMaxTraining = 1000;
        pathFolder = "C:\\Users\\Amre\\FidelityBook\\Assets\\FidelityBook\\Python\\image_data";
    }
}
public enum TrainInferType
{
    train,
    infer        
}

