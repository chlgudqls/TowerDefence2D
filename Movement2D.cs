using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.0f;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    // get만 가능하게 이런식으로 한건가

    private float baseMoveSpeed;
    public float MoveSpeed
    {
        // 음수가 되지않도록 한다고함 // 맞네 최소 0이겠다 음수가 되지않겠다
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

    // 외부에서 이동방향을 설정
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
    public void ResetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;
    }
}
