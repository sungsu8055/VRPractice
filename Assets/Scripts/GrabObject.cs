using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // 물체를 잡았는지 여부
    bool isGrabbing = false;
    // 잡고 있는 물체
    GameObject grabbedObject;
    // 잡을 물체 종류
    public LayerMask grabbedLayer;
    // 잡을 수 있는 거리
    public float grabRange = 0.2f;

    // 물체 던지기
    // 이전 위치
    Vector3 prevRHandPos;
    // 던지는 힘
    float throwPower = 10;
    // 각속도 적용
    // 이전 회전
    Quaternion prevRHandRot;
    // 회전력
    public float torPower = 5;

    // 원거리 잡기
    // 원거리 물체 잡기 기능 활성화 여부
    public bool isRemoteGrab = true;
    // 원거리 물체 잡을 수 있는 거리
    public float remoteGrabDistance = 20;

    void Start()
    {

    }

    void Update()
    {
        // 잡고 있지 않는 상태일 때 잡기 시도
        if (isGrabbing == false)
        {
            TryGrab();
        }
        else
        {
            TryUngrab();
        }
    }

    void TryGrab()
    {
        // Grab 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 원거리 물체 잡기 기능을 사용 시
            if (isRemoteGrab)
            {
                // 오른손 컨트롤러가 향하는 방향으로 레이 생성
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;
                // Sphere cast 이용 물체 충돌 체크
                if(Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
                {
                    // 잡은 상태로 전환
                    isGrabbing = true;
                    // 잡은 물체 저장
                    grabbedObject = hitInfo.transform.gameObject;
                    // 물체 끌어오기
                    StartCoroutine(GrabbingAnimation());
                }
                return;
            }

            // 일정 영역 안에 폭탄이 있을 때
            Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);
            // 폭탄을 잡는다
            int closest = 0;
            // 손과 가장 가까운 물체 선택
            for (int i = 1; i < hitObjects.Length; i++)
            {
                // 가장 가까운 물체와 손의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
                // 다음 물체와 손의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
                // 다음 물체와의 거리가 더 가까우면 인덱스 교체
                if (nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
            // 검출 물체 있으면
            if (hitObjects.Length > 0)
            {
                // 그랩 상태 트루로 변경
                isGrabbing = true;
                // 가장 가까운 잡을 물체 저장
                grabbedObject = hitObjects[closest].gameObject;
                // 잡은 물체를 손의 자식으로 등록
                grabbedObject.transform.parent = ARAVRInput.RHand;
                // 물리 기능 정지
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

                // 던지기 이전 위치 초기값 지정
                prevRHandPos = ARAVRInput.RHandPosition;
                // 던지기 이전 회전 초기값 지정
                prevRHandRot = ARAVRInput.RHand.rotation;
            }
        }
    }

    void TryUngrab()
    {
        // 던져질 방향
        Vector3 throwDirection = (ARAVRInput.RHandPosition - prevRHandPos);
        // 위치값 갱신
        prevRHandPos = ARAVRInput.RHandPosition;
        // 쿼터니온 차 구하기 공식
        // angle1 = Q1, angle2= Q2
        // angle1 + angle2 = Q1 * Q2
        // -angle2 = Quaternion.Inverse(Q2)
        // angle2 - angle1 = Quaternion.FromToRotation(Q1, Q2) = Q2 * Quaternion.Inverse(Q1)
        // 회전 방향 = current - prev, -prev는 Inverse로 구함
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRHandRot);
        // 이전 회전
        prevRHandRot = ARAVRInput.RHand.rotation;

        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 잡기 상태 false로 전환
            isGrabbing = false;
            // 물리 기능 활성화
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            // 손에서 폭탄 떼어내기, 부모 자식 관계 비활성
            grabbedObject.transform.parent = null;
            // 던지기
            grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;
            // 각속도 = (1/dt) * d0(특정 축 기준 변위 각도)
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;
            // 잡은 물체 정보 삭제
            grabbedObject = null;
        }
    }

    IEnumerator GrabbingAnimation()
    {
        // 물리 기능 정지
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        // 초기 위치 값 지정
        prevRHandPos = ARAVRInput.RHandPosition;
        // 초기 회전 값 지정
        prevRHandRot = ARAVRInput.RHand.rotation;
        // 원거리 물체 출발 도착 위치
        Vector3 startLocation = grabbedObject.transform.position;
        Vector3 targetLocation = ARAVRInput.RHandPosition + ARAVRInput.RHandDirection * 0.1f;

        float currentTime = 0;
        float finishTime = 0.2f;
        // 경과율
        float elapsedRate = currentTime / finishTime;
        while(elapsedRate < 1)
        {
            currentTime += Time.deltaTime;
            elapsedRate = currentTime / finishTime;
            // 출발, 도착 위치 간의 벡터 값을 보간하고 원거리 물체의 위치 값에 적용
            grabbedObject.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);
            yield return null;
        }
        // 잡은 물체를 손 오브젝트로 이동 후 자식으로 등록(잡기)
        grabbedObject.transform.position = targetLocation;
        grabbedObject.transform.parent = ARAVRInput.RHand;
    }
}
