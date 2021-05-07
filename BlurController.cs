using UnityEngine;
public class BlurController : MonoBehaviour
{
    public int iterations = 3;                   // blur iterations - larger number means more blur.
    public float blurSpread = 0.6f;              // blur spread for each iteration. Lower values give better looking blur.
    static Material m_Material = null;
    protected Material material { get { if (m_Material == null) { m_Material = new Material(blurShader) { hideFlags = HideFlags.DontSave }; } return m_Material; } }

    public Shader blurShader = null;             // the blur iteration shader just takes 4 texture samples and averages them.
                                     

 
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * blurSpread;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }   // performs one blur iteration.

  
    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        Graphics.BlitMultiTap(source, dest, material,
                               new Vector2(-off, -off),
                               new Vector2(-off, off),
                               new Vector2(off, off),
                               new Vector2(off, -off));
    }  // downsamples the texture to a quarter resolution.

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        DownSample4x(source, buffer);  // copy source to the 4x4 smaller texture.


        for (int i = 0; i < iterations; i++)
        {
            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            FourTapCone(buffer, buffer2, i);
            RenderTexture.ReleaseTemporary(buffer);
            buffer = buffer2;
        }     // blur the small texture
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }    // called by the camera to apply the image effect
}