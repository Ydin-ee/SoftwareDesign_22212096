using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private SortingSkill bubbleSortSkill;

    [SerializeField] private DataStructureSkill stackShieldSkill;

    [Header("HP UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image hpFillImage; 

    [SerializeField] private Color highHPColor = Color.green;
    [SerializeField] private Color mediumHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;

    // 🔥 플로팅 텍스트 관련 변수 추가
    [Header("Damage Text Settings")]
    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private Transform damageTextParent; // Canvas를 연결할 예정

    [Header("Skill References")]
    [SerializeField] private SelectionSortSkill selectionSortSkill;
    [Header("Sort Sub-Menu UI")]
    [SerializeField] private GameObject sortSubMenuPanel; // 하위 스킬을 띄울 팝업 패널
    [SerializeField] private Button[] sortSkillButtons;   // 패널 안의 스킬 버튼 3개
    [SerializeField] private TextMeshProUGUI[] sortSkillTexts;
    

    [Header("Warning UI")]
    [SerializeField] private GameObject warningTextObject; // 껐다 켤 텍스트 오브젝트
    [SerializeField] private TextMeshProUGUI warningText;  // 글씨를 바꿀 컴포넌트

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    public void OnMainSortButtonClicked()
    {
        // 서브 메뉴가 꺼져있으면 켜고, 켜져있으면 끕니다 (토글 기능)
        bool isActive = sortSubMenuPanel.activeSelf;
        sortSubMenuPanel.SetActive(!isActive);

        if (!isActive)
        {
            UpdateSortSubMenu();
        }
    }
    // 장착 중인 정렬 스킬을 읽어와서 서브 메뉴 버튼에 할당하는 함수
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
                
                // 이름 변환 (BubbleSort -> 버블 정렬)
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
                // 장착된 스킬이 없으면 남는 버튼은 숨깁니다.
                sortSkillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 서브 메뉴에서 실제 스킬을 클릭했을 때 실행되는 함수
    private void ExecuteSortSkill(string skillName)
    {
        // 스킬을 골랐으니 서브 메뉴 창은 다시 닫아줍니다.
        sortSubMenuPanel.SetActive(false); 

        if (skillName == "BubbleSort")
        {
            // 이전에 만들어둔 버블 정렬 데미지 감소 등의 로직을 그대로 재사용합니다.
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
            // 방금 만든 선택 정렬 대기 상태로 진입합니다.
            if (selectionSortSkill != null) selectionSortSkill.ActivateSkill();
        }
    }


    public void OnBubbleSortButtonClicked()
    {
        Debug.Log("UI 버튼 클릭됨: 버블 정렬 스킬 발동");

        // 1. 플레이어와 스킬 매니저를 찾습니다.
        PlayerController player = FindFirstObjectByType<PlayerController>();
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();

        if (player != null && skillManager != null)
        {
            // 2. 버블 정렬 스킬의 현재 레벨을 가져옵니다.
            int level = skillManager.GetSkillLevel("BubbleSort");
            
            // 3. 기획하신 계수에 따라 데미지 감소량을 설정합니다. (예: 1레벨 3, 2레벨 6, 3레벨 9 감소)
            int reductionAmount = level * 3; 
            
            player.AddDamageReduction(reductionAmount);
        }

        // 기존의 배열 정렬 스킬 연동 로직 실행
        battleManager.OnPlayerSkillSelected(bubbleSortSkill);
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

    // 🔥 데미지 텍스트 생성 함수 추가
    public void SpawnDamageText(int damage)
    {
        if (damageTextPrefab != null && damageTextParent != null)
        {
            DamageText textObj = Instantiate(damageTextPrefab, damageTextParent);
            
            // 화면 중앙에서 살짝 랜덤하게 퍼지도록 위치 조정 (UI 앵커 기준)
            Vector3 randomOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            textObj.transform.localPosition = randomOffset;
            
            textObj.Setup(damage);
        }
    }
    // UIManager.cs 내부의 함수를 아래와 같이 교체합니다.
    public void OnStackShieldButtonClicked()
    {
        Debug.Log("UI 버튼 클릭됨: 스택 방패 스킬 발동!");
        
        if (stackShieldSkill != null)
        {
            // 1. 방패 스킬 즉시 발동 (스택 획득)
            stackShieldSkill.ExecuteShieldSkill(); 
            
            // 2. 방어 행동을 했으므로 즉시 적의 턴으로 넘깁니다.
            battleManager.ChangeState(BattleState.EnemyTurn); 
        }
    }
    // UIManager.cs의 상단 변수 선언부에 선택 정렬 UI 요소 추가
    [Header("Skill Button Objects")]
    [SerializeField] private GameObject selectionSortButtonObj; // 선택 정렬 버튼 자체 (켜고 끄기 위함)

    // GameManager에서 InitGame()을 호출하거나 전투가 시작될 때, 
    // 혹은 UIManager의 Start()에서 이 함수를 실행하도록 연결해 줍니다.
    private void Start()
    {
        // 게임 시작 시 서브 메뉴 패널이 켜져 있다면 무조건 숨깁니다.
        if (sortSubMenuPanel != null)
        {
            sortSubMenuPanel.SetActive(false);
        }
    }
    public void UpdateSkillUI()
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null && selectionSortButtonObj != null)
        {
            // SelectionSort 스킬 레벨이 1 이상이면(가져본 적 있으면) 버튼을 켭니다.
            bool hasSelectionSort = skillManager.GetSkillLevel("SelectionSort") > 0;
            selectionSortButtonObj.SetActive(hasSelectionSort);
        }
    }

    public void OnSelectionSortButtonClicked()
    {
        Debug.Log("UI 버튼 클릭됨: 선택 정렬 스킬 선택");

        if (selectionSortSkill != null)
        {
            selectionSortSkill.ActivateSkill();
        }
    }

    public void ShowWarning(string message, float duration = 1.5f)
    {
        if (warningTextObject != null && warningText != null)
        {
            warningText.text = message;
            warningTextObject.SetActive(true); // 텍스트 켜기
            
            // 기존에 돌고 있던 코루틴이 있다면 멈추고 새로 시작 (연속으로 뜰 때 꼬임 방지)
            StopAllCoroutines(); 
            StartCoroutine(HideWarningRoutine(duration));
        }
    }

    private System.Collections.IEnumerator HideWarningRoutine(float duration)
    {
        // 지정된 시간만큼 기다린 후
        yield return new WaitForSeconds(duration);
        
        // 텍스트를 다시 숨깁니다.
        if (warningTextObject != null)
        {
            warningTextObject.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}