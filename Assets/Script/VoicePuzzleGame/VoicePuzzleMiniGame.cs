using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

public class VoicePuzzleMiniGame : MonoBehaviour, IMiniGame
{
    public event Action OnSuccess;
    public event Action OnFail;

    public TextMeshProUGUI puzzleText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI LifeText;


    public MagicGestureRecognizer gestureRecognizer; // 인스펙터 연결용 public

    private List<string> magicWords = new List<string> { "ignis", "aqua", "terra", "ventus" };

    private string targetWord;
    private KeywordRecognizer keywordRecognizer;
    private float currentTime;
    private bool isPlaying = false;
    private bool awaitingGesture = false;

    public void StartGame(float duration, int currentLevel)
    {
        Debug.Log("VoicePuzzleMiniGame 시작!");

        LifeText.text = "남은 목숨 : " + GameManager.Instance._currentLives;

        targetWord = SelectMagicWord();

        if (puzzleText != null)
        {
            puzzleText.text = $"Chant the magic word: {targetWord.ToUpper()}!"; 
        }

        SetupRecognizer();

        if (gestureRecognizer == null)
        {
            Debug.LogError("GestureRecognizer가 연결되지 않았습니다!");
        }

        currentTime = duration;
        isPlaying = true;
    }

    public string GetMiniGameDescription()
    {
        return "올바른 주문과 동작을 취해라!";
    }

    private string SelectMagicWord()
    {
        int index = UnityEngine.Random.Range(0, magicWords.Count);
        return magicWords[index];
    }

    private void SetupRecognizer()
    {
        keywordRecognizer = new KeywordRecognizer(new string[] { targetWord });
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text.Equals(targetWord, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"마법 단어 인식 성공: {args.text}");
            awaitingGesture = true;

            if (gestureRecognizer != null)
            {
                gestureRecognizer.SetExpectedGesture(targetWord);
                gestureRecognizer.OnGestureSuccess += OnGestureSuccess;
                gestureRecognizer.OnGestureFail += OnGestureFail;
            }

            if (puzzleText != null)
            {
                puzzleText.text = $"동작을 수행해주세요!";
            }
        }
    }

    private void OnGestureSuccess()
    {
        Debug.Log("제스처까지 성공!");
        Cleanup();
        OnSuccess?.Invoke();
    }

    private void OnGestureFail()
    {
        Debug.Log("제스처 실패!");
        Cleanup();
        OnFail?.Invoke();
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (!awaitingGesture)
        {
            currentTime -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"남은시간 : {Mathf.Ceil(currentTime).ToString()}";
            }

            if (currentTime <= 0)
            {
                Fail();
            }
        }
    }

    private void Fail()
    {
        Debug.Log("시간 초과로 실패!");
        Cleanup();
        OnFail?.Invoke();
    }

    private void Cleanup()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }

        if (gestureRecognizer != null)
        {
            gestureRecognizer.OnGestureSuccess -= OnGestureSuccess;
            gestureRecognizer.OnGestureFail -= OnGestureFail;
        }

        isPlaying = false;
        awaitingGesture = false;
    }
}
