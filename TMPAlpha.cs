using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// �ؽ�Ʈ�� ������ ������ ����
// � ����� �����°���
// ����ȿ���� ���� ��ũ��Ʈ�� ���� �����
// viewr��ũ��Ʈ�� ���θ���� ���Ÿ� ����ؼ� �׿� �ش��ϴ� �ؽ�Ʈ�� ��½�Ű�� �� �Լ��� ȣ���Ŵ
public class TMPAlpha : MonoBehaviour
{
    [SerializeField] private float lerpTime = 0.5f;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0));
    }
    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while(percent < 1)
        {
            // ��¥�׳� �ۼ�Ʈ lerpTime ���������ϰ�  
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;

            Color color = text.color;
            // ������ �̷������� ���� ������� ������ ����� percent�� ���ܿ�
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;

            yield return null;
        }
    }
}
