using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private float maxHP; // 최대 체력
    private float currentHP; // 현재 체력
    private bool isDie = false; // 적이 사망 상태면 isDie를 true로 설정
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    // 외부에서 확인하게 하려고 이런식으로 따로 만들어줌 public으로 컴포넌트만알면 쉽게 접근가능
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
        // 체력이 데미지만큼 감소해서 죽을 상황인데 죽기전에 공격이 들어오면 
        // 쓸데없이 Ondie 함수가 여러번 실행된다  그래서 bool이 필요

        if (isDie) return;

        currentHP -= damage;

        // 투명도를 조절하는 효과를 적용할거같은데 그래서 sprite를 받아왔다
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
        // 변수에 저장하고
        Color color = spriteRenderer.color;
        
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 깜빡거리는 효과를 만드는거임
        yield return new WaitForSeconds(0.05f);

        // 되돌림
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
