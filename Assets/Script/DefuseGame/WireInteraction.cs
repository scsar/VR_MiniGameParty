using UnityEngine;

public class WireInteraction : MonoBehaviour
{
    public string wireColor;
    private DefuseGame game;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game = FindFirstObjectByType<DefuseGame>();
    }

    public void OnCut()
    {
        game.Cutwire(wireColor);
        Destroy(gameObject);

    }

}
