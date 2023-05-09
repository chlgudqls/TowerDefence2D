using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.0f;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    // get�� �����ϰ� �̷������� �Ѱǰ�

    private float baseMoveSpeed;
    public float MoveSpeed
    {
        // ������ �����ʵ��� �Ѵٰ��� // �³� �ּ� 0�̰ڴ� ������ �����ʰڴ�
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    private void Awake()
    {
        baseMoveSpeed = moveSpeed;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // �ܺο��� �̵������� ����
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
    public void ResetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;
    }
}
