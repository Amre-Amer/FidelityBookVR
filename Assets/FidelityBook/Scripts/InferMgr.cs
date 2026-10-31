using UnityEngine;
using Unity.InferenceEngine;

public class InferMgr : MonoBehaviour
{
    public Globals g;
    private Model runtimeModel;
    private Worker worker;
    TensorShape inputShape;

    void Start()
    {
        inputShape = new TensorShape(1, g.channels, g.height, g.width);        
        runtimeModel = ModelLoader.Load(g.modelAsset);
        worker = new Worker(runtimeModel, g.backend);
        if (g.trainInfer == TrainInferType.infer)
        {
            InvokeRepeating(nameof(Infer), 1, 1);
        }

    }

    public float[] Infer(){
        Texture2D inputTexture = g.textureConverter.RenderTextureToTexture2D(g.renderTexture);
        Color[] colors = inputTexture.GetPixels(); 
        float[] rawInputData = ColorsToInputData(colors);        
        using Tensor<float> inputTensor = new Tensor<float>(inputShape, rawInputData);
        worker.Schedule(inputTensor);
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
        float[] output = outputTensor.DownloadToArray();
        Debug.Log($"Inference complete! Output shape: {outputTensor.shape}");
        // Debug.Log($"First output value: {outputTensor[0]}");
        inputTensor.Dispose();
        float[] outputNormalized = Normalize(output);
        ShowOutput(outputNormalized);
        return outputNormalized;
    }

    void ShowOutput(float [] outputNormalized)
    {
        int nMax = -1;
        float max = -1;
        for(int n = 0; n < outputNormalized.Length; n++)
        {
            float value = outputNormalized[n];
            if (value > max)
            {
                nMax = n;
                max = value;
            }
        }        
        string txtAll = "";
        for(int n = 0; n < outputNormalized.Length; n++)
        {
            float value = outputNormalized[n];
            int g = (int)(value * 50f) + 1;
            string txt = new string('|', g);
            if (n == nMax) {
                txt = "<color=green>" + txt + "</color>";
            }
            txtAll += "\n" + txt;
        }        
        Debug.Log("nClass: " + nMax + txtAll + "\n");
    }

    float[]ColorsToInputData(Color[] colors)
    {
        Debug.Log("colors length: " + colors.Length);
        float[] rawInputData = new float[g.height * g.width * g.channels];        
        Debug.Log("rawInputData length: " + rawInputData.Length);
        for (int n = 0; n < colors.Length; n++)
        {
            Color color = colors[n];
            int nn = n * g.channels;
            rawInputData[nn] = color.r;
            rawInputData[nn + 1] = color.g;
            rawInputData[nn + 2] = color.b;
        }
        return rawInputData;
    }

    float[] Normalize(float[] array)
    {
        float[] normalized = new float[array.Length];
        float min = -1;
        float max = -1;
        for(int n = 0; n < array.Length; n++)
        {
            float value = array[n];
            if (value < min || n == 0)
            {
                min = value;
            }
            if (value > max || n == 0)
            {
                max = value;
            }
        }
        float range = max - min;
        for(int n = 0; n < array.Length; n++)
        {
            float value = array[n];
            float norm = (value - min) / range;
            normalized[n] = norm;
        }
        return normalized;
    }

    private void OnDestroy()
    {
        worker?.Dispose();
    }
}
