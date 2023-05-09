using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// 텍스트의 투명도를 제어할 예정
// 어떤 방식을 쓰려는건지
// 알파효과에 대한 스크립트를 따로 만들고
// viewr스크립트를 따로만들고 열거를 사용해서 그에 해당하는 텍스트를 출력시키고 이 함수도 호출시킴
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
            // 진짜그냥 퍼센트 lerpTime 수정가능하게  
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;

            Color color = text.color;
            // 투명도를 이런식으로 변경 러프사용 위에서 계산한 percent는 끝단에
            color.a = Mathf.Lerp(start, end, percent);
            text.color = color;

            yield return null;
        }
    }
}
