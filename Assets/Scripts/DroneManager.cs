using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // ���� �ð� ����
    public float minTime = 1;
    public float maxTime = 5;
    // ���� �ð�
    float createTime;
    // ��� �ð�
    float currentTime;
    // ��� ���� ��ġ
    public Transform[] spawnPoint;
    // ��� ������
    public GameObject dronePrefab;

    void Start()
    {
        // createTime ���� ���� �ð� ������ ����
        createTime = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        // ������ ���� �ð����� ����� �� ���� ����
        // �ð� ���
        currentTime += Time.deltaTime;
        // ��� �ð��� ���� �ð��� �ʰ�
        if(currentTime > createTime)
        {
            // ��� �������� Instantiate
            GameObject drone = Instantiate(dronePrefab);
            // �������� spawnPoint �� ��ġ
            int index = Random.Range(0, spawnPoint.Length);
            drone.transform.position = spawnPoint[index].position;
            // ��� �ð� �ʱ�ȭ
            currentTime = 0f;
            // ���� ���� �ð� ���Ҵ�
            createTime = Random.Range(minTime, maxTime);
        }
    }
}
