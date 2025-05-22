using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject mainSceneObjects; // MainScene UI + XR Origin 그룹

    public int maxLives = 3;
    private int currentLives;
    public int _currentLives
    {
        get 
        {
            return currentLives;
        }
    }
    private int currentLevel = 1;

    public string[] minigameSceneNames;
    private string currentMiniGameScene;

    private bool isPlayingMiniGame = false;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentLives = maxLives;
    }

    public void StartMiniGame()
    {
        if (mainSceneObjects != null)
            mainSceneObjects.SetActive(false);

        currentLives = maxLives;
        currentLevel = 1;

        LoadNextMiniGame();
    }

    void LoadNextMiniGame()
    {
        if (!string.IsNullOrEmpty(currentMiniGameScene))
        {
            SceneManager.UnloadSceneAsync(currentMiniGameScene);
        }

        currentMiniGameScene = GetRandomMiniGameScene();

        SceneManager.LoadSceneAsync(currentMiniGameScene, LoadSceneMode.Additive).completed += (op) =>
        {
            isPlayingMiniGame = true;

            var miniGame = FindMiniGameInLoadedScene(currentMiniGameScene);
            if (miniGame != null)
            {
                miniGame.OnSuccess += HandleSuccess;
                miniGame.OnFail += HandleFail;

                float baseTime = 30f;
                float timeReductionPerLevel = 2f;
                float timeLimit = Mathf.Max(10f, baseTime - (currentLevel - 1) * timeReductionPerLevel);

                miniGame.StartGame(timeLimit, currentLevel);
            }
        };
    }

    string GetRandomMiniGameScene()
    {
        if (minigameSceneNames == null || minigameSceneNames.Length == 0)
        {
            Debug.LogError("미니게임 씬 리스트가 비어있습니다!");
            return null;
        }

        int index = Random.Range(0, minigameSceneNames.Length);
        return minigameSceneNames[index];
    }

    IMiniGame FindMiniGameInLoadedScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            IMiniGame mg = root.GetComponentInChildren<IMiniGame>();
            if (mg != null)
                return mg;
        }
        return null;
    }

    void HandleSuccess()
    {
        Debug.Log("미니게임 성공!");
        currentLevel++;
        LoadNextMiniGame();
    }

    void HandleFail()
    {
        Debug.Log("미니게임 실패!");
        currentLives--;

        if (currentLives <= 0)
        {
            Debug.Log("모든 목숨 소진. 메인 씬으로 복귀.");
            EndGameAndReturnToMain();
        }
        else
        {
            LoadNextMiniGame();
        }
    }

    void EndGameAndReturnToMain()
    {
        if (!string.IsNullOrEmpty(currentMiniGameScene))
        {
            SceneManager.UnloadSceneAsync(currentMiniGameScene);
            currentMiniGameScene = null;
        }

        if (mainSceneObjects != null)
            mainSceneObjects.SetActive(true);

        currentLives = maxLives;
        currentLevel = 1;
        isPlayingMiniGame = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

}
