using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // ��ü�� ��Ҵ��� ����
    bool isGrabbing = false;
    // ��� �ִ� ��
    GameObject grabbedObject;
    // ���� ��ü ����
    public LayerMask grabbedLayer;
    // ���� �� �ִ� �Ÿ�
    public float grabRange = 0.2f;

    void Start()
    {
        
    }

    void Update()
    {
        // ��� ���� �ʴ� ������ �� ��� �õ�
        if (isGrabbing == false)
        {
            TryGrab();
        }
    }

    void TryGrab()
    {
        // Grab ��ư�� ������
        if(ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // ���� ���� �ȿ� ��ź�� ���� ��
            Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);
            // ��ź�� ��´�
            int closest = 0;
            // �հ� ���� ����� ��ü ����
            for (int i = 1; i < hitObjects.Length; i++)
            {
                // ���� ����� ��ü�� ���� �Ÿ�
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
                // ���� ��ü�� ���� �Ÿ�
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
                // ���� ��ü���� �Ÿ��� �� ������ �ε��� ��ü
                if(nextDistance < closestDistance)
                {
                    closest = i;
                }
            }
        }
    }
}
