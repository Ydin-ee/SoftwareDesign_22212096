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

        // 🔥 스테이지에 맞는 배경음악 재생 로직 추가
        if (AudioManager.Instance != null)
        {
            if (currentStage % 5 == 0)
                AudioManager.Instance.PlayBGM(BGMType.BossBattle); // 보스층 BGM
            else
                AudioManager.Instance.PlayBGM(BGMType.NormalBattle); // 일반층 BGM
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
        Debug.Log("게임 오버!");
        
        // 🔥 BGM을 멈추고 게임오버 연출음 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(SFXType.GameOver);
        }

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null) uiManager.ShowGameOver();
    }

    public void RestartGameFromFirstStage()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSFX();
        }
        // 1. 스테이지 정보를 1층으로 초기화합니다.
        currentStage = 1;

        // 2. 혹시 멈춰있을지 모르는 전역 시간을 정상화합니다.
        Time.timeScale = 1f;

        // 3. 메인 전투 씬을 깨끗하게 새로 로드합니다.
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
}