using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderAutoPositionSetter : MonoBehaviour
{
    // ���� �����ǰ� ���ؼ� ����ٴϰ��ҰŰ��⵵
    [SerializeField] private Vector3 distance = Vector3.down * 20.0f;
    // ��
    private Transform targetTransform;
    // �� ��ũ��?
    private RectTransform rectTransform;

    

    //  ���� �¾��� ���ؼ� ������ ������ ��� �ϰԲ�
    // 
    public void Setup(Transform target)
    {
        targetTransform = target;
        // �ڱⲬ ���������� 
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if(targetTransform == null)
        {
            // ���̸� ��ũ�ѹ� �ı��Ϸ���
            Destroy(gameObject);
            return;
        }
        // 2D ��ǥ�� ��ũ�� UI ��ũ����ǥ�� Ÿ�ٿ��ʹ̸� ��ȯ�ؼ� �װ��� distance��ŭ �����ָ� ����ٴѴ� �̰��� lateUpdate���� �Ѵ�
        // �� 3D������� ������Ʈ�� ui�� �����°ǰ� ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);

        rectTransform.position = screenPosition + distance;
    }

}
