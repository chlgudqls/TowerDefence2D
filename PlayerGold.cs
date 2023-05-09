using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 골드도 따로 만듬
// 골드 변수선언후 기본값 설정해주고 변수 get,set허용
public class PlayerGold : MonoBehaviour
{
    [SerializeField] private int currentGold = 100;

    public int CurrentGold
    {
        // value 들어오는 value 값 가장큰값?
        set => currentGold = Mathf.Max(0, value);
        get => currentGold;
    }


}
