using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 조작을 위해 필요

public class EnemyVisualsManager : MonoBehaviour
{
    public static EnemyVisualsManager Instance { get; private set; } // 싱글톤

    [Header("Enemy Sprite Components")]
    [SerializeField] private Image enemyImageComponent; // 화면에 보이는 Image 컴포넌트 연결

    [Header("Slime Sprites (Pixel Art)")]
    // Kenney나 다른 곳에서 구한 슬라임 픽셀 이미지 3개를 여기에 연결합니다.
    [SerializeField] private Sprite greenSlime; // 1~5층
    [SerializeField] private Sprite blueSlime;  // 6~10층
    [SerializeField] private Sprite redSlime;   // 11층 이상

    private void Awake()
    {
        // 간단한 싱글톤 세팅
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Image 컴포넌트가 자동으로 연결 안 됐다면 찾아봅니다.
        if (enemyImageComponent == null)
        {
            enemyImageComponent = GetComponentInChildren<Image>();
        }
    }

    // 🔥 BattleManager나 GameManager에서 전투가 세팅될 때 이 함수를 호출합니다.
    public void UpdateEnemyVisualsByStage(int currentStage)
    {
        if (enemyImageComponent == null) return;

        // user님의 기획안 그대로 반영
        if (currentStage >= 1 && currentStage <= 5)
        {
            SetSprite(greenSlime);
            Debug.Log($"[EnemyVisuals] 스테이지 {currentStage}: 초록 슬라임 등장");
        }
        else if (currentStage >= 6 && currentStage <= 10)
        {
            SetSprite(blueSlime);
            Debug.Log($"[EnemyVisuals] 스테이지 {currentStage}: 파랑 슬라임 등장");
        }
        else if (currentStage >= 11)
        {
            SetSprite(redSlime);
            Debug.Log($"[EnemyVisuals] 스테이지 {currentStage}: 빨강 슬라임 등장 (최종 구역)");
        }
    }

    private void SetSprite(Sprite newSprite)
    {
        if (newSprite == null)
        {
            Debug.LogWarning("[EnemyVisuals] 연결된 슬라임 스프라이트가 없습니다! 플레이스홀더를 사용합니다.");
            // 팁: 이미지가 비었을 때를 대비해 흰색 네모를 기본으로 꽂아두거나, 에러 스프라이트를 꽂아둘 수 있습니다.
            // enemyImageComponent.sprite = null; return;
        }
        
        enemyImageComponent.sprite = newSprite;
        // 팁: 이미지 크기가 제각각일 경우 native size로 맞춰주는 연출을 넣을 수 있습니다.
        // enemyImageComponent.SetNativeSize();
    }
}