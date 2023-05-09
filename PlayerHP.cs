using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Image imageScreen; // ��üȭ���� ���� ������ �̹���
    // �׻� ��ġ������� � ���̱��δ� �׷� �̷��������ʿ��ϳ�
    [SerializeField] private float maxHP = 20;
    private float currentHP;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    void Awake()
    {
        currentHP = maxHP;
    }

    // �÷��̾��� �ִ� ü���� �����ǰ�
    // � �г�Ƽ�� �Լ��� ���� ü���� �����ϰ� �ᱹ�� �װ���
    // ������ ���� �ٲٰų� ��Ÿ�ൿ�� ó���Ѵ� 
    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        // ���ӿ���
        if(currentHP <= 0)
        {

        }
    }

    // �÷��̾� HP �����̿� �����ų ȿ��
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
