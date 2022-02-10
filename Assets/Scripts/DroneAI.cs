using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    // 드론 상태 머신
    enum DroneState
    {
        idle,
        Move,
        Attack,
        Damage,
        Die
    }

    // 초기 상태 설정
    DroneState state = DroneState.idle;

    // Idle 변수
    // 대기상태 지속 시간
    public float idleDelayTime = 2;
    // 경과 시간
    float currentTime;

    // Move 변수
    // 이동 속도
    public float moveSpeed = 1f;
    // 타워 위치
    Transform towerPos;
    // 내비 메시 에이전트
    NavMeshAgent agent;

    // Attack 변수
    // 공격 범위 
    public float attackRange = 3;
    // 공격 지연 시간
    public float attackDelayTime = 2;

    // Damage 변수
    // private 속성이어도 에디터에 노출
    [SerializeField]
    // 체력
    int hp = 3;

    // Die 변수
    // 폭발 효과
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;

    void Start()
    {
        // 타워 객체 저장
        towerPos = GameObject.Find("Tower").transform;
        // NavMeshAgent 컴포넌트 저장
        agent = this.GetComponent<NavMeshAgent>();
        agent.enabled = false; // true 상태로 시작 시 내비를 찾지 못 할 수도 있음
        // agent 속도 지정, public 기능 사용 시 업데이트로 변경해주어야함
        agent.speed = moveSpeed;

        // Explosion 객체 초기화
        explosion = GameObject.Find("Explosion").transform;
        // ParticleSystem 컴포넌트 초기화
        expEffect = explosion.GetComponent<ParticleSystem>();
        // AudioSource 컴포넌트 초기화
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        // 현재 상태 문구 출력
        Debug.Log("Current state : " + state);

        // 상태 목차
        switch (state)
        {
            case DroneState.idle:
                Idle();
                break;
            case DroneState.Move:
                Move();
                break;
            case DroneState.Attack:
                Attack();
                break;
            case DroneState.Damage:
                // Damage();
                break;
            case DroneState.Die:
                // Die();
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
            // NavMeshAgent 정지 (수정 전 해당 if문 밖에 작성되어 Move 상태에서 바로 NavMesh 비활성되는 오류 발생)
            agent.enabled = false;
        }
    }
    void Attack()
    {
        // 공격 지연 경과 시간 누적
        currentTime += Time.deltaTime;
        // 경과 시간이 지연 시간 초과
        if(currentTime > attackDelayTime)
        {
            // 공격
            Tower.Instance.HP--;
            // 경과 시간 초기화
            currentTime = 0f;
        }
    }
    IEnumerator Damage()
    {
        // 길 찾기 중지
        agent.enabled = false ;
        // 자식 객체의 MeshRenderer material 저장
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        // 기존 색 저장
        Color originColor = mat.color;
        // material 색 red로 변경
        mat.color = Color.red;
        // 0.1 초 대기
        yield return new WaitForSeconds(0.1f);
        // material 색 원복
        mat.color = originColor;
        // state = Idle 전환
        state = DroneState.idle;
        // state 전환 경과 시간 초기화
        currentTime = 0f;
    }
    void Die()
    {

    }

    // 피격 상태 알림 함수
    public void OnDamageProcess()
    {
        // 체력 감소
        hp--;
        // HP가 남아 있다면
        if(hp > 0)
        {
            // 상태를 Damage로 전환
            state = DroneState.Damage;
            // 코루틴 함수 호출
            StopAllCoroutines();
            StartCoroutine(Damage());
        }
        else
        {
            // explosion 위치 값을 드론의 위치로 설정
            explosion.position = this.transform.position;
            // expEffect 재생
            expEffect.Play();
            // expAdio 재생
            expAudio.Play();
            // 드론 destroy
            Destroy(this.gameObject);
        }

    }
}
