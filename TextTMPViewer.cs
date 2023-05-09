using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// ������ ��Ȯ�ϳ� �ؽ�Ʈ�� �����Ͱ� get�ؿͼ� �������Ŵ
public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP; // �÷��̾��� ü��
    [SerializeField] private TextMeshProUGUI textPlayerGold; // �÷��̾��� ���
    [SerializeField] private TextMeshProUGUI textWave; // ���̺�
    [SerializeField] private TextMeshProUGUI textEnemyCount; // �� ��
    [SerializeField] private PlayerHP playerHP; // �ǽð� ����
    [SerializeField] private PlayerGold playerGold; // ����
    [SerializeField] private WaveSystem waveSystem; // ����
    [SerializeField] private EnemySpawner enemySpawner; // ����

    void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}
