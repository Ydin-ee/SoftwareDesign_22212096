using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [Header("Reward UI References")]
    [SerializeField] private GameObject rewardPanel; 
    [SerializeField] private Button[] rewardButtons; 
    [SerializeField] private TextMeshProUGUI[] rewardTexts; 

    // 🔥 스킬 진화 전용 UI 변수 추가
    [Header("Skill Evolution UI References")]
    [SerializeField] private GameObject skillEvolutionPanel;
    [SerializeField] private Button[] evolutionButtons;
    [SerializeField] private TextMeshProUGUI[] evolutionTexts;

    private int calculatedHealAmount = 0;
    private const int maxHpIncreaseAmount = 15; 

    private void Start()
    {
        // 시작 시 두 패널 모두 숨김 처리합니다.
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
    }

    // RewardManager.cs의 ShowRewards() 함수를 아래와 같이 수정합니다.
    public void ShowRewards()
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        int currentStage = GameManager.Instance.CurrentStage;

        // 🔥 5스테이지(보스층) 클리어 시: 보스 전용 보상(스킬 해금)
        if (currentStage % 5 == 0)
        {
            Debug.Log("보스 스테이지 클리어! 특별 보상을 제공합니다.");

            // 가운데 버튼만 스킬 해금용으로 씁니다.
            rewardButtons[0].gameObject.SetActive(false); // 왼쪽 숨김
            rewardButtons[2].gameObject.SetActive(false); // 오른쪽 숨김
            
            rewardButtons[1].gameObject.SetActive(true);
            if (rewardTexts[1] != null) rewardTexts[1].text = "✨신규 스킬 해금✨\n[선택 정렬]";
            
            rewardButtons[1].onClick.RemoveAllListeners();
            rewardButtons[1].onClick.AddListener(() => UnlockNewSkill("SelectionSort"));
        }
        // 🔥 일반 스테이지 클리어 시: 기존 보상 그대로 유지
        else
        {
            rewardButtons[0].gameObject.SetActive(true);
            rewardButtons[2].gameObject.SetActive(true);

            if (rewardTexts[0] != null) rewardTexts[0].text = $"최대 체력 +{maxHpIncreaseAmount}";
            rewardButtons[0].onClick.RemoveAllListeners();
            rewardButtons[0].onClick.AddListener(() => SelectReward(0));

            if (player != null && rewardTexts[1] != null)
            {
                float randomPercent = Random.Range(0.20f, 0.50f);
                calculatedHealAmount = Mathf.RoundToInt(player.MaxHP * randomPercent);
                rewardTexts[1].text = $"체력 {calculatedHealAmount} 회복";
            }
            rewardButtons[1].onClick.RemoveAllListeners();
            rewardButtons[1].onClick.AddListener(() => SelectReward(1));

            if (rewardTexts[2] != null) rewardTexts[2].text = "스킬 설정/진화";
            rewardButtons[2].onClick.RemoveAllListeners();
            rewardButtons[2].onClick.AddListener(() => SelectReward(2));
        }
    }

    // 신규 스킬을 해금하고 다음 스테이지로 넘어가는 헬퍼 함수
    private void UnlockNewSkill(string newSkillName)
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null)
        {
            skillManager.UpgradeSkill(newSkillName); // 없는 스킬이면 LV.1로 새로 획득합니다.
        }

        if (rewardPanel != null) rewardPanel.SetActive(false);
        GameManager.Instance.AdvanceToNextStage();
    }

    public void SelectReward(int index)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (index == 0)
        {
            if (player != null) player.IncreaseMaxHP(maxHpIncreaseAmount);
            CloseRewardAndAdvance();
        }
        else if (index == 1)
        {
            if (player != null) player.Heal(calculatedHealAmount);
            CloseRewardAndAdvance();
        }
        else if (index == 2)
        {
            // 🔥 스킬 진화 버튼을 누르면 기본 보상 창을 숨기고 진화 창을 엽니다.
            if (rewardPanel != null) rewardPanel.SetActive(false);
            OpenSkillEvolutionPanel();
        }
    }

    // 기본 보상(체력 등)을 받고 다음 층으로 가는 헬퍼 함수
    private void CloseRewardAndAdvance()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
        GameManager.Instance.AdvanceToNextStage();
    }

    // 🔥 스킬 진화 패널을 열고 버튼을 동적으로 세팅하는 함수
    private void OpenSkillEvolutionPanel()
    {
        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(true);

        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager == null) return;

        List<string> ownedSkills = skillManager.GetOwnedSkills();

        // 버튼 개수(예: 3개)만큼 반복하며 보유한 스킬을 할당합니다.
        for (int i = 0; i < evolutionButtons.Length; i++)
        {
            if (i < ownedSkills.Count)
            {
                evolutionButtons[i].gameObject.SetActive(true); // 버튼 활성화
                
                string skillName = ownedSkills[i];
                int currentLevel = skillManager.GetSkillLevel(skillName);
                
                // "버블 정렬\nLV.1 -> LV.2" 형태로 텍스트 출력
                if (evolutionTexts[i] != null)
                {
                    evolutionTexts[i].text = $"{skillName}\nLV.{currentLevel} -> LV.{currentLevel + 1}";
                }

                evolutionButtons[i].onClick.RemoveAllListeners();
                evolutionButtons[i].onClick.AddListener(() => OnSkillSelectedForEvolution(skillName));
            }
            else
            {
                // 가진 스킬보다 버튼이 많으면 남는 버튼은 숨깁니다.
                evolutionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 🔥 진화할 스킬을 최종 선택했을 때 실행되는 함수
    private void OnSkillSelectedForEvolution(string skillName)
    {
        PlayerSkillManager skillManager = FindFirstObjectByType<PlayerSkillManager>();
        if (skillManager != null) skillManager.UpgradeSkill(skillName);

        if (skillEvolutionPanel != null) skillEvolutionPanel.SetActive(false);
        
        // 진화가 끝나면 다음 스테이지로 진입합니다.
        GameManager.Instance.AdvanceToNextStage();
    }
}