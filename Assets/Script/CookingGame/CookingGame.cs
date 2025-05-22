using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class CookingGame : MonoBehaviour, IMiniGame {
    public event Action OnSuccess;
    public event Action OnFail;

    public GameObject[] ingredientPrefabs;
    public Transform spawnArea;

    private List<string> recipeOrder = new List<string>();
    private int currentIngredientIndex = 0;
    private float currentTime;
    private bool isRunning = false;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI recipeText;
    public TextMeshProUGUI LifeText;


    public Animation PotAni;
    public AudioClip successSound;
    public AudioClip failSound;



    public void StartGame(float timeLimit, int level) {
        currentTime = timeLimit;
        isRunning = true;
        LifeText.text = "남은 목숨 : " + GameManager.Instance._currentLives;


        int ingredientCount = 2 + level; // 레벨에 따라 레시피 길이 증가
        GenerateRecipe(ingredientCount);
    }

    public string GetMiniGameDescription()
    {
        return "알맞은 재료를 넣어라!";
    }

    void Update() {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"남은시간 : {Mathf.Ceil(currentTime).ToString()}";

        if (currentTime <= 0f) {
            isRunning = false;
            OnFail?.Invoke();
        }
    }

    void GenerateRecipe(int count) {
        recipeOrder.Clear();
        recipeText.text = "";

        for (int i = 0; i < count; i++) {
            int index = UnityEngine.Random.Range(0, ingredientPrefabs.Length);
            string ingredientName = ingredientPrefabs[index].name;
            recipeOrder.Add(ingredientName);

            recipeText.text += $"{ingredientName}\n";
            Vector3 spawnPos = spawnArea.position + new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                0f,
                UnityEngine.Random.Range(-1f, 1f)
            );
            Instantiate(ingredientPrefabs[index], spawnPos, Quaternion.identity, transform);
        }

        currentIngredientIndex = 0;
    }

    public void SubmitIngredient(string ingredientName) {
        if (!isRunning) return;

        PotAni.Play();
        if (recipeOrder[currentIngredientIndex] == ingredientName) {
            GetComponent<AudioSource>().PlayOneShot(successSound);
            currentIngredientIndex++;
            if (currentIngredientIndex >= recipeOrder.Count) {
                isRunning = false;
                OnSuccess?.Invoke();
            }
        } else {
            GetComponent<AudioSource>().PlayOneShot(failSound);
            isRunning = false;
            OnFail?.Invoke();
        }
    }
}
