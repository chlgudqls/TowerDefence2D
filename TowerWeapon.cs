using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 타워 프리팹에 붙는 스크립트
// 타워의 타입별로 구분
// 웨폰 타입은 인스펙터창에서 프리팹 각각에 설정
// 타워에 붙는 스크립트가뭔지
public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
// 이건 함수명으로 사용함
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, } // AttackToTarget 사라짐 어택의 상태가 무기마다 다르기때문
public class TowerWeapon : MonoBehaviour
{
    // 헤더를 사용해서 공통과 캐논변수들을 구분한다
    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate; // 타워 정보
    [SerializeField] private Transform spawnPoint; // 발사체 생성 위치
    [SerializeField] private WeaponType weaponType;
    //[SerializeField] private float attackRate = 0.5f; // 공격 속도
    //[SerializeField] private float attackRange = 2.0f; // 공격 범위 
    //[SerializeField] private int attackDamage = 1; // 공격력
    [Header("Cannon")]
    [SerializeField] private GameObject projectTilePrefab; // 발사체 프리팹

    // 레이저 타워에 필요한 변수들     //  모르겠음 일단해봐야알거같은데
    [Header("Laser")]
    // 이건 아예 처음써보는 컴포넌트
    [SerializeField] private  LineRenderer lineRenderer; // 레이저로 사용되는 선
    [SerializeField] private Transform hitEffect; // 타격 효과
    [SerializeField] private LayerMask targetLayer; // 광선에 부딪히는 레이어 설정
    [Header("Buff")]
    private float addedDamage; // 버프에 의해 추가된 데미지
    private int buffLevel; // 버프를 받는지 여부 설정


    private int level = 0; // 타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget; // 타워 무기의 상태
    private Transform attackTarget = null; // 공격 대상
    // 게임에 존재하는 적 정보 획득 용도 list를 여기서 꺼내쓰는건가
    private EnemySpawner enemySpawner;
    // 이건 뭐지 왜추가
    private TowerSpawner towerSpawner;

    private SpriteRenderer spriteRenderer; // 타워 오브젝트 이미지 변경
    private PlayerGold playerGold; // 플레이어의 골드 정보 획득 및 설정
    private Tile ownerTile; // 현재 타워가 배치되어있는 타일


    // 레벨 빼고는 다 원래 있던거 어차피 프리팹하나에 타입에따라서 스텟들 다 설정하기떄문에
    //public float Damage => attackDamage;
    //public float Rate => attackRate;
    //public float Range => attackRange;
    // 타워정보를 가져오는데 레벨뻇는데 레벨관리어떻게 하려는건지
    // ※ 바로밑에서 +1을 해주고있는데 인덱스에 무조건 0이 들어가는게 맞나 맞으니까 썻겠지만
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    // 레벨이 최대레벨 초과하면 비용 0 으로 나옴
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level+1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public int Level => level + 1;
    // 멕스레벨은 여기오면 업그레이드 불가한건가
    // 웨폰의 갯수가 레벨인셈
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public float Buff => towerTemplate.weapon[level].buff;

    // 웨폰타입 원래있던거 get할수있게 했다 읽기전용프로퍼티 선언
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

    // 업그레이드를 여기서 해야되서 넘긴거구나 골드차감은 처음설치한 타워의 골드차감
    public void Setup(TowerSpawner towerSpawner,EnemySpawner enemySpawner, PlayerGold playerGold, Tile tile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = tile;


        //ChangeState(WeaponState.SearchTarget);  // 슬로우 추가로 인해 주석처리 슬로우일땐 이걸 하지않는듯
        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
    }
    
    public void ChangeState(WeaponState newState)
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        // 상태 변경
        weaponState = newState;
        // 새로운 상태 재생
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    // 바라본다는건 각도조절한다는뜻
    private void RotateToTarget()
    {
        // 원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = arctan(y/x)
        // x, y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radian단위 이기때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
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
                // 여기서 이제 무기타입에 따라 코루틴도 설정하는구나 프리팹에 각각설정해놓은 웨폰타입이 메모리에 저장되어있을거니
                // if문으로 분기시켜서 코루틴각각 상황에 맞게 호출함
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
            //// 다른 발사체에 의해 제거됐거나 Goal지점 까지이동해서 삭제됨
            //if(attackTarget == null)
            //{
            //    ChangeState(WeaponState.SearchTarget);
            //    break;
            //}

            //// 사이의 거리를 다시구하고 범위를 벗어났을경우
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
    // 체인지로 이게 호출됨
    private IEnumerator TryAttackLaser()
    {
        // 일단 활성화만 시키는건가
        EnableLaser();

        while (true)
        {
            // 여기서 또 범위안에 있고 타겟이 널인지아닌지 true false check하고 true면 넘어가겠지
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
    // 버프 설정 버프중첩 불가, 타워 레벨보다 높은 버프레벨은 패스
    // 버프타워만 호출하겠네 
    public void OnBuffAroundTower()
    {
        // 맵에있는 모든 타워를 탐색해서 스크립트를가져온다 프리팹이기때문에 
        // 반복문안에서 버프레벨하나하나 찾고 현재 버프레벨이 더 크면 다음실행 작으면 아래로직실행
        // 버프 레벨과 range 걸러내는 함수
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();
             
            // 현재 버프레벨이 버프타워의 레벨보다 크다 다른 타워로 점프
            // 그럼 더 큰 버프가 탐색한다 아래실행 아래서 버프레벨갱신 데미지도 추가
            if (weapon.BuffLevel > Level)
                continue;

            // 이건 다르네 slow랑 다르게 했음 거리로 범위 계산함 태그로하는게 아니고
            // 맵의 타워와 지금 탐색하고있는 버프 타워의 거리와 사정거리 아무튼 저거임
            if(Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // 버프타워레벨을 버프레벨에 대입
                if(weapon.weaponType == WeaponType.Cannon || weapon.weaponType == WeaponType.Laser)
                {
                    // 버프에 의해 공격력 증가
                    weapon.AddedDamage = weapon.Damage * towerTemplate.weapon[level].buff;
                    // 타워가 받고 있는 버프 레벨 설정
                    weapon.BuffLevel = Level;
                }
            }
        }
    }
    private Transform FindCloseAttackTarget() 
    {
        // 제일 가까이에 있는 적을 찾기위해서 최초 거리를 최대한 크게 설정한다고함
        float closestDistSqr = Mathf.Infinity;
        // 드디어 list가 최초로쓰임 현재 맵에 존재하는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // 일단 거리를 구하고 그거리를 검사한다
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }
        return attackTarget;
    }
    // 간단하게 bool로 바꿔서 원래는 함수를 호출했는데 bool로 확인해서 false일땐 search로 체인지했음
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
        // 아까만들었던 LineRenderer와 HitEffect  레이저효과내는것과 파티클인거같은데 어떻게 하려는건지는 모르겠음
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }
    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }
    // 위에선 그냥 활성화 인거고
    // 여기서 실제로 레이저에 대한 효과를 내는거같은데
    private void SpawnLaser()
    {
        // 여기서 이제 레이어 마스크를 이용해서 뭘하는거지
        // 목표 - 발사위치로 방향을 구한다 맞네 레이저를 쏘려면 방향이 필요하니까 케논도 똑같지않나
        Vector3 direction = attackTarget.position - spawnPoint.position;
        // 모든 것들중에서 어떤것들을 구하려는건가 범위안에있는 모든것들중에 가장가까운걸 찾으려는건가 이자세 은근 집중되네
        // 같은 방향으로 광선을 여러개쏜다 
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        // 맞은것중 현재 attackTarget과 동일한 오브젝트 검출
        for (int i = 0; i < hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                // 검출한것에 뭔가를 하려는데 그냥 데미지만 입히면되는거아닌가 선을 이어야되는구나 조건문에 해당하는 오브젝트와 선을이음
                // 선의 시작점 선의 인덱스 2개중에 0이시작점 1이 끝점인가보네
                lineRenderer.SetPosition(0, spawnPoint.position);
                // 선의 목표지점 // z 값은 따로 빼서 더해줘야되나 안그러면 어떻게 되나본데
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;
                // 적 체력 감소
                // 데미지는 코루틴으로 계속실행되기때문에 1초에 데미지만큼 감소하게 deltatime을 곱해줬음
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    // 여기에서 타워의정보를 관리하다보니
    public bool Upgrade()
    {
        // 골드가 충분한지
        if(playerGold.CurrentGold < towerTemplate.weapon[level].cost)
            return false;

        level++;

        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;
        // 업그레이드할때 레이저인 무기면 linerenderer를 또만져주네 광선의 굵기를 level만큼 올려줌
        if (weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        // 근데 사정거리안에 있는지 항상체크할필요가없겠네  타워는 움직이지 않으니 생각해보니 그럼
        // 버프 갱신만 한번씩해주면됨 공격타워가 업그레이드됐을때랑 타워가 설치됐을때  이 두상황
        // 타워가 업그레이드 될때 모든 버프 타워의 버프 효과 갱신
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
