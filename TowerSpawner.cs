using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타워 건설 버튼을 눌렀을때 적용하는 함수를 여기서 만들었다
public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate[] towerTemplate;
    //[SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold; // 플레이어가 가지고 있는 총 골드
    //[SerializeField] private int towerBuildGold = 50; // 타워 건설에 소모되는 골드
    [SerializeField] private SystemTextViewer systemTextViewer;
    private bool isOnTowerButton = false; // 타워 건설 버튼을 눌렀는지 체크
    // 임시 타워 사용 완료 시 삭제를 위해 저장하는 변수
    private GameObject followTowerClone = null;
    private int towerType; // 타워 속성

    // 타입은 버튼에서 받아오는구나 인덱스가 필요해서
    public void ReadyToSpawnTower(int type)
    {
        towerType = type;
        // 버튼을 여러번 눌렀을때 이 장치가 없으면 프리패스되버림
        if (isOnTowerButton)
            return;


        if(towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        isOnTowerButton = true;
        // 여기서호출
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        // 타워 건설을 취소 할 수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }

    // 마우스 따라갈수있는 행동 

    // 여기는 매개변수를 받아서 많이쓰네 
    // 하긴 위치만 받으면 그 위치에 생성됨 
    public void SpawnTower(Transform tileTransform)
    {
        if (!isOnTowerButton)   
            return;

        Tile tile = tileTransform.GetComponent<Tile>();
        if (tile.IsBuildTower)
        {
            systemTextViewer.PrintText(SystemType.Build);   
            return;
        }

        isOnTowerButton = false;

        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;
        // ※ 타일보다 z축 -1의 위치에 배치  광선 감지를 끌수없으니까 z축을 뒤로밀었구나 눌렀을때 유아이띄울려고
        Vector3 position = tileTransform.position + Vector3.back;
        // 선택한 타일의 위치에 타워 건설                                                               
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab , position, Quaternion.identity);
        // 타워 생성즉시 적의 정보를 전달한다 타워프리팹에 붙어있는 TowerWeapon스크립트의 setup함수통해서
        clone.GetComponent<TowerWeapon>().Setup(this,enemySpawner, playerGold, tile);

        OnBuffAllBuffTowers();

        Destroy(followTowerClone);
        // 타워 건설을 취소 할 수 있는 코루틴 함수 중지
        StopCoroutine("OnTowerCancelSystem");
    }
    // 어차피 어떤 상황에 다시 코루틴이 스탑되기때문에 무한루프여도 상관이없다
    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }
    }
    // 어떤 타워가 설치된다 모든 타워 탐색 버프타워있으면 버프 갱신
    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();
            // 모든 타워를 탐색해서 buff타워인것들 이게 어디서 호출되는지
            if (weapon.WeaponType == WeaponType.Buff)
                weapon.OnBuffAroundTower();
        }
    }

}
