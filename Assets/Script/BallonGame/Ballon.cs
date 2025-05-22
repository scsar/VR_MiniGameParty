using UnityEngine;

public class Balloon : MonoBehaviour
{
    protected BalloonGame game;


    
    public float shakeAmount = 0.5f;  // 흔들리는 정도
    public float shakeSpeed = 2f;     // 흔들리는 속도

    [HideInInspector]
    public Vector3 initialPosition;  // 초기 위치 저장



    protected virtual void Start()
    {
        game = FindFirstObjectByType<BalloonGame>();
        initialPosition = transform.position;

    }

    public virtual void OnPop()
    {
        if (game != null)
        {
            game.PopBalloon();
            GetComponent<AudioSource>().Play();
        }
        Destroy(gameObject);
    }

    void Update()
    {
        // Sin 함수를 사용하여 y축으로 흔들리게 만듬
        float newY = initialPosition.y + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        
        // 새로운 위치로 오브젝트 이동
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
