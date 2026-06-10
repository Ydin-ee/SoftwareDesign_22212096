using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string battleSceneName = "BattleScene"; // 플레이할 메인 전투 씬 이름

    private void Start()
    {
        // 타이틀 화면에 진입했을 때 잔잔한 일반 배틀 BGM을 미리 깔아줍니다.
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

        // 메인 게임 씬으로 전환합니다.
        SceneManager.LoadScene(battleSceneName);
    }

    // [Exit] 버튼에 연결할 함수
    public void OnExitButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        }

        Debug.Log("게임이 종료되었습니다.");
        Application.Quit(); // 빌드된 게임을 종료하는 명령 (에디터에서는 작동 안 함)
    }
}