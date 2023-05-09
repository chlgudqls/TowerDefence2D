using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ÿ�� �����տ� �ٴ� ��ũ��Ʈ
// Ÿ���� Ÿ�Ժ��� ����
// ���� Ÿ���� �ν�����â���� ������ ������ ����
// Ÿ���� �ٴ� ��ũ��Ʈ������
public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
// �̰� �Լ������� �����
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, } // AttackToTarget ����� ������ ���°� ���⸶�� �ٸ��⶧��
public class TowerWeapon : MonoBehaviour
{
    // ����� ����ؼ� ����� ĳ�������� �����Ѵ�
    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate; // Ÿ�� ����
    [SerializeField] private Transform spawnPoint; // �߻�ü ���� ��ġ
    [SerializeField] private WeaponType weaponType;
    //[SerializeField] private float attackRate = 0.5f; // ���� �ӵ�
    //[SerializeField] private float attackRange = 2.0f; // ���� ���� 
    //[SerializeField] private int attackDamage = 1; // ���ݷ�
    [Header("Cannon")]
    [SerializeField] private GameObject projectTilePrefab; // �߻�ü ������

    // ������ Ÿ���� �ʿ��� ������     //  �𸣰��� �ϴ��غ��߾˰Ű�����
    [Header("Laser")]
    // �̰� �ƿ� ó���Ẹ�� ������Ʈ
    [SerializeField] private  LineRenderer lineRenderer; // �������� ���Ǵ� ��
    [SerializeField] private Transform hitEffect; // Ÿ�� ȿ��
    [SerializeField] private LayerMask targetLayer; // ������ �ε����� ���̾� ����
    [Header("Buff")]
    private float addedDamage; // ������ ���� �߰��� ������
    private int buffLevel; // ������ �޴��� ���� ����


    private int level = 0; // Ÿ�� ����
    private WeaponState weaponState = WeaponState.SearchTarget; // Ÿ�� ������ ����
    private Transform attackTarget = null; // ���� ���
    // ���ӿ� �����ϴ� �� ���� ȹ�� �뵵 list�� ���⼭ �������°ǰ�
    private EnemySpawner enemySpawner;
    // �̰� ���� ���߰�
    private TowerSpawner towerSpawner;

    private SpriteRenderer spriteRenderer; // Ÿ�� ������Ʈ �̹��� ����
    private PlayerGold playerGold; // �÷��̾��� ��� ���� ȹ�� �� ����
    private Tile ownerTile; // ���� Ÿ���� ��ġ�Ǿ��ִ� Ÿ��


    // ���� ����� �� ���� �ִ��� ������ �������ϳ��� Ÿ�Կ����� ���ݵ� �� �����ϱ⋚����
    //public float Damage => attackDamage;
    //public float Rate => attackRate;
    //public float Range => attackRange;
    // Ÿ�������� �������µ� �����P�µ� ����������� �Ϸ��°���
    // �� �ٷιؿ��� +1�� ���ְ��ִµ� �ε����� ������ 0�� ���°� �³� �����ϱ� ��������
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    // ������ �ִ뷹�� �ʰ��ϸ� ��� 0 ���� ����
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level+1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level + 1;
    // �߽������� ������� ���׷��̵� �Ұ��Ѱǰ�
    // ������ ������ �����μ�
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;

    // ����Ÿ�� �����ִ��� get�Ҽ��ְ� �ߴ� �б�����������Ƽ ����
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    // ���׷��̵带 ���⼭ �ؾߵǼ� �ѱ�ű��� ��������� ó����ġ�� Ÿ���� �������
    public void Setup(TowerSpawner towerSpawner,EnemySpawner enemySpawner, PlayerGold playerGold, Tile tile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = tile;


        //ChangeState(WeaponState.SearchTarget);  // ���ο� �߰��� ���� �ּ�ó�� ���ο��϶� �̰� �����ʴµ�
        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
    }
    
    public void ChangeState(WeaponState newState)
    {
        // ������ ������̴� ���� ����
        StopCoroutine(weaponState.ToString());
        // ���� ����
        weaponState = newState;
        // ���ο� ���� ���
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    // �ٶ󺻴ٴ°� ���������Ѵٴ¶�
    private void RotateToTarget()
    {
        // �������κ����� �Ÿ��� ���������κ����� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�� �̿�
        // ���� = arctan(y/x)
        // x, y ������ ���ϱ�
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y �������� �������� ���� ���ϱ�
        // ������ radian���� �̱⶧���� Mathf.Rad2Deg�� ���� �� ������ ����
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {

            attackTarget = FindCloseAttackTarget();

            if (attackTarget != null)
            {
                // ���⼭ ���� ����Ÿ�Կ� ���� �ڷ�ƾ�� �����ϴ±��� �����տ� ���������س��� ����Ÿ���� �޸𸮿� ����Ǿ������Ŵ�
                // if������ �б���Ѽ� �ڷ�ƾ���� ��Ȳ�� �°� ȣ����
                if(weaponType == WeaponType.Cannon)
                    ChangeState(WeaponState.TryAttackCannon);
                else if (weaponType == WeaponType.Laser)
                    ChangeState(WeaponState.TryAttackLaser);
            }
            yield return null;
        }
    }
    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            //// �ٸ� �߻�ü�� ���� ���ŵưų� Goal���� �����̵��ؼ� ������
            //if(attackTarget == null)
            //{
            //    ChangeState(WeaponState.SearchTarget);
            //    break;
            //}

            //// ������ �Ÿ��� �ٽñ��ϰ� ������ ��������
            //float distance = Vector3.Distance(attackTarget.position, transform.position);

            //if (distance > towerTemplate.weapon[level].range)
            //{
            //    attackTarget = null;
            //    ChangeState(WeaponState.SearchTarget);
            //    break;
            //}

            if (!IsPossibleToAttackTarget())
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }
    // ü������ �̰� ȣ���
    private IEnumerator TryAttackLaser()
    {
        // �ϴ� Ȱ��ȭ�� ��Ű�°ǰ�
        EnableLaser();

        while (true)
        {
            // ���⼭ �� �����ȿ� �ְ� Ÿ���� �������ƴ��� true false check�ϰ� true�� �Ѿ����
            if(!IsPossibleToAttackTarget())
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            SpawnLaser();

            yield return null;
        }
    }
    // ���� ���� ������ø �Ұ�, Ÿ�� �������� ���� ���������� �н�
    // ����Ÿ���� ȣ���ϰڳ� 
    public void OnBuffAroundTower()
    {
        // �ʿ��ִ� ��� Ÿ���� Ž���ؼ� ��ũ��Ʈ�������´� �������̱⶧���� 
        // �ݺ����ȿ��� ���������ϳ��ϳ� ã�� ���� ���������� �� ũ�� �������� ������ �Ʒ���������
        // ���� ������ range �ɷ����� �Լ�
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();
             
            // ���� ���������� ����Ÿ���� �������� ũ�� �ٸ� Ÿ���� ����
            // �׷� �� ū ������ Ž���Ѵ� �Ʒ����� �Ʒ��� ������������ �������� �߰�
            if (weapon.BuffLevel > Level)
                continue;

            // �̰� �ٸ��� slow�� �ٸ��� ���� �Ÿ��� ���� ����� �±׷��ϴ°� �ƴϰ�
            // ���� Ÿ���� ���� Ž���ϰ��ִ� ���� Ÿ���� �Ÿ��� �����Ÿ� �ƹ�ư ������
            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // ����Ÿ�������� ���������� ����
                if(weapon.weaponType == WeaponType.Cannon || weapon.weaponType == WeaponType.Laser)
                {
                    // ������ ���� ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * towerTemplate.weapon[level].buff;
                    // Ÿ���� �ް� �ִ� ���� ���� ����
                    weapon.BuffLevel = Level;
                }
            }
        }
    }
    private Transform FindCloseAttackTarget() 
    {
        // ���� �����̿� �ִ� ���� ã�����ؼ� ���� �Ÿ��� �ִ��� ũ�� �����Ѵٰ���
        float closestDistSqr = Mathf.Infinity;
        // ���� list�� ���ʷξ��� ���� �ʿ� �����ϴ� ��� �� �˻�
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // �ϴ� �Ÿ��� ���ϰ� �װŸ��� �˻��Ѵ�
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }
        return attackTarget;
    }
    // �����ϰ� bool�� �ٲ㼭 ������ �Լ��� ȣ���ߴµ� bool�� Ȯ���ؼ� false�϶� search�� ü��������
    private bool IsPossibleToAttackTarget()
    {
        if (attackTarget == null)
            return false;

        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if(distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }
        return true;
    }
    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectTilePrefab, spawnPoint.position, Quaternion.identity);
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    private void EnableLaser()
    {
        // �Ʊ����� LineRenderer�� HitEffect  ������ȿ�����°Ͱ� ��ƼŬ�ΰŰ����� ��� �Ϸ��°����� �𸣰���
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }
    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }
    // ������ �׳� Ȱ��ȭ �ΰŰ�
    // ���⼭ ������ �������� ���� ȿ���� ���°Ű�����
    private void SpawnLaser()
    {
        // ���⼭ ���� ���̾� ����ũ�� �̿��ؼ� ���ϴ°���
        // ��ǥ - �߻���ġ�� ������ ���Ѵ� �³� �������� ����� ������ �ʿ��ϴϱ� �ɳ� �Ȱ����ʳ�
        Vector3 direction = attackTarget.position - spawnPoint.position;
        // ��� �͵��߿��� ��͵��� ���Ϸ��°ǰ� �����ȿ��ִ� ���͵��߿� ���尡���� ã�����°ǰ� ���ڼ� ���� ���ߵǳ�
        // ���� �������� ������ ��������� 
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        // �������� ���� attackTarget�� ������ ������Ʈ ����
        for (int i = 0; i < hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                // �����ѰͿ� ������ �Ϸ��µ� �׳� �������� ������Ǵ°žƴѰ� ���� �̾�ߵǴ±��� ���ǹ��� �ش��ϴ� ������Ʈ�� ��������
                // ���� ������ ���� �ε��� 2���߿� 0�̽����� 1�� �����ΰ�����
                lineRenderer.SetPosition(0, spawnPoint.position);
                // ���� ��ǥ���� // z ���� ���� ���� ������ߵǳ� �ȱ׷��� ��� �ǳ�����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // Ÿ�� ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;
                // �� ü�� ����
                // �������� �ڷ�ƾ���� ��ӽ���Ǳ⶧���� 1�ʿ� ��������ŭ �����ϰ� deltatime�� ��������
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    // ���⿡�� Ÿ���������� �����ϴٺ���
    public bool Upgrade()
    {
        // ��尡 �������
        if(playerGold.CurrentGold < towerTemplate.weapon[level].cost)
            return false;

        level++;

        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;
        // ���׷��̵��Ҷ� �������� ����� linerenderer�� �Ǹ����ֳ� ������ ���⸦ level��ŭ �÷���
        if (weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        // �ٵ� �����Ÿ��ȿ� �ִ��� �׻�üũ���ʿ䰡���ڳ�  Ÿ���� �������� ������ �����غ��� �׷�
        // ���� ���Ÿ� �ѹ������ָ�� ����Ÿ���� ���׷��̵�������� Ÿ���� ��ġ������  �� �λ�Ȳ
        // Ÿ���� ���׷��̵� �ɶ� ��� ���� Ÿ���� ���� ȿ�� ����
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }
    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;

        ownerTile.IsBuildTower = false;

        Destroy(gameObject);
    }
}
