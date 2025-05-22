using UnityEngine;

public class Ingredient : MonoBehaviour
{
    private CookingGame game;
    public string ingredientName;

    void Start()
    {
        game = FindFirstObjectByType<CookingGame>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pot")) // 냄비에 닿았을 때만
        {
            SubmitIngredient();
        }
    }

    private void SubmitIngredient()
    {
        if (game != null)
        {
            game.SubmitIngredient(ingredientName);
        }

        Destroy(gameObject); // 제출 후 재료 제거
    }
}
