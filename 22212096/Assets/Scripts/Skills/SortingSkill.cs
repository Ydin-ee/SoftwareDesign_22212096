using UnityEngine;
using System.Collections;

public class SortingSkill : SkillBase
{
    private float damageMultiplier = 1.5f;
    [SerializeField] private ArrayVisualizer arrayVisualizer;

    private void Start()
    {
        skillName = "Bubble Sort";
        cooldown = 2;
    }

    public override void Execute(ref EnemyData target)
    {
        Debug.Log($"{skillName} 스킬 발동!");
        int[] targetArray = target.arrayData;
        StartCoroutine(ExecuteSkillRoutine(targetArray));
    }

    private IEnumerator ExecuteSkillRoutine(int[] arr)
    {
        int swapIndexA = -1;
        int swapIndexB = -1;

        for(int i =0; i< arr.Length -1; i++)
        {
            if(arr[i] > arr[i + 1])
            {
                swapIndexA = i;
                swapIndexB = i+1;

                int temp = arr[i];
                arr[i] = arr[i+1];
                arr[i+1] = temp;

                Debug.Log($"[로직] 데이터 스왑 발생: {arr[swapIndexB]} <-> {arr[swapIndexA]}");
                break;
            }
        }

        if (swapIndexA != -1 && arrayVisualizer != null)
        {
            Debug.Log("[연출] 스왑 애니메이션 재생 시작...");
            yield return StartCoroutine(arrayVisualizer.PlaySwapAnimation(swapIndexA, swapIndexB));
            Debug.Log("[연출] 스왑 애니메이션 재생 완료!");
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if(battleManager != null)
        {
            battleManager.ChangeState(BattleState.EvaluateVictory);
        }

    }
}
