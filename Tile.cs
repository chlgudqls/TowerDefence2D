using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuildTower { set; get; }
    void Awake()
    {
        IsBuildTower = false;
    }

}
