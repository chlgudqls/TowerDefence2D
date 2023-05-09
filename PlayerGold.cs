using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��嵵 ���� ����
// ��� ���������� �⺻�� �������ְ� ���� get,set���
public class PlayerGold : MonoBehaviour
{
    [SerializeField] private int currentGold = 100;

    public int CurrentGold
    {
        // value ������ value �� ����ū��?
        set => currentGold = Mathf.Max(0, value);
        get => currentGold;
    }


}
