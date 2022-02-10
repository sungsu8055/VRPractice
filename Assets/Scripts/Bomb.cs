using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ���� ȿ��
    Transform explosion;
    ParticleSystem expEffect;
    AudioSource expAudio;
    // ���� �ݰ�
    public float expRange = 5;

    void Start()
    {
        // Explosion ��ü �ʱ�ȭ
        explosion = GameObject.Find("Explosion").transform;
        // ParticleSystem ������Ʈ �ʱ�ȭ
        expEffect = explosion.GetComponent<ParticleSystem>();
        // AudioSource ������Ʈ �ʱ�ȭ
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // ���̾� ����ũ ����
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        // ��ź ��ġ �߽����� ���� �ݰ� �ȿ� ���� ��� �˻�
        Collider[] drones = Physics.OverlapSphere(this.transform.position, expRange, layerMask);
        // ���� �ȿ� �ִ� ��� ��� ����
        foreach(Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        // ���� ȿ�� ��ġ ����
        explosion.position = this.transform.position;
        // ����Ʈ ���
        expEffect.Play();
        // ���� ���
        expAudio.Play();
        // ��ź ����
        Destroy(this.gameObject);
    }
}
