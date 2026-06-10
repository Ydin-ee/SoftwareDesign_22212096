using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    [Header("Skill References")]
    [SerializeField] private SortingSkill bubbleSortSkill;
    [SerializeField] private DataStructureSkill stackShieldSkill;
    [SerializeField] private QueueShieldSkill queueShieldSkill;
    [SerializeField] private SelectionSortSkill selectionSortSkill;
    [SerializeField] private InsertionSortSkill insertionSortSkill;

    [Header("HP UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpFillImage; 
    [SerializeField] private Color highHPColor = Color.green;
    [SerializeField] private Color mediumHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;

    [Header("Sub-Menu UI")]
    [SerializeField] private GameObject sortSubMenuPanel;
    [SerializeField] private Button[] sortSkillButtons;   
    [SerializeField] private TextMeshProUGUI[] sortSkillTexts;
    [SerializeField] private GameObject shieldSubMenuPanel;

    [Header("Warning & Game Over UI")]
    [SerializeField] private GameObject warningTextObject; 
    [SerializeField] private TextMeshProUGUI warningText;  
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
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

    public void OnMainSortButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        bool isActive = sortSubMenuPanel.activeSelf;
        sortSubMenuPanel.SetActive(!isActive);
        if (!isActive && shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);
        
        if (!isActive) UpdateSortSubMenu();
    }

    public void OnMainShieldButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        bool isActive = shieldSubMenuPanel.activeSelf;
        shieldSubMenuPanel.SetActive(!isActive);
        if (!isActive && sortSubMenuPanel != null) sortSubMenuPanel.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);
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
            if (bubbleSortSkill != null) bubbleSortSkill.ActivateSkill();
        }
        else if (skillName == "SelectionSort")
        {
            if (selectionSortSkill != null) selectionSortSkill.ActivateSkill();
        }
        else if (skillName == "InsertionSort")
        {
            if (insertionSortSkill != null) insertionSortSkill.ActivateSkill();
        }
    }

    public void OnStackShieldButtonClicked()
    {
        if (battleManager != null && battleManager.IsInputBlocked) return;
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
        if (shieldSubMenuPanel != null) shieldSubMenuPanel.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (queueShieldSkill != null)
        {
            queueShieldSkill.ExecuteShieldSkill(); 
            battleManager.ChangeState(BattleState.EnemyTurn); 
        }
    }

    // --- 카메라 흔들림 통합 로직 ---
    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.3f)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private System.Collections.IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Transform camTransform = Camera.main.transform;
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null; 
        }
        camTransform.localPosition = originalPos;
    }

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
        Vector3 targetPos = startPos + new Vector3(0, 30f, 0); 

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        warningTextObject.SetActive(false);
        textRect.anchoredPosition = startPos; 
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
}