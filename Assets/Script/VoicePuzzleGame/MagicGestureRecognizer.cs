using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public enum GestureType
{
    SwipeRight,
    SwipeLeft,
    SwipeUp,
    SwipeDown,
    None
}




public class MagicGestureRecognizer : MonoBehaviour
{

    public GameObject fireEffectPrefab;
    public GameObject waterEffectPrefab;
    public GameObject earthEffectPrefab;
    public GameObject windEffectPrefab;


    public InputActionReference triggerAction;

    public event Action OnGestureSuccess;
    public event Action OnGestureFail; 

    private bool isTracking = false;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private string word;

    public float minimumDistance = 0.2f;

    private GestureType expectedGesture = GestureType.None;

    private Dictionary<string, GestureType> magicGestureMap = new Dictionary<string, GestureType>
    {
        { "ignis", GestureType.SwipeRight },
        { "aqua", GestureType.SwipeUp },
        { "terra", GestureType.SwipeLeft },
        { "ventus", GestureType.SwipeDown }
    };

    private void OnEnable()
    {
        triggerAction.action.started += OnTriggerPressed;
        triggerAction.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        triggerAction.action.started -= OnTriggerPressed;
        triggerAction.action.canceled -= OnTriggerReleased;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        isTracking = true;
        startPosition = transform.position;
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (!isTracking) return;

        isTracking = false;
        endPosition = transform.position;

        Vector3 gestureVector = endPosition - startPosition;

        if (gestureVector.magnitude >= minimumDistance)
        {
            EvaluateGesture(gestureVector.normalized);
        }
        else
        {
            Debug.Log("Gesture too short.");
            OnGestureFail?.Invoke();
        }
    }

    private void EvaluateGesture(Vector3 direction)
    {
        GestureType detectedGesture = GestureType.None;

        if (Vector3.Dot(direction, Vector3.right) > 0.7f)
            detectedGesture = GestureType.SwipeRight;
        else if (Vector3.Dot(direction, Vector3.left) > 0.7f)
            detectedGesture = GestureType.SwipeLeft;
        else if (Vector3.Dot(direction, Vector3.up) > 0.7f)
            detectedGesture = GestureType.SwipeUp;
        else if (Vector3.Dot(direction, Vector3.down) > 0.7f)
            detectedGesture = GestureType.SwipeDown;

        if (detectedGesture == expectedGesture)
        {
            Debug.Log($"Gesture Success: {detectedGesture}");
            PlayMagicEffect(word); 
            OnGestureSuccess?.Invoke();
        }
        else
        {
            Debug.Log($"Gesture Failed. Expected: {expectedGesture}, Detected: {detectedGesture}");
            OnGestureFail?.Invoke(); 
        }
    }

    public void SetExpectedGesture(string magicWord)
    {
        magicWord = magicWord.ToLower();
        word = magicWord;

        if (magicGestureMap.ContainsKey(magicWord))
        {
            expectedGesture = magicGestureMap[magicWord];
            Debug.Log($"Set Expected Gesture for {magicWord}: {expectedGesture}");
        }
        else
        {
            expectedGesture = GestureType.None;
            Debug.LogWarning("Unknown magic word. No gesture expected.");
        }
    }



    private void PlayMagicEffect(string magicWord)
    {
        GameObject effectPrefab = null;

    switch (magicWord.ToLower())
    {
        case "ignis":
            effectPrefab = fireEffectPrefab;
            break;
        case "aqua":
            effectPrefab = waterEffectPrefab;
            break;
        case "terra":
            effectPrefab = earthEffectPrefab;
            break;
        case "ventus":
            effectPrefab = windEffectPrefab;
            break;
    }

    if (effectPrefab != null)
    {
        Instantiate(effectPrefab, new Vector3(-3, 30, 51), Quaternion.identity);
    }
    }
}
