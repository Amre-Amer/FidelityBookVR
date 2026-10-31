using UnityEngine;

public class TextureConverter : MonoBehaviour
{
    public Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        // 1. Create a new Texture2D matching the RenderTexture's dimensions
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        
        // 2. Remember the currently active RenderTexture, then set your source as active
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = rTex;
        
        // 3. Read the pixels from the active RenderTexture into the Texture2D
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        
        // 4. Apply changes to upload/finalize the pixel data on the CPU
        tex.Apply();
        
        // 5. Restore the previous active RenderTexture to avoid breaking other rendering pipelines
        RenderTexture.active = previousActive;
        
        return tex;
    }
}
