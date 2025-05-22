using UnityEngine;
using TMPro;
using System;

public class BalloonGame : MonoBehaviour, IMiniGame
{
    public event Action OnSuccess;
    public event Action OnFail;

    public GameObject balloonPrefab;
    public GameObject bombBalloonPrefab; // ✨ 추가
    public Transform spawnArea;

    public int baseBalloonCount = 5; // 기본 풍선 수
    private int successBalloonCount = 0; // 성공해야 하는 정상 풍선 수
    private int poppedSuccessCount = 0; // 현재 터뜨린 정상 풍선 수
    private float currentTime;
    private bool isRunning = false;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI balloonsLeftText;
    public TextMeshProUGUI LifeText;


    public void StartGame(float timeLimit, int level)
    {
        currentTime = timeLimit;
        isRunning = true;

        LifeText.text = "남은 목숨 : " + GameManager.Instance._currentLives;

        poppedSuccessCount = 0;

        float bombChance = Mathf.Clamp(0.1f + (level * 0.05f), 0.1f, 0.5f); 
        // 레벨 올라갈수록 bomb 확률 증가 (최대 50%)

        int normalBalloonCount = baseBalloonCount + (level * 2); 
        int bombBalloonCount = Mathf.RoundToInt(normalBalloonCount * bombChance);

        successBalloonCount = normalBalloonCount;

        SpawnBalloons(normalBalloonCount, bombBalloonCount);

        UpdateBalloonCountText();
    }

    public string GetMiniGameDescription()
    {
        return "빨간 풍선만 터트려라!";
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"남은시간 : {Mathf.Ceil(currentTime).ToString()}";

        if (currentTime <= 0f)
        {
            FailGame();
        }
    }

    void SpawnBalloons(int normalCount, int bombCount)
    {
        int totalCount = normalCount + bombCount;

        for (int i = 0; i < totalCount; i++)
        {
            Vector3 randomPos = spawnArea.position + new Vector3(
                UnityEngine.Random.Range(-4f, 4f),
                UnityEngine.Random.Range(0f, 4f),
                UnityEngine.Random.Range(-4f, 4f)
            );

            GameObject balloonObj;
            if (i < bombCount)
            {
                balloonObj = Instantiate(bombBalloonPrefab, randomPos, Quaternion.identity, transform);
            }
            else
            {
                balloonObj = Instantiate(balloonPrefab, randomPos, Quaternion.identity, transform);
            }
        }
    }

    public void PopBalloon()
    {
        poppedSuccessCount++;
        UpdateBalloonCountText();

        if (poppedSuccessCount >= successBalloonCount)
        {
            SuccessGame();
        }
    }

    public void SuccessGame()
    {
        if (isRunning)
        {
            isRunning = false;
            OnSuccess?.Invoke();
        }
    }

    public void FailGame()
    {
        if (isRunning)
        {
            isRunning = false;
            OnFail?.Invoke();
        }
    }

    private void UpdateBalloonCountText()
    {
        if (balloonsLeftText != null)
        {
            int remaining = successBalloonCount - poppedSuccessCount;
            balloonsLeftText.text = $"남은 풍선수: {remaining}";
        }
    }
}
