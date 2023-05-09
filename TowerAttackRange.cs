using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타워의 범위를 활성화, 비활성화예정
public class TowerAttackRange : MonoBehaviour
{
    //void Awake()
    //{
    //    OffAttackRange();
    //}

    // 해당타워의 위치, 공격범위를 받아서 계산
    // 범위를 토대로 실제 이미지의 크기를 바꿈
    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        // 실제 타워의 공격범위수치를 넘겨받아서 범위크기를 변경시킴   실제 수치가 반지름과 같네 그래서 *2
        // 공격범위 크기
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;

        transform.position = position;
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
}
