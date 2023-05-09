using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//trigger �� �浹������ enemy�� �ӵ��� �����Ѵ� Movement2D��ũ��Ʈ �����̿�
public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;
    void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        // �̵��ӵ� = �̵��ӵ� - �̵��ӵ� * ���ӷ�
        // 4 = 5 - 5 * 0.2;
        movement2D.MoveSpeed -= movement2D.MoveSpeed * towerWeapon.Slow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        collision.GetComponent<Movement2D>().ResetMoveSpeed();
    }
}
