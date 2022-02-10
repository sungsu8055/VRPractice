using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // 데미지 표현 UI
    public Transform damageUI;
    public Image damageImage;

    // 타워 최초 HP 변수
    public int initialHP = 10;
    // 내부 HP 변수
    int _hp = 0;
    // _hp 의 get/set 프로퍼티
    public int HP
    {
        get 
        { 
            return _hp; 
        }
        set
        {
            _hp = value;

            // 실행 중인 코루틴 정지
            StopAllCoroutines();
            // 피격 표시를 표현할 코루틴 실행
            StartCoroutine(DamageEvent());

            // hp가 0 이하 이면 제거
            if(_hp <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Tower 싱글턴 객체
    public static Tower Instance;

    private void Awake()
    {
        // 싱글턴 객체 값 할당
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // 카메라 near 값 조절용 임시 변수
    public float cameraNearOffset;

    void Start()
    {
        // 최초 기본 HP값으로 초기화
        _hp = initialHP;
        // 카메라의 nearClipPlane 값을 저장 (0.01 값 설정 시 카메라 near 보다 뒤에 배치됨, 0.25로 설정 하면 해결)
        float z = Camera.main.nearClipPlane + cameraNearOffset;
        // damageUI 객체의 부모를 카메라로 설정
        damageUI.parent = Camera.main.transform;
        // damageUI의 위치 값을 xy는 0, z값은 카메라의 near 값으로 설정
        damageUI.localPosition = new Vector3(0, 0, z);
        // damageImage는 보이지 않도록 초기에 비활성화
        damageImage.enabled = false;
    }

    void Update()
    {
        
    }

    // 피격 표시 시간
    public float damageTime = 0.1f;

    // 피격 처리 코루틴
    IEnumerator DamageEvent()
    {
        // damageImage 컴포넌트 활성화
        damageImage.enabled = true;
        // damageTime 만큼 대기
        yield return new WaitForSeconds(damageTime);
        // damageImage 컴포넌트 비활성화
        damageImage.enabled = false;
    }
}
