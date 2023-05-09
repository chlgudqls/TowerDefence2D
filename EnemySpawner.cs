using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // ���� ������ �� ���������� 
    //[SerializeField] private GameObject enemyPrefab; // �� ������

    [SerializeField] private GameObject enemyHPSliderPrefab; // �� ü���� ��Ÿ���� �����̴� ������ ���� �����Ǵ¸�ŭ �����ؾߵǼ� �������� ���⼭
    // ���Ⱑ ���������ٺ��� 
    [SerializeField] private Transform canvasTransform; 

    //[SerializeField] private float spawnTime; // �� �����ֱ�
    [SerializeField] private Transform[] wayPoints; // ���� ���������� �̵� ���

    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private PlayerGold playerGold;

    private Wave currentWave;
    private int currentEnemyCount; 
    // �迭�� ������ �������� add�� ���ϴ� ��ŭ �������ִ� �̸����ϴ°Ծƴ�
    private List<Enemy> enemyList = new List<Enemy>();

    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    // �̷����ϸ� get�� ���µ�  set ����
    public List<Enemy> EnemyList => enemyList;

    void Awake()
    {
        // �� ���� �ڷ�ƾ �Լ� ȣ��
        // ���ӽ���Ǹ� �ٷ� �����߾���
        // ���ϴ� Ÿ�ֿ̹� �����ų���� �׷� �� �ڷ�ƾ�� ���������
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;

        // �ϴ� ���̺� �����ϸ� �ٷ� ���� ���̺��� �ִ�ī��Ʈ ����
        currentEnemyCount = currentWave.maxEnemyCount;

        StartCoroutine("SpawnEnemy");
    }
    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;
        // �׷��̰� true ���� ���ߴµ� ���ߴ� �����̾��µ� �̴�� ���ѷ����� ������ �γ�
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // �ε����� �������� �� ���� ������
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this,wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }
    // �������κ��� �Ѿ���� Ÿ�Կ� ���� �б�
    // ����Ÿ�Կ����� �������Լ����ֱ⋚���� 
    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if(type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            // kill�ϰ��� 
            playerGold.CurrentGold += gold;

        }
        // ����κп��� ���ҽ�Ų��
        currentEnemyCount--;
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        // �� ���������� ���� ��ũ��Ʈ�� ���̻����ɶ� �������� �����Ǽ� ����ٳ�� �Ǳ⶧���� �ٵ� �Լ��� ���λ��� �����Ǵºκп� ���̾���
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        // ĵ�����ȿ� �־�ߺ��δ�, 
        sliderClone.transform.SetParent(canvasTransform);
        // ������������ ���� ũ�Ⱑ�ٲ��� �ٽ� ����
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderAutoPositionSetter>().Setup(enemy.transform);

        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
