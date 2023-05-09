using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 변수 세개로 한 스테이지에 
    //[SerializeField] private GameObject enemyPrefab; // 적 프리팹

    [SerializeField] private GameObject enemyHPSliderPrefab; // 적 체력을 나타내는 슬라이더 프리팹 적이 생성되는만큼 생성해야되서 프리팹은 여기서
    // 여기가 스테이지다보니 
    [SerializeField] private Transform canvasTransform; 

    //[SerializeField] private float spawnTime; // 적 생성주기
    [SerializeField] private Transform[] wayPoints; // 현재 스테이지의 이동 경로

    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private PlayerGold playerGold;

    private Wave currentWave;
    private int currentEnemyCount; 
    // 배열과 차이점 가변적임 add로 원하는 만큼 넣을수있다 미리정하는게아님
    private List<Enemy> enemyList = new List<Enemy>();

    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    // 이렇게하면 get만 쓰는듯  set 못씀
    public List<Enemy> EnemyList => enemyList;

    void Awake()
    {
        // 적 생성 코루틴 함수 호출
        // 게임실행되면 바로 등장했었다
        // 원하는 타이밍에 등장시킬예정 그럼 이 코루틴을 어느시점에
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;

        // 일단 웨이브 시작하면 바로 현재 웨이브의 최대카운트 저장
        currentEnemyCount = currentWave.maxEnemyCount;

        StartCoroutine("SpawnEnemy");
    }
    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;
        // 그럼이거 true 언제 멈추는데 멈추는 조건이없는데 이대로 무한루프에 빠지게 두네
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // 인덱스중 랜덤으로 적 종류 구해줌
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
    // 죽음으로부터 넘어오는 타입에 따라 분기
    // 죽음타입에따라서 실행할함수가있기떄문에 
    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if(type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            // kill하고나서 
            playerGold.CurrentGold += gold;

        }
        // 사망부분에서 감소시킨다
        currentEnemyCount--;
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        // 이 스테이지와 같은 스크립트가 적이생성될때 적과같이 생성되서 따라다녀야 되기때문에 근데 함수로 따로빼서 생성되는부분에 같이쓴다
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        // 캔버스안에 있어야보인다, 
        sliderClone.transform.SetParent(canvasTransform);
        // 계층설정으로 인해 크기가바껴서 다시 설정
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderAutoPositionSetter>().Setup(enemy.transform);

        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
