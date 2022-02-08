using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    // 텔레포트 UI
    public Transform teleportCircleUI;
    // 선을 그릴 라인 렌더러
    LineRenderer lr;
    // 텔레포트 UI 기본 크기
    Vector3 originScale = Vector3.one * 0.02f;
    // 커브의 부드러운 정도
    public int lineSmooth = 40;
    // 커브의 길이
    public float curveLength = 50f;
    // 커브 중력
    public float gravity = -60f;
    // 곡선 시뮬레이션 간격, 시간
    public float simulateTime = 0.02f;
    // 곡선을 이루는 점 저장 리스트
    List<Vector3> lines = new List<Vector3>();

    void Start()
    {
        // 시작 시 비활성화
        teleportCircleUI.gameObject.SetActive(false);
        // 라인 렌더러 컴포넌트 얻어오기
        lr = GetComponent<LineRenderer>();
        // 라인 너비 설정
        lr.startWidth = 0.0f;
        lr.endWidth = 0.2f;
    }

    void Update()
    {
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 활성
            lr.enabled = true;
        }
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 비활성
            lr.enabled = false;
            // 텔레포트 UI 활성화 시
            if (teleportCircleUI.gameObject.activeSelf)
            {
                // CC 비활성
                GetComponent<CharacterController>().enabled = false;
                // 텔레포트 UI 위치로 순간이동
                this.transform.position = teleportCircleUI.position + Vector3.up;
                // cc 활성
                GetComponent<CharacterController>().enabled = true;
            }
            // 텔레포트 UI 비활성
            teleportCircleUI.gameObject.SetActive(false);
        }
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 주어진 길이의 커브 생성
            MakeLines();
        }
    }

    void MakeLines()
    {
        // 리스트에 담긴 위치 정보들을 비워준다
        lines.RemoveRange(0, lines.Count);
        // 라인 진행 방향
        Vector3 dir = ARAVRInput.LHandDirection * curveLength;
        // 라인이 그려질 위치의 초기값 설정
        Vector3 pos = ARAVRInput.LHandPosition;
        // 최초 위치를 리스트에 담음
        lines.Add(pos);

        // lineSmooth 개수만큼 아래 명령 반복
        for(int i = 0; i < lineSmooth; i++)
        {
            // 현재 위치 기억
            Vector3 lastPos = pos;
            // 중력을 적용한 속도 계산
            // v(미래속도) = v0(현재속도) + a(가속도)t(시간)
            dir.y += gravity * simulateTime;
            // 등속 운동으로 다음 위치 계산
            // P(미래위치) = P0(현재위치) + v(속도)t(시간)
            pos += dir * simulateTime;

            if (CheckHitRay(lastPos, ref pos))
            {
                lines.Add(pos);
                break;
            }
            else
            {
                teleportCircleUI.gameObject.SetActive(false);
            }

            // 구한 위치 등록
            lines.Add(pos);
        }
        // 라인 렌더러가 표현할 점의 개수를 등록 개수로 할당
        lr.positionCount = lines.Count;
        // 라인 렌더러에 구해진 점의 정보 지정
        lr.SetPositions(lines.ToArray());
    }

    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)
    {
        Vector3 rayDir = pos - lastPos; // lastPos 부터 pos 를 향하는 방향
        Ray ray = new Ray(lastPos, rayDir); // lastPos 부터 pos를 향하는 방향으로 레이 생성
        RaycastHit hitInfo; // ray 히트 정보를 담을 변수 

        // Raycast 시 레이의 크기를 lastPos ~ pos 사이로 제한
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            // 다음 점의 위치를 충돌 지점으로 설정
            pos = hitInfo.point;

            int layer = LayerMask.NameToLayer("Terrain");
            // Terrain 와 충돌했을 경우 텔레포트 ui출력
            if (hitInfo.transform.gameObject.layer == layer)
            {
                teleportCircleUI.gameObject.SetActive(true);
                // 텔레포트 UI 위치 설정 
                teleportCircleUI.position = pos;
                // 텔레포트 UI 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                // 텔레포트 UI가 보일 크기 설정
                float distance = (pos - ARAVRInput.LHandPosition).magnitude; // 왼쪽 컨트롤러 지점 부터 충돌 지점까지 거리 계산
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);
            }

            return true;
        }
        return false;
    }
}
