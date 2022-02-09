using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    // 드론 상태 머신
    enum DroneState
    {
        Idel,
        Move,
        Attack,
        Damage,
        Die
    }

    // 초기 상태 설정
    DroneState state = DroneState.Idel;

    // Idle 변수
    // 대기상태 지속 시간
    public float idleDelayTime = 2;
    // 경과 시간
    float currentTime;

    // Move 변수
    // 이동 속도
    float moveSpeed = 1f;
    // 타워 위치
    Transform towerPos;
    // 내비 메시 에이전트
    NavMeshAgent agent;

    // Attack 변수
    // 공격 범위 
    public float attackRange = 3;
    // 공격 지연 시간
    public float attackDelayTime = 2;

    void Start()
    {
        // 타워 객체 저장
        towerPos = GameObject.Find("Tower").transform;
        // NavMeshAgent 컴포넌트 저장
        agent = this.GetComponent<NavMeshAgent>();
        agent.enabled = false; // true 상태로 시작 시 내비를 찾지 못 할 수도 있음
        // agent 속도 지정
        agent.speed = moveSpeed;
    }

    void Update()
    {
        // 현재 상태 문구 출력
        Debug.Log("Current state : " + state);

        // 상태 목차
        switch (state)
        {
            case DroneState.Idel:
                Idle();
                break;
            case DroneState.Move:
                Move();
                break;
            case DroneState.Attack:
                Attack();
                break;
            case DroneState.Damage:
                Damage();
                break;
            case DroneState.Die:
                Die();
                break;
        }
    }

    // 일정 시간 대기 후 공격 상태로 전환
    void Idle()
    {
        // 경과 시간 누적
        currentTime += Time.deltaTime;
        // 경과 시간의 대기 시간 초과 
        if (currentTime > idleDelayTime)
        {
            // 상태를 이동으로 전환
            state = DroneState.Move;
            // agent 활성화
            agent.enabled = true;
        }
    }
    // 타워를 향해 이동
    void Move()
    {
        // 내비게이션 목적지 타워로 지정
        agent.SetDestination(towerPos.position);
        // 공격 범위 진입 시 공격 상태 전환
        if(Vector3.Distance(this.transform.position, towerPos.position) < attackRange)
        {
            state = DroneState.Attack;
        }
        // NavMeshAgent 정지
        agent.enabled = false;
    }
    void Attack()
    {
        // 공격 지연 경과 시간 누적
        currentTime += Time.deltaTime;
        // 경과 시간이 지연 시간 초과
        if(currentTime > attackDelayTime)
        {
            // 공격

            // 경과 시간 초기화
            currentTime = 0f;
        }
    }
    void Damage()
    {

    }
    void Die()
    {

    }
}
