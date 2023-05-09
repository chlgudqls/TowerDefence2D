using System.Collections;
using UnityEngine;


// ��������� � ������� ����ߴ��� �ٵ� �� Ŭ�����ۿ��ִ°��� ������
public enum EnemyDestroyType { Kill = 0, Arrive }
public class Enemy : MonoBehaviour
{
    private int wayPointCount; // �̵� ��� ����
    private Transform[] wayPoints; // �̵� ��� ����
    private int currentIndex = 0; // ���� ��ǥ���� �ε���
    private Movement2D movement2D; // ������Ʈ �̵� ����
    private EnemySpawner enemySpawner; // ������ ���������� ������ �ߴµ� EnemySpawner�� �˷��� �����Ѵ�
    [SerializeField] private int gold = 10; // enemy ������Ÿ���ϳ��� ��ũ��Ʈ������ Ÿ�Կ� ���� gold �����Ͱ��� �޶�������

    // �� � ������� �̵���θ� �Ѱܹް� �޸𸮿� �Ű��������� �����ϴ� ������� Ȱ��
    // �� �� ������ �Ű������� ���ؼ� �� ���� ����
    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        // �پ��ִ� �������� movent�� �����´�
        movement2D = GetComponent<Movement2D>();

        this.enemySpawner = enemySpawner;

        // �� �̵� ��� wayPoints ���� ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù��° wayPoint ��ġ�� ����
        transform.position = wayPoints[currentIndex].position;

        // �� �̵�/��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            transform.Rotate(Vector3.forward * 10);

            // �� ��ü�� ��ġ�� �������� ��ġ�Ÿ��� � ������ ��ġ���� ������ nextMove�Լ� ����
            // ���� �����ϴ°��� 
            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                NextMoveTo(); 
            }
            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // ������ �ִٸ� ��ġ�� ����
        if(currentIndex < wayPointCount - 1)
        {
            transform.position = wayPoints[currentIndex].position;
            currentIndex++;
            // �� ��ü������ ������ ���ϴ� ���֛��� ���
            // ���������� ��ü���ֱ⋚���� ���� ��� �ٶ󺸵� ���Ⱚ�� ���Ҽ��ִ� �׸��� �׹����� move�Լ��� �Ѱܼ� �̵��̰����ϴ�
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // ���� ������ ������ �������ȴ� ���������� ���� ���潺������ �⺻���κκ�
        else
        {
            //Destroy(gameObject);
            // �Ű������� ��带 �ޱ⶧���� Arrive ���¿��� ����� enemy�� gold �� 0���γѱ��
            // ���̰� ȣ��Ǹ� ���ʹ� ���� �Լ��� ȣ��Ǵµ� ����ȣ��Ǳ����� kill�������Ѵ� gold ������ 0���� �ٲ��ش�
            // �ʱⰪ�� 10�̱⶧���� kill�Ȱ� �˾Ƽ� 10�� ����
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    // ������ Ÿ��
    public void OnDie(EnemyDestroyType type)
    {
        // �����ʿ��� ����Ʈ�� �����ϱ⋚���� ����Ѵٰ��Ѵ�
        enemySpawner.DestroyEnemy(type,this, gold);
    }
}
