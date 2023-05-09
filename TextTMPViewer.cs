using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// 역할이 명확하네 텍스트에 데이터값 get해와서 값저장시킴
public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP; // 플레이어의 체력
    [SerializeField] private TextMeshProUGUI textPlayerGold; // 플레이어의 골드
    [SerializeField] private TextMeshProUGUI textWave; // 웨이브
    [SerializeField] private TextMeshProUGUI textEnemyCount; // 적 수
    [SerializeField] private PlayerHP playerHP; // 실시간 정보
    [SerializeField] private PlayerGold playerGold; // 정보
    [SerializeField] private WaveSystem waveSystem; // 정보
    [SerializeField] private EnemySpawner enemySpawner; // 정보

    void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}
