using UnityEngine;
using System;

public interface IMiniGame
{
    event Action OnSuccess;
    event Action OnFail;
    void StartGame(float duration, int currentLevel); // 제한 시간

    string GetMiniGameDescription(); // 미니게임 설명

}