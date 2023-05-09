using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    // �̹����� ��� ���� ������ ���ٵ�
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

    // ���⼭ �޾ƿ�����    
    private TowerWeapon currentTower;

    void Awake()
    {
        OffPanel();
    }

    // �׳� ���ʹ����� ����� off ������Ʈ �����Ϳ��� ������ � Ű������ ������ �Ϸ��°ǰ�
    // ������ �´� ��ũ��Ʈ���� Ÿ�� ���������ִ°��� ������Ƽ �޾Ƽ� �ؽ�Ʈ�� ����
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    // Ÿ�� ��ü�� ������ũ��Ʈ
    public void OnPanel(Transform transform)
    {
        currentTower = transform.GetComponent<TowerWeapon>();
        gameObject.SetActive(true);
        // �г��� �� ������ ���� �־��ִµ� ������ϱ� �Լ��� ���Ε��� ȣ��
        // Ÿ������ ����
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
            // �̹����� ����� ����
            imageTower.rectTransform.sizeDelta = new Vector2(88, 59);
            textDamage.text = "Damage : " + currentTower.Damage 
                            + "+" + "<color=red>" + currentTower.AddedDamage.ToString("F1") + "</color>";     // f1 �� �Ҽ���1�ڸ� ǥ�ó�
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
        //�̹����� ��� �ٲٷ��°���
        //imageTower.sprite = 

        // Ÿ���� �ִ뷹���̸� ��ư �Ⱥ��̰�
        buttonUpgrage.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    // ������ ������ bool ������ �����ϰ�  ���� �𸣰����� update�� �ϰ� range�� ����
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
            // ���׷��̵� ��� ���� ���� ���
        }
    }
    public void OnClickEventTowerSell()
    {
        currentTower.Sell();
        OffPanel();
    }
}
