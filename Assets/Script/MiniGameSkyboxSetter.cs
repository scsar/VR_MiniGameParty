using UnityEngine;

public class MiniGameSkyboxSetter : MonoBehaviour
{
    public Material skyboxMaterial;

    void Start()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}
