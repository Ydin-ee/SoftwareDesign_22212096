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
    [SerializeField] private SelectionSortSkill selectionSortSkill;

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
        
        // 🔥 게임 오버 패널은 시작할 때 꺼지는 것이 "정상"입니다!
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
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
        bool isActive = sortSubMenuPanel.activeSelf;
        sortSubMenuPanel.SetActive(!isActive);

        if (!isActive)
        {
            UpdateSortSubMenu();
        }
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
                
                if (sortSkillTexts[i] != null)
                {
                    sortSkillTexts[i].text = skillName == "BubbleSort" ? "버블 정렬" : 
                                             skillName == "SelectionSort" ? "선택 정렬" : skillName;
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

        if (skillName == "BubbleSort")
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
            if (player != null && skillManager != null)
            {
                int level = skillManager.GetSkillLevel("BubbleSort");
                player.AddDamageReduction(level * 3);
            }
            battleManager.OnPlayerSkillSelected(bubbleSortSkill);
        }
        else if (skillName == "SelectionSort")
        {
            if (selectionSortSkill != null) selectionSortSkill.ActivateSkill();
        }
    }

    public void OnStackShieldButtonClicked()
    {
        if (stackShieldSkill != null)
        {
            stackShieldSkill.ExecuteShieldSkill(); 
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
        yield return new WaitForSeconds(duration);
        if (warningTextObject != null) warningTextObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}