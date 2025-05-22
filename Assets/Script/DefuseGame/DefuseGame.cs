using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class DefuseGame : MonoBehaviour, IMiniGame
{
    public event Action OnSuccess;
    public event Action OnFail;

    public GameObject[] wirePrefabs;
    // wire 색상 이름 목록 추가
    private readonly string[] wireColors = { "Red", "Blue", "Green", "Yellow", "Purple", "Orange" };

    public TMP_Text timerText;
    public TMP_Text hintText;
    public TextMeshProUGUI LifeText;


    private float currentTime;
    private bool isPlaying = false;

    private string correctWire;

    public GameObject FailEffect;



    public void StartGame(float duration, int currentLevel)
    {
        Debug.Log("DefuseGame Start!");
        LifeText.text = "남은 목숨 : " + GameManager.Instance._currentLives;


        GenerateWire(currentLevel);

        currentTime = duration;
        isPlaying = true;
    }

    public string GetMiniGameDescription()
    {
        return "올바른 전선을 잘라라!";
    }

    private void Update()
    {
        if (!isPlaying) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"남은시간 : {Mathf.Ceil(currentTime).ToString()}";

        if (currentTime <= 0f)
        {
            Fail();
        }
    }

private void GenerateWire(int wireCount)
{
    foreach (Transform child in transform)
    {
        if (child.CompareTag("Wire"))
            Destroy(child.gameObject);
    }

    List<string> colorPool = new List<string>();

    List<(GameObject prefab, string colorName)> wireData = new List<(GameObject, string)>();
    for (int i = 0; i < wirePrefabs.Length && i < wireColors.Length; i++)
    {
        wireData.Add((wirePrefabs[i], wireColors[i]));
    }

    ShuffleList(wireData); 

    float spacing = 0.3f;
    Vector3 startPos = transform.position - new Vector3((wireCount - 1) * spacing * 0.5f, 0f, 0f);

    List<string> shuffledColorNames = new List<string>();

    for (int i = 0; i < wireCount; i++)
    {
        string color = $"Wire_{i}";
        GameObject wire = Instantiate(wireData[i].prefab, transform);
        wire.name = color;

        WireInteraction wi = wire.GetComponent<WireInteraction>();
        wi.wireColor = color;
        colorPool.Add(color);

        wire.transform.position = startPos + new Vector3(0f, 0f, i * spacing);

        shuffledColorNames.Add(wireData[i].colorName); 
    }

    int correctIndex = UnityEngine.Random.Range(0, wireCount);
    correctWire = colorPool[correctIndex];

    if (hintText != null)
    {
        if (correctIndex < shuffledColorNames.Count)
        {
            hintText.text = $"{shuffledColorNames[correctIndex]} 전선을 자르세요!";
        }
        else
        {
            hintText.text = $"Cut the Wire!";
        }
    }
}


private void ShuffleList<T>(List<T> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        int randomIndex = UnityEngine.Random.Range(i, list.Count);
        T temp = list[i];
        list[i] = list[randomIndex];
        list[randomIndex] = temp;
    }
}


    public void Cutwire(string selectedWire)
    {
        if (!isPlaying) return;

        if (selectedWire == correctWire)
        {
            Success();
        }
        else
        {
            Fail();
        }
    }

    private void Success()
    {
        Debug.Log("DefuseGame Success!");
        isPlaying = false;
        OnSuccess?.Invoke();
    }

    private void Fail()
    {
        Instantiate(FailEffect, transform.position, transform.rotation);
        Debug.Log("DefuseGame Fail...");
        isPlaying = false;
        OnFail?.Invoke();
    }
}
