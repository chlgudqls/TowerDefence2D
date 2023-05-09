using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    // 이미지는 어떻게 쓰지 레벨을 써줄듯
    [SerializeField] private Image imageTower;
    [SerializeField] private TextMeshProUGUI textDamage;
    [SerializeField] private TextMeshProUGUI textRate;
    [SerializeField] private TextMeshProUGUI textRange;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TextMeshProUGUI textUpgradeCost;
    [SerializeField] private TextMeshProUGUI textSellCost;
    [SerializeField] private TowerAttackRange towerAttackRange;
    [SerializeField] private Button buttonUpgrage;
    [SerializeField] private SystemTextViewer systemTextViewer;

    // 여기서 받아오겠지    
    private TowerWeapon currentTower;

    void Awake()
    {
        OffPanel();
    }

    // 그냥 엔터누르면 페널이 off 오브젝트 디텍터에서 무조건 어떤 키에대한 반응을 하려는건가
    // 쓰임이 맞는 스크립트에서 타워 연동도해주는거임 프로퍼티 받아서 텍스트에 저장
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    // 타워 객체에 붙을스크립트
    public void OnPanel(Transform transform)
    {
        currentTower = transform.GetComponent<TowerWeapon>();
        gameObject.SetActive(true);
        // 패널을 띄울떄 정보도 같이 넣어주는데 길어지니까 함수로 따로뺴서 호출
        // 타워정보 갱신
        UpdateTowerData();

        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }
    public void OffPanel()
    {
        gameObject.SetActive(false);
        towerAttackRange.OffAttackRange();
    }
    private void UpdateTowerData()
    {
        if(currentTower.WeaponType == WeaponType.Cannon || currentTower.WeaponType == WeaponType.Laser)
        {
            // 이미지의 사이즈를 설정
            imageTower.rectTransform.sizeDelta = new Vector2(88, 59);
            textDamage.text = "Damage : " + currentTower.Damage 
                            + "+" + "<color=red>" + currentTower.AddedDamage.ToString("F1") + "</color>";     // f1 이 소숫점1자리 표시네
        }
        else
        {
            imageTower.rectTransform.sizeDelta = new Vector2(59, 59);
            if(currentTower.WeaponType == WeaponType.Slow)
                textDamage.text = "Slow : " + currentTower.Slow * 100 + "%";
            else if (currentTower.WeaponType == WeaponType.Buff)
                textDamage.text = "Buff : " + currentTower.Buff * 100 + "%";
        }
        imageTower.sprite = currentTower.TowerSprite;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;
        textUpgradeCost.text = currentTower.UpgradeCost.ToString();
        textSellCost.text = currentTower.SellCost.ToString();
        //이미지는 어떻게 바꾸려는거지
        //imageTower.sprite = 

        // 타워가 최대레벨이면 버튼 안보이게
        buttonUpgrage.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    // 조건이 맞으면 bool 로직도 실행하고  잘은 모르겠지만 update도 하고 range도 갱신
    public void OnClickEventTowerUpgrate()
    {
        bool isSuccess = currentTower.Upgrade();

        if(isSuccess)
        {
            UpdateTowerData();

            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            systemTextViewer.PrintText(SystemType.Money);
            // 업그레이드 비용 부족 로직 출력
        }
    }
    public void OnClickEventTowerSell()
    {
        currentTower.Sell();
        OffPanel();
    }
}
