using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



public class SafeCrackMiniGame : MonoBehaviour, IMiniGame
{
    public event Action OnSuccess;
    public event Action OnFail;

    private IXRSelectInteractor interactor;
    private XRGrabInteractable grabInteractable;

    private List<float> targetAngles = new List<float>();
    private List<int> targetDirections = new List<int>();
    private int currentStep = 0;

    private float allowedRange = 5f;
    private float maxDistance = 90f;
    private float currentTime;
    private bool isPlaying = false;

    private float previousAngle = 0f;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI directionText;
    public TextMeshProUGUI LifeText;


    public void StartGame(float duration, int currentLevel)
    {
        Debug.Log("SafeCrackMiniGame 시작!");
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        LifeText.text = "남은 목숨 : " + GameManager.Instance._currentLives;

        SetupNewLock(currentLevel);

        currentTime = duration;
        isPlaying = true;
    }

    public string GetMiniGameDescription()
    {
        return "다이얼을돌려 위치를 맞춰라!";
    }

    void Update()
    {
        if (!isPlaying) return;

        if (timerText != null)
        {
            timerText.text = $"남은시간 : {Mathf.Ceil(currentTime).ToString()}";
        }

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            Fail();
            return;
        }

        float currentAngle = transform.localEulerAngles.y;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngles[currentStep]));

        SendHapticFeedback(angleDifference);

        float angleDelta = Mathf.DeltaAngle(previousAngle, currentAngle);
        previousAngle = currentAngle;

        int expectedDirection = targetDirections[currentStep];
        int actualDirection = angleDelta > 0 ? 1 : (angleDelta < 0 ? -1 : 0);

        if (directionText != null)
        {
            directionText.text = (expectedDirection == 1) ? "오른쪽으로! →" : "왼쪽으로! ←";
        }

        if (angleDifference <= allowedRange && actualDirection == expectedDirection)
        {
            currentStep++;
            if (currentStep >= targetAngles.Count)
            {
                GetComponent<AudioSource>().Play();
                Success();
            }
            else
            {
                Debug.Log($"단계 {currentStep} 성공! 다음 목표로...");
            }
        }
    }

    private void SendHapticFeedback(float angleDifference)
    {
        if (interactor is XRBaseInputInteractor controllerInteractor)
        {
            float hapticStrength = Mathf.Clamp01(1f - (angleDifference / maxDistance));
            controllerInteractor.SendHapticImpulse(hapticStrength, 0.05f);
        }
    }

    private void SetupNewLock(int level)
    {
        targetAngles.Clear();
        targetDirections.Clear();
        currentStep = 0;

        int steps = Mathf.Clamp(level, 1, 5);

        for (int i = 0; i < steps; i++)
        {
            targetAngles.Add(UnityEngine.Random.Range(0f, 360f));

            int direction = (i % 2 == 0) ? 1 : -1;
            targetDirections.Add(direction);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        previousAngle = transform.localEulerAngles.y;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    private void Success()
    {
        Debug.Log("금고 해제 성공!");
        isPlaying = false;
        OnSuccess?.Invoke();
    }

    private void Fail()
    {
        Debug.Log("금고 해제 실패...");
        isPlaying = false;
        OnFail?.Invoke();
    }
}
