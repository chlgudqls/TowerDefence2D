using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private float maxHP; // �ִ� ü��
    private float currentHP; // ���� ü��
    private bool isDie = false; // ���� ��� ���¸� isDie�� true�� ����
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    // �ܺο��� Ȯ���ϰ� �Ϸ��� �̷������� ���� ������� public���� ������Ʈ���˸� ���� ���ٰ���
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;


    void Awake()
    {
        currentHP = maxHP;
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // ü���� ��������ŭ �����ؼ� ���� ��Ȳ�ε� �ױ����� ������ ������ 
        // �������� Ondie �Լ��� ������ ����ȴ�  �׷��� bool�� �ʿ�

        if (isDie) return;

        currentHP -= damage;

        // ������ �����ϴ� ȿ���� �����ҰŰ����� �׷��� sprite�� �޾ƿԴ�
        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHP <= 0)
        {
            enemy.OnDie(EnemyDestroyType.Kill);
            isDie = true;
        }
    }
    private IEnumerator HitAlphaAnimation()
    {
        // ������ �����ϰ�
        Color color = spriteRenderer.color;
        
        color.a = 0.4f;
        spriteRenderer.color = color;

        // �����Ÿ��� ȿ���� ����°���
        yield return new WaitForSeconds(0.05f);

        // �ǵ���
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
