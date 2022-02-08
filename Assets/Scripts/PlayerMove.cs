using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도
    public float speed = 5;
    // CharacterController  컴포넌트
    CharacterController cc;

    // 중력 적용
    public float gravity = -20; // 중력 가속도 크기 변수
    float yVelocity = 0; // 수직 속도

    // 점프 크기
    public float jumpPower = 5;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 입력값을 받아 전후좌우로 이동
        // 입력값을 받는다
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        // 이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);

        // 플레이어가 바라보는 방향으로 이동 입력값 변화
        dir = Camera.main.transform.TransformDirection(dir);

        // 중력을 적용한 수직 방향 이동 추가
        yVelocity += gravity * Time.deltaTime;

        // 바닥에 있을 경우 수직 항력을 적용
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }
        // 점프 버튼을 누르면 속도에 점프 크기 할당
        if(ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        dir.y = yVelocity;

        // cc Move 함수로 이동 설정
        cc.Move(dir * speed * Time.deltaTime);
    }
}
