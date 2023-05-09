using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 진짜 스크립트하나에 기능한개네
public class EnemyHPViewer : MonoBehaviour
{
    private EnemyHP enemyHP;
    private Slider hpSlider;


    public void Setup(EnemyHP enemyHP)
    {
        this.enemyHP = enemyHP;
        hpSlider = GetComponent<Slider>();
    }

    void Update()
    {
        hpSlider.value = enemyHP.CurrentHP / enemyHP.MaxHP;
    }
}
