using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ÿ�� �Ǽ� ��ư�� �������� �����ϴ� �Լ��� ���⼭ �������
public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate[] towerTemplate;
    //[SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold; // �÷��̾ ������ �ִ� �� ���
    //[SerializeField] private int towerBuildGold = 50; // Ÿ�� �Ǽ��� �Ҹ�Ǵ� ���
    [SerializeField] private SystemTextViewer systemTextViewer;
    private bool isOnTowerButton = false; // Ÿ�� �Ǽ� ��ư�� �������� üũ
    // �ӽ� Ÿ�� ��� �Ϸ� �� ������ ���� �����ϴ� ����
    private GameObject followTowerClone = null;
    private int towerType; // Ÿ�� �Ӽ�

    // Ÿ���� ��ư���� �޾ƿ��±��� �ε����� �ʿ��ؼ�
    public void ReadyToSpawnTower(int type)
    {
        towerType = type;
        // ��ư�� ������ �������� �� ��ġ�� ������ �����н��ǹ���
        if (isOnTowerButton)
            return;


        if(towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        isOnTowerButton = true;
        // ���⼭ȣ��
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        // Ÿ�� �Ǽ��� ��� �� �� �ִ� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnTowerCancelSystem");
    }

    // ���콺 ���󰥼��ִ� �ൿ 

    // ����� �Ű������� �޾Ƽ� ���̾��� 
    // �ϱ� ��ġ�� ������ �� ��ġ�� ������ 
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
        // �� Ÿ�Ϻ��� z�� -1�� ��ġ�� ��ġ  ���� ������ ���������ϱ� z���� �ڷιо����� �������� �����̶�����
        Vector3 position = tileTransform.position + Vector3.back;
        // ������ Ÿ���� ��ġ�� Ÿ�� �Ǽ�                                                               
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab , position, Quaternion.identity);
        // Ÿ�� ������� ���� ������ �����Ѵ� Ÿ�������տ� �پ��ִ� TowerWeapon��ũ��Ʈ�� setup�Լ����ؼ�
        clone.GetComponent<TowerWeapon>().Setup(this,enemySpawner, playerGold, tile);

        OnBuffAllBuffTowers();

        Destroy(followTowerClone);
        // Ÿ�� �Ǽ��� ��� �� �� �ִ� �ڷ�ƾ �Լ� ����
        StopCoroutine("OnTowerCancelSystem");
    }
    // ������ � ��Ȳ�� �ٽ� �ڷ�ƾ�� ��ž�Ǳ⶧���� ���ѷ������� ����̾���
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
    // � Ÿ���� ��ġ�ȴ� ��� Ÿ�� Ž�� ����Ÿ�������� ���� ����
    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();
            // ��� Ÿ���� Ž���ؼ� buffŸ���ΰ͵� �̰� ��� ȣ��Ǵ���
            if (weapon.WeaponType == WeaponType.Buff)
                weapon.OnBuffAroundTower();
        }
    }

}
