using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Image imageScreen; // 전체화면을 덮는 빨간색 이미지
    // 항상어떤 수치기반으로 어떤 값이깎인다 그럼 이런변수가필요하네
    [SerializeField] private float maxHP = 20;
    private float currentHP;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    void Awake()
    {
        currentHP = maxHP;
    }

    // 플레이어의 최대 체력이 설정되고
    // 어떤 패널티의 함수에 의해 체력이 감소하고 결국엔 죽겠지
    // 죽을땐 씬을 바꾸거나 기타행동을 처리한다 
    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        // 게임오버
        if(currentHP <= 0)
        {

        }
    }

    // 플레이어 HP 유아이에 적용시킬 효과
    private IEnumerator HitAlphaAnimation()
    {
        Color color = imageScreen.color;

        color.a = 0.4f;

        imageScreen.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;

            yield return null;
        }
    }
}
