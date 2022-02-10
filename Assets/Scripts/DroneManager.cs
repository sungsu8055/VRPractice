using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // 랜덤 시간 범위
    public float minTime = 1;
    public float maxTime = 5;
    // 생성 시간
    float createTime;
    // 경과 시간
    float currentTime;
    // 드론 생성 위치
    public Transform[] spawnPoint;
    // 드론 프리팹
    public GameObject dronePrefab;

    void Start()
    {
        // createTime 값을 랜덤 시간 범위로 설정
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        // 랜덤한 생성 시간마다 드론을 한 개씩 생성
        // 시간 경과
        currentTime += Time.deltaTime;
        // 경과 시간이 생성 시간을 초과
        if(currentTime > createTime)
        {
            // 드론 프리팹을 Instantiate
            GameObject drone = Instantiate(dronePrefab);
            // 프리팹을 spawnPoint 로 위치
            int index = Random.Range(0, spawnPoint.Length);
            drone.transform.position = spawnPoint[index].position;
            // 경과 시간 초기화
            currentTime = 0f;
            // 랜덤 생성 시간 재할당
            createTime = Random.Range(minTime, maxTime);
        }
    }
}
