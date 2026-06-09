using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    [Header("Stage Settings")]
    [SerializeField] private int rewardInterval = 3; 
    private int currentStage = 1;

    // 🔥 에러의 원인이었던 부분! 외부에서 현재 스테이지를 읽을 수 있게 합니다.
    public int CurrentStage { get { return currentStage; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        InitGame();
    }

    public void InitGame()
    {
        Debug.Log($"--- 스테이지 {currentStage} 시작 ---");
        
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.ChangeState(BattleState.Initializing);
        }

        // 🔥 매 스테이지가 시작될 때마다 해금된 스킬(선택 정렬 등) 버튼을 갱신합니다.
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateSkillUI();
        }
    }

    public void OnStageCleared()
    {
        Debug.Log($"스테이지 {currentStage} 클리어 성공!");

        // 일반 보상 층이거나 보스 층(5의 배수)일 때 보상 창을 띄웁니다.
        if (currentStage % rewardInterval == 0 || currentStage % 5 == 0)
        {
            RewardManager rewardManager = FindFirstObjectByType<RewardManager>();
            if (rewardManager != null)
            {
                rewardManager.ShowRewards();
            }
        }
        else
        {
            // 보상 층이 아니면 바로 다음 스테이지로 진행합니다.
            AdvanceToNextStage();
        }
    }

    public void AdvanceToNextStage()
    {
        currentStage++;
        InitGame();
    }

    public void GameOver()
    {
        Debug.Log("게임 종료!");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}