using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // 물체를 잡았는지 여부
    bool isGrabbing = false;
    // 잡고 있는 물
    GameObject grabbedObject;
    // 잡을 물체 종류
    public LayerMask grabbedLayer;
    // 잡을 수 있는 거리
    public float grabRange = 0.2f;

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
    }

    void TryGrab()
    {
        // Grab 버튼을 누르면
        if(ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
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
                if(nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
        }
    }
}
