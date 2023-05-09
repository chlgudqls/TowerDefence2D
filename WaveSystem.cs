using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���⼱ struct�� ����ϳ� class �� ���ٸ��� ���°ǰ�
[System.Serializable]
public struct Wave
{
    public float spawnTime; // �����ֱ�
    public int maxEnemyCount; // �� ���̺��� ���Ǽ���
    public GameObject[] enemyPrefabs;  // ���� ����
}
public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private EnemySpawner enemySpawner;
    private int currentWaveIndex = -1;

    // ���̺� ���� ����� ���� Get ������Ƽ  // �ؽ�Ʈ�� �����Ͱ��ѱ濹�� // ������Ƽ�� ���ϱ� ���Ȳ�� �����ʰ�
    // �׳� ���������� �θ� �˾Ƽ� ���� ���� ���� �Ѱܹްڳ�
    public int CurrentWave => currentWaveIndex + 1; // -1�̶� 1���Ѵٰ���
    public int MaxWave => waves.Length;

    public void StartWave()
    {
        // list�� 0�̴� == ����ʿ� ���̾��� ���������� �������̺갡 ���������Ѵ�
        // ����Ʈ���� �˱����ؼ� ��ũ��Ʈ�� �����°ų�
         if(enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length - 1)
        {
            currentWaveIndex++;

            // ���ʹ� ������ == �������� ���������� ���̺� ���� ����
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}
