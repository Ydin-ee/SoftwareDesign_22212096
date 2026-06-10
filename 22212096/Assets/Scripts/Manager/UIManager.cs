using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    // --- 스킬 스크립트 연결 ---
    [SerializeField] private SortingSkill bubbleSortSkill;
    [SerializeField] private DataStructureSkill stackShieldSkill;
    [SerializeField] private QueueShieldSkill queueShieldSkill;
    [SerializeField] private SelectionSortSkill selectionSortSkill;
    [SerializeField] private InsertionSortSkill insertionSortSkill;

    // --- HP UI ---
    [Header("HP UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpFillImage; 
    [SerializeField] private Color highHPColor = Color.green;
    [SerializeField] private Color mediumHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;

    // --- 서브 메뉴 UI ---
    [Header("Sort Sub-Menu UI")]
    [SerializeField] private GameObject sortSubMenuPanel;
    [SerializeField] private Button[] sortSkillButtons;   
    [SerializeField] private TextMeshProUGUI[] sortSkillTexts;
    
    [Header("Shield Sub-Menu UI")]
    [SerializeField] private GameObject shieldSubMenuPanel;

    // --- 알림 UI ---
    [Header("Warning UI")]
    [SerializeField] private GameObject warningTextObject; 
    [SerializeField] private TextMeshProUGUI warningText;  

    // --- 게임 오버 UI ---
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;


    private void Start()
    {
        // 게임 시작 시 화면에 보이면 안 되는 UI들을 확실하게 끕니다.
        if (sortSubMenuPanel != null) sortSubMenuPanel.SetActive(false);
        if (warningTextObject != null) warningTextObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);
    }

    public void UpdateHPBar(int currentHP, int maxHP)
    {
        float ratio = (float)currentHP / maxHP;
        
        if (hpSlider != null) hpSlider.value = ratio;
        if (hpText != null) hpText.text = $"HP: {currentHP}/{maxHP}";

        if (hpFillImage != null)
        {
            if (ratio > 0.5f) hpFillImage.color = highHPColor;
            else if (ratio > 0.2f) hpFillImage.color = mediumHPColor;
            else hpFillImage.color = lowHPColor;
        }
    }

    // --- 스킬 조작 ---
    public void OnMainSortButtonClicked()
    {
        // ⚡ 현재 입력이 잠긴 대기 시간 상태라면 버튼 작동을 무시합니다.
        if (battleManager != null && battleManager.IsInputBlocked)
        {
            Debug.LogWarning("아직 화면 정렬 중이거나 대기 시간입니다. 조작할 수 없습니다.");
            return; 
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        bool isActive = sortSubMenuPanel.activeSelf;
        sortSubMenuPanel.SetActive(!isActive);
        if (!isActive && shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);
        
        if (!isActive) UpdateSortSubMenu();
        
    }

    public void OnMainShieldButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked)
        {
            Debug.LogWarning("아직 화면 정렬 중이거나 대기 시간입니다. 조작할 수 없습니다.");
            return; 
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        bool isActive = shieldSubMenuPanel.activeSelf;
        shieldSubMenuPanel.SetActive(!isActive);

        // 🔥 방어 메뉴를 열 때 정렬 메뉴가 켜져 있다면 닫아줍니다.
        if (!isActive && sortSubMenuPanel != null) sortSubMenuPanel.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        // ⚡ 일반 버튼 클릭음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        // 열려있는 모든 서브 메뉴를 강제로 닫습니다.
        if (sortSubMenuPanel != null) sortSubMenuPanel.SetActive(false);
        if (shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);
    }

    private void UpdateSortSubMenu()
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager == null) return;

        List<string> equippedSkills = skillManager.equippedSortSkills;

        for (int i = 0; i < sortSkillButtons.Length; i++)
        {
            if (i < equippedSkills.Count)
            {
                sortSkillButtons[i].gameObject.SetActive(true);
                string skillName = equippedSkills[i];
                
                // 🔥 삽입 정렬 한글 텍스트 인식 로직 추가
                if (sortSkillTexts[i] != null)
                {
                    if (skillName == "BubbleSort") sortSkillTexts[i].text = "버블 정렬";
                    else if (skillName == "SelectionSort") sortSkillTexts[i].text = "선택 정렬";
                    else if (skillName == "InsertionSort") sortSkillTexts[i].text = "삽입 정렬";
                    else sortSkillTexts[i].text = skillName;
                }

                sortSkillButtons[i].onClick.RemoveAllListeners();
                sortSkillButtons[i].onClick.AddListener(() => ExecuteSortSkill(skillName));
            }
            else
            {
                sortSkillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ExecuteSortSkill(string skillName)
    {
        sortSubMenuPanel.SetActive(false); 

        // ⚡ 일반 버튼 클릭 사운드로 통일
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (skillName == "BubbleSort")
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
            if (player != null && skillManager != null)
            {
                int level = skillManager.GetSkillLevel("BubbleSort");
                player.AddDamageReduction(level * 3);
            }
            ShowWarning($"버블 정렬 발동!\n피해 감소 버프 획득!", 1.5f);

            battleManager.OnPlayerSkillSelected(bubbleSortSkill);
        }
        else if (skillName == "SelectionSort")
        {
            if (selectionSortSkill != null) selectionSortSkill.ActivateSkill();
        }
        // 🔥 삽입 정렬 발동 조건 추가
        else if (skillName == "InsertionSort")
        {
            if (insertionSortSkill != null) insertionSortSkill.ActivateSkill();
        }
    }

    public void OnStackShieldButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked) return;

        // 🔥 스킬을 선택하면 서브 메뉴를 닫습니다.
        if (shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (stackShieldSkill != null)
        {
            stackShieldSkill.ExecuteShieldSkill(); 
            battleManager.ChangeState(BattleState.EnemyTurn); 
        }
    }

    public void OnQueueShieldButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked) return;

        // 🔥 스킬을 선택하면 서브 메뉴를 닫습니다.
        if (shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (queueShieldSkill != null)
        {
            queueShieldSkill.ExecuteShieldSkill(); 
            battleManager.ChangeState(BattleState.EnemyTurn); 
        }
    }

    // --- 시각적 피드백 ---
    public void ShowWarning(string message, float duration = 1.5f)
    {
        if (warningTextObject != null && warningText != null)
        {
            warningText.text = message;
            warningTextObject.SetActive(true); 
            StopAllCoroutines(); 
            StartCoroutine(HideWarningRoutine(duration));
        }
    }

    private System.Collections.IEnumerator HideWarningRoutine(float duration)
{
    RectTransform textRect = warningTextObject.GetComponent<RectTransform>();
    Vector3 startPos = textRect.anchoredPosition;
    Vector3 targetPos = startPos + new Vector3(0, 30f, 0); // 위로 50픽셀 이동

    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        // 부드럽게 위로 이동
        textRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
        yield return null;
    }

    warningTextObject.SetActive(false);
    textRect.anchoredPosition = startPos; // 원래 위치로 복구
}

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}