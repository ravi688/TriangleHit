using UnityEngine;

[ExecuteInEditMode]
public class VignetteFX : MonoBehaviour
{
    public Shader CurShader;
    [Range(0, 1)]
    public float VignetteAmount = 1.0f;

    Material curMaterial;

    Material material
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(CurShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (!CurShader || !CurShader.isSupported)
        {
            enabled = false;
            return;
        }
        material.SetInt("_ScreenWidth", Screen.width);
        material.SetInt("_ScreenHeight", Screen.height);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CurShader != null)
        {
            material.SetFloat("_VignetteAmount", VignetteAmount);

            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void OnDisable()
    {
        if (curMaterial)
            DestroyImmediate(curMaterial);
    }
}
