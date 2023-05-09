using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ÿ���� ������ Ȱ��ȭ, ��Ȱ��ȭ����
public class TowerAttackRange : MonoBehaviour
{
    //void Awake()
    //{
    //    OffAttackRange();
    //}

    // �ش�Ÿ���� ��ġ, ���ݹ����� �޾Ƽ� ���
    // ������ ���� ���� �̹����� ũ�⸦ �ٲ�
    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        // ���� Ÿ���� ���ݹ�����ġ�� �Ѱܹ޾Ƽ� ����ũ�⸦ �����Ŵ   ���� ��ġ�� �������� ���� �׷��� *2
        // ���ݹ��� ũ��
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;

        transform.position = position;
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
}
