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
        
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.ChangeState(BattleState.Initializing);
        }

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
        // 일반 보상 층이거나 보스 층(5의 배수)일 때 보상 창
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
        // BGM을 멈추고 게임오버 연출음 재생
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
        currentStage = 1;

        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
}