using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string battleSceneName = "BattleScene"; // 플레이할 메인 전투 씬 이름

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.NormalBattle);
        }
    }

    // [Game Start] 버튼에 연결할 함수
    public void OnStartButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        }

        // 메인 게임 씬으로 전환
        SceneManager.LoadScene(battleSceneName);
    }

    // [Exit] 버튼에 연결할 함수
    public void OnExitButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        }
        Application.Quit(); // 빌드된 게임을 종료하는 명령 (에디터에서는 작동 안 함)
    }
}