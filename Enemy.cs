using System.Collections;
using UnityEngine;


// 사망했을때 어떤 방식으로 사망했는지 근데 왜 클래스밖에있는거지 열거형
public enum EnemyDestroyType { Kill = 0, Arrive }
public class Enemy : MonoBehaviour
{
    private int wayPointCount; // 이동 경로 개수
    private Transform[] wayPoints; // 이동 경로 정보
    private int currentIndex = 0; // 현재 목표지점 인덱스
    private Movement2D movement2D; // 오브젝트 이동 제어
    private EnemySpawner enemySpawner; // 삭제를 종착역에서 본인이 했는데 EnemySpawner에 알려서 삭제한다
    [SerializeField] private int gold = 10; // enemy 프리팹타입하나에 스크립트가붙음 타입에 따라 gold 데이터값이 달라질거임

    // ※ 어떤 방식으로 이동경로를 넘겨받고 메모리에 매개변수값을 저장하는 방식으로 활용
    // ※ 빈 변수가 매개변수로 인해서 새 값을 받음
    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        // 붙어있는 프리팹의 movent를 가져온다
        movement2D = GetComponent<Movement2D>();

        this.enemySpawner = enemySpawner;

        // 적 이동 경로 wayPoints 정보 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫번째 wayPoint 위치로 설정
        transform.position = wayPoints[currentIndex].position;

        // 적 이동/목표지점 설정 코루틴 함수 시작
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            transform.Rotate(Vector3.forward * 10);

            // 현 객체의 위치와 현지점의 위치거리가 어떤 지정한 수치보다 작으면 nextMove함수 실행
            // 새로 생성하는거임 
            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                NextMoveTo(); 
            }
            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // 남은게 있다면 위치로 보냄
        if(currentIndex < wayPointCount - 1)
        {
            transform.position = wayPoints[currentIndex].position;
            currentIndex++;
            // 두 객체값으로 방향을 구하는 자주썻던 방법
            // 꼭짓점마다 객체가있기떄문에 적이 어딜 바라보든 방향값을 구할수있다 그리고 그방향을 move함수로 넘겨서 이동이가능하다
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // 다음 지점이 없으니 삭제가된다 종착지에서 삭제 디펜스게임의 기본적인부분
        else
        {
            //Destroy(gameObject);
            // 매개변수로 골드를 받기때문에 Arrive 상태에서 사라진 enemy는 gold 를 0으로넘긴다
            // 다이가 호출되면 에너미 삭제 함수가 호출되는데 다이호출되기전에 kill되지못한다 gold 변수를 0으로 바꿔준다
            // 초기값이 10이기때문에 kill된건 알아서 10이 들어간다
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    // 죽음의 타입
    public void OnDie(EnemyDestroyType type)
    {
        // 스포너에서 리스트를 관리하기떄문에 사용한다고한다
        enemySpawner.DestroyEnemy(type,this, gold);
    }
}
