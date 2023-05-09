using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여기선 struct로 사용하네 class 랑 별다를게 없는건가
[System.Serializable]
public struct Wave
{
    public float spawnTime; // 생성주기
    public int maxEnemyCount; // 한 웨이브의 적의숫자
    public GameObject[] enemyPrefabs;  // 적의 종류
}
public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private EnemySpawner enemySpawner;
    private int currentWaveIndex = -1;

    // 웨이브 정보 출력을 위한 Get 프로퍼티  // 텍스트에 데이터값넘길예정 // 프로퍼티로 쓰니까 어떤상황에 두지않고
    // 그냥 전역변수에 두면 알아서 때에 따라 값을 넘겨받겠네
    public int CurrentWave => currentWaveIndex + 1; // -1이라서 1더한다고함
    public int MaxWave => waves.Length;

    public void StartWave()
    {
        // list가 0이다 == 현재맵에 적이없다 적이있으면 다음웨이브가 나오지못한다
        // 리스트값을 알기위해서 스크립트를 가져온거네
         if(enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length - 1)
        {
            currentWaveIndex++;

            // 에너미 스포너 == 스테이지 스테이지에 웨이브 정보 제공
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}
