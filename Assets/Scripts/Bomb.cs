using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 폭발 효과
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;
    // 폭발 반경
    public float expRange = 5;

    void Start()
    {
        // Explosion 객체 초기화
        explosion = GameObject.Find("Explosion").transform;
        // ParticleSystem 컴포넌트 초기화
        expEffect = explosion.GetComponent<ParticleSystem>();
        // AudioSource 컴포넌트 초기화
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // 레이어 마스크 저장
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        // 폭탄 위치 중심으로 폭발 반경 안에 들어온 드론 검사
        Collider[] drones = Physics.OverlapSphere(this.transform.position, expRange, layerMask);
        // 영역 안에 있는 모든 드론 제거
        foreach(Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        // 폭발 효과 위치 지정
        explosion.position = this.transform.position;
        // 이펙트 재생
        expEffect.Play();
        // 사운드 재생
        expAudio.Play();
        // 폭탄 삭제
        Destroy(this.gameObject);
    }
}
