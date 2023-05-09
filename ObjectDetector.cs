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
    private Transform hitTransform = null; // 마우스 픽킹으로 선택한 오브젝트 임시 저장

    private void Awake()
    {
        // 카메라 컴포넌트를 이런식으로 가져옴
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // UI일땐 아래로직을 제외한다 UI일땐 클릭이고 뭐고 무시한다는거 핵심은 UI
        // UI면 뭘하든 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // 스크린의 마우스 포지션을 넘겨서 광선으로 바꿔줌 카메라기준 마우스 포지션까지의 광선을 생성
            // origin과 direction의 값을 다 가지고있음
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 광선에 부딪힌 오브젝트 hit에 저장  이 변수가 가질수있는 최대값의 길이
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 여기서 뭘 부딪히든 null이 아니게되는데 null 이게 될때를 위해서?
                hitTransform = hit.transform;

                if (hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
                // 어떻게 감지하나 했는데 기존의 광선쏘고 태그조건맞으면 실행하도록
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
