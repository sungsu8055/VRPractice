using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TeleportStraight : MonoBehaviour
{
    public Transform teleportCircleUI;
    LineRenderer lr;
    // 초기 텔레포트 UI의 크기 설정
    Vector3 originScale = Vector3.one * 0.02f;

    // 워프 사용 여부
    public bool isWarp = false;
    // 워프에 걸리는 시간
    public float warpTime = 0.1f;
    // 포스트 프로세싱 컴포넌트
    public PostProcessVolume post;

    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
    }
    
    void Update()
    {
        if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true; // 라인 렌더러 컴포넌트 활성화
        }
        else if(ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false;
            if (teleportCircleUI.gameObject.activeSelf)
            {
                // 워프 기능 사용 하지 않을 때 순간이동
                if(isWarp == false)
                {
                    // transform을 사용하기 위해 CharacterController 비활성화
                    GetComponent<CharacterController>().enabled = false;
                    //텔레포트 UI 위치로 순간 이동
                    this.transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    StartCoroutine(Warp());
                }
                
            }
            teleportCircleUI.gameObject.SetActive(false);
        }
        // 왼쪽 컨트롤러의 one 버튼을 누르고 있으면
        else if(ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 왼쪽 컨트롤러 위치로 부터 향하고 있는 방향으로 Ray 생성
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo;
            // Terrain 이름이 있는 레이어가 나올 때까지 왼쪽으로 비트 이동
            int layer = 1 << LayerMask.NameToLayer("Terrain");
            // Terrain만 충돌 검출
            if(Physics.Raycast(ray, out hitInfo, 200, layer))
            {
                // Ray 충돌 지점에 라인 생성
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hitInfo.point);
                // 텔레포트 UI 활성화 후 레이가 부딪힌 지점에 텔레포트 UI 표시
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;
                // 텔레포트 UI의 방향과 hitInfo 지점의 방향 일치 시켜줌
                teleportCircleUI.forward = hitInfo.normal;
                // 텔레포트 UI의 크기가 거리에 따라 보정되도록 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            // ray 충돌이 발생하지 않으면 허공에 라인만 그려지도록 설정
            else
            {
                // ray 시작점 부터 200만큼의 거리를 가지는 지점까지 
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200);
                // 텔레포트 UI 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator Warp()
    {
        // 모션 블러
        MotionBlur blur;
        // 워프 시작점
        Vector3 pos = this.transform.position;
        // 목적지
        Vector3 targetPos = teleportCircleUI.position + Vector3.up;
        // 워프 경과 시간
        float currentTime = 0;
        // 포스트 프로세싱에서 사용 중인 프로필에서 모션 블러 얻어오기
        post.profile.TryGetSettings<MotionBlur>(out blur);
        // 워프 시작 전 블러 활성화
        blur.active = true;
        GetComponent<CharacterController>().enabled = false;

        // 경과 시간이 워프보다 짧은 시간 동안 이동 처리 진행
        while (currentTime < warpTime)
        {
            currentTime += Time.deltaTime; ;
            // 워프의 시작점에서 도착점에 도착하기 위해 워프 시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos, currentTime / warpTime);
            // 코루틴 대기
            yield return null;
        }

        transform.position = teleportCircleUI.position + Vector3.up;
        GetComponent<CharacterController>().enabled = true;
        blur.active = false;
    }
}
