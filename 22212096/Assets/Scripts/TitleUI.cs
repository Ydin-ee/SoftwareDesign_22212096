using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string battleSceneName = "BattleScene"; 

    [Header("UI Panels")]
    [SerializeField] private GameObject helpPanel;         // 튜토리얼 창
    [SerializeField] private GameObject exitConfirmPanel;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGMType.TitleBGM);
        }

        if (helpPanel != null) helpPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
    }

    // --- 메인 버튼 로직 ---
    public void OnStartButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneManager.LoadScene(battleSceneName);
    }

    public void OnExitButtonClicked()
    {
        // 종료 확인 창
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(true);
    }

    // --- 종료 확인 팝업 로직 ---
    public void OnConfirmExitClicked()
    {
        // "예(종료)" 버튼을 눌렀을 때
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        Debug.Log("게임을 종료합니다.");
        Application.Quit(); 
    }

    public void OnCancelExitClicked()
    {
        // "아니오(취소)" 버튼을 눌렀을 때
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
    }

    // --- 튜토리얼(도움말) 팝업 로직 ---
    public void OnHelpButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (helpPanel != null) helpPanel.SetActive(true);
    }

    public void OnCloseHelpButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (helpPanel != null) helpPanel.SetActive(false);
    }
}