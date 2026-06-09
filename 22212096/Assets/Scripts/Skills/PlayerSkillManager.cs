using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    // 스킬의 고유 이름과 현재 레벨을 짝지어 저장하는 딕셔너리입니다.
    private Dictionary<string, int> skillLevels = new Dictionary<string, int>();
    public List<string> equippedSortSkills = new List<string>();

    private void Start()
    {
        skillLevels.Add("BubbleSort", 1);
        skillLevels.Add("StackShield", 1);
        
        // 시작할 때 버블 정렬을 1번 슬롯에 장착합니다.
        equippedSortSkills.Add("BubbleSort"); 
    }
    
    // 현재 보유 중인 스킬 이름들의 목록을 List 형태로 반환합니다.
    public List<string> GetOwnedSkills()
    {
        return new List<string>(skillLevels.Keys);
    }

    // 외부에서 특정 스킬의 레벨이 궁금할 때 물어보는 함수
    public int GetSkillLevel(string skillName)
    {
        if (skillLevels.ContainsKey(skillName)) 
            return skillLevels[skillName];
        
        return 0; // 스킬이 없으면 0 반환
    }

    // 스킬의 레벨을 1 올리거나 새로 획득하는 함수
    public void UpgradeSkill(string skillName)
    {
        if (skillLevels.ContainsKey(skillName))
        {
            skillLevels[skillName]++;
            Debug.Log($"[스킬 진화] {skillName} 스킬이 LV.{skillLevels[skillName]} (으)로 강화되었습니다!");
        }
        else
        {
            skillLevels.Add(skillName, 1);
            Debug.Log($"[스킬 해금] {skillName} 스킬을 새로 획득했습니다!");
        }
    }

    // 임시 테스트용: 현재 가진 스킬 중 하나를 무작위로 골라 강화합니다.
    public void UpgradeRandomOwnedSkill()
    {
        List<string> ownedSkills = new List<string>(skillLevels.Keys);
        if (ownedSkills.Count > 0)
        {
            int randomIndex = Random.Range(0, ownedSkills.Count);
            UpgradeSkill(ownedSkills[randomIndex]);
        }
    }
    
}