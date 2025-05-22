using UnityEngine;

public class SirenLight : MonoBehaviour
{
    private Light sirenLight;
    private bool isFlashing = false;

    public float minIntensity = 0f;   // 최소 강도 
    public float maxIntensity = 100f;  // 최대 강도 (100)
    public float cycleTime = 1f;      // 사이클 시간 (증가+감소 시간)
    private float timer = 0f;          // 타이머


    void Start()
    {
        sirenLight = GetComponent<Light>();
        sirenLight.color = Color.red;  // 빨간색으로 설정
        sirenLight.intensity = minIntensity;  // 시작 강도는 50
        StartFlashing();
    }

    void Update()
    {
        if (isFlashing)
        {
            timer += Time.deltaTime;

            // 0과 1 사이에서 사이클이 돌아가도록 계산
            float cycleProgress = Mathf.PingPong(timer / cycleTime, 1f);

            // 강도를 Lerp를 사용해 부드럽게 변화시킴
            sirenLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, cycleProgress);
        }
    }

    // 사이렌 깜빡이기 시작
    public void StartFlashing()
    {
        isFlashing = true;  // 사이렌 깜빡임 시작
    }

    // 사이렌 깜빡이기 멈추기
    public void StopFlashing()
    {
        isFlashing = false;
        sirenLight.intensity = minIntensity;  // 깜빡임 멈추고 강도 초기화
    }
}
