using UnityEngine;

public class ToolsMgr : MonoBehaviour
{
    public Texture2D RenderTextureToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = previousActive;        
        return tex;
    }

    public void SetColor(GameObject go, Color color)
    {
        Renderer rend = go.GetComponent<Renderer>();
        rend.material.color = color;
    }
}
