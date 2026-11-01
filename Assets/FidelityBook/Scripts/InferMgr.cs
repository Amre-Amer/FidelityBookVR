using UnityEngine;
using Unity.InferenceEngine;

public class InferMgr : MonoBehaviour
{
    public GlobalsMgr g;
    private Model runtimeModel;
    private Worker worker;
    TensorShape inputShape;

    void Start()
    {
        inputShape = new TensorShape(1, g.channels, g.height, g.width);        
        runtimeModel = ModelLoader.Load(g.modelAsset);
        worker = new Worker(runtimeModel, g.backend);
    }

    public float[] Infer(){
        Texture2D inputTexture = g.toolsMgr.RenderTextureToTexture2D(g.renderTexture);
        Color[] colors = inputTexture.GetPixels(); 
        // float[] rawInputData = ColorsToInputData(colors);   
        // Debug.Log("colors: " + colors.Length + "\n");     
            TensorShape shape = new TensorShape(1, g.channels, g.height, g.width);

            // Tensor<float> inputTensor = new Tensor<float>(shape);
            // TextureConverter.ToTensor(inputTexture, inputTensor);
            // // using Tensor<float> inputTensor = new Tensor<float>(inputShape, rawInputData);
            // // Tensor<float> inputTensor = new Tensor<float>(inputShape, rawInputData);
            // worker.Schedule(inputTensor);
            // Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
            // float[] output = outputTensor.DownloadToArray();
        // Debug.Log($"Inference complete! Output shape: {outputTensor.shape}");
        // Debug.Log($"First output value: {outputTensor[0]}");

        // TensorShape shape = new TensorShape(1, 1, 32, 64);

        // TextureTransform settings = new TextureTransform();
        //     settings.SetTensorLayout(TensorLayout.NCHW);
            
            // .SetChannelBits(ColorTensorChannels.R); // Extract only the Red/Grey channel
        Tensor<float> inputTensor = new Tensor<float>(shape);
        // TextureConverter.ToTensor(inputTexture, inputTensor);

        TextureConverter.ToTensor(inputTexture, inputTensor);
        // using Tensor<float> inputTensor = TextureConverter.ToTensor(inputTexture, shape, settings);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
        worker.Schedule(inputTensor);
        Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
        // outputTensor.MakeReadable(); 
        float[] output = outputTensor.DownloadToArray();



        inputTensor.Dispose();

    //    inputTensor.Dispose();
 

        float[] outputNormalized = Normalize(output);
        // SetNPageSelectedBForference(outputNormalized);
        ShowOutputDebug(outputNormalized);
        return outputNormalized;
    }

    

    // public void SetNPageSelectedByInference(float [] outputNormalized)
    // {
    //     g.nPageSelected = -1;
    //     float max = -1;
    //     for(int n = 0; n < outputNormalized.Length; n++)
    //     {
    //         float value = outputNormalized[n];
    //         if (value > max)
    //         {
    //             g.nPageSelected = n;
    //             max = value;
    //         }
    //     }  
    // }

    void ShowOutputDebug(float [] outputNormalized){      
        string txtAll = "";
        for(int n = 0; n < outputNormalized.Length; n++)
        {
            float value = outputNormalized[n];
            int nn = (int)(value * 50f) + 1;
            string txt = value.ToString("F1") + "\t " + new string('|', nn);
            if (n == g.nPageSelected) {
                txt = "<color=green>" + txt + "</color>";
            }
            txtAll += "\n" + txt;
        }        
//        Debug.Log("g.nPageSelected: " + g.nPageSelected + txtAll + "\n");
    }

    // float[]ColorsToInputData(Color[] colors)
    // {
    //     float[] rawInputData = new float[g.height * g.width * g.channels];        
    //     for (int n = 0; n < colors.Length; n++)
    //     {
    //         Color color = colors[n];
    //         int nn = n * g.channels;
    //         rawInputData[nn] = color.r;
    //         rawInputData[nn + 1] = color.g;
    //         rawInputData[nn + 2] = color.b;
    //     }
    //     return rawInputData;
    // }

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
