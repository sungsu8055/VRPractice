using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // 총알 이펙트
    public Transform bulletImpact;  // 파편 효과 
    ParticleSystem bulletEffect;    // 파편 파티클 시스템 
    AudioSource bulletAudio;    // 총알 발사 사운드 

    // 크로스헤어 오브젝트
    public Transform crosshair;

    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    void Update()
    {
        // 크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // 컨트롤러 진동 재생
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);

            // 총 발사 사운드 재생
            bulletAudio.Stop();
            bulletAudio.Play();

            // 카메라 위치로 부터 레이 생성
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            RaycastHit hitInfo;
            // 플레이어 레이어 받아오기
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            // 타워 레이어 받아오기
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;

            // Ray 발사 후 충돌 정보는 hitInfo에 저장
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                bulletEffect.Stop();
                bulletEffect.Play();
                // 충돌 지점에 이펙트 위치 설정
                bulletImpact.position = hitInfo.point;
                // 충돌 지점 노말 방향으로 이펙트 방향 설정
                bulletImpact.forward = hitInfo.normal;
            }
        }
    }
}