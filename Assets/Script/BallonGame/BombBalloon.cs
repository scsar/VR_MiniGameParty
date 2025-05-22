using UnityEngine;

public class BombBalloon : Balloon
{
    public override void OnPop()
    {
        if (game != null)
        {
            game.FailGame(); // 실패 처리
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
