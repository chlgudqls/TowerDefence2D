using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] TowerSpawner towerSpawner;
    [SerializeField] TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null; // ���콺 ��ŷ���� ������ ������Ʈ �ӽ� ����

    private void Awake()
    {
        // ī�޶� ������Ʈ�� �̷������� ������
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // UI�϶� �Ʒ������� �����Ѵ� UI�϶� Ŭ���̰� ���� �����Ѵٴ°� �ٽ��� UI
        // UI�� ���ϵ� ����
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // ��ũ���� ���콺 �������� �Ѱܼ� �������� �ٲ��� ī�޶���� ���콺 �����Ǳ����� ������ ����
            // origin�� direction�� ���� �� ����������
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // ������ �ε��� ������Ʈ hit�� ����  �� ������ �������ִ� �ִ밪�� ����
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // ���⼭ �� �ε����� null�� �ƴϰԵǴµ� null �̰� �ɶ��� ���ؼ�?
                hitTransform = hit.transform;

                if (hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                // ��� �����ϳ� �ߴµ� ������ ������� �±����Ǹ����� �����ϵ���
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(hitTransform == null || !hit.transform.CompareTag("Tower"))
            {
                towerDataViewer.OffPanel();
            }
                hitTransform = null; 
        }
    }
}
