using UnityEngine;

public class ToolsMgr : MonoBehaviour
{
    public GlobalsMgr g;
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

    public void ErrorMessage(string txtError)
    {
        g.audioSource.PlayOneShot(g.clipError);
        Debug.Log("<color=red>" + txtError + "</color>\n");
    }
}
