using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderAutoPositionSetter : MonoBehaviour
{
    // 적의 포지션과 곱해서 따라다니게할거같기도
    [SerializeField] private Vector3 distance = Vector3.down * 20.0f;
    // 적
    private Transform targetTransform;
    // 모름 스크롤?
    private RectTransform rectTransform;

    

    //  보통 셋업을 통해서 뭔가를 던져줌 계산 하게끔
    // 
    public void Setup(Transform target)
    {
        targetTransform = target;
        // 자기껄 가져오려고 
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if(targetTransform == null)
        {
            // 널이면 스크롤바 파괴하려고
            Destroy(gameObject);
            return;
        }
        // 2D 좌표가 스크롤 UI 스크린좌표로 타겟에너미를 변환해서 그값을 distance만큼 더해주면 따라다닌다 이것은 lateUpdate에서 한다
        // 왜 3D취급하지 오브젝트를 ui랑 나누는건가 원래
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);

        rectTransform.position = screenPosition + distance;
    }

}
