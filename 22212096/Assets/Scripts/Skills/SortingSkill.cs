using UnityEngine;
using System.Collections;

public class SortingSkill : MonoBehaviour
{
    [SerializeField] private ArrayVisualizer arrayVisualizer;

    public void ActivateSkill()
    {
        BattleManager battleManager = FindFirstObjectByType<BattleManager>();
        if (battleManager == null) return;

        int[] targetArray = battleManager.currentEnemy.GetArray();
        StartCoroutine(ExecuteSkillRoutine(targetArray, battleManager));
    }

    private IEnumerator ExecuteSkillRoutine(int[] arr, BattleManager battleManager)
    {
        int swapIndexA = -1;
        int swapIndexB = -1;

        for (int i = 0; i < arr.Length - 1; i++)
        {
            if (arr[i] > arr[i + 1])
            {
                swapIndexA = i;
                swapIndexB = i + 1;
                int temp = arr[i];
                arr[i] = arr[i + 1];
                arr[i + 1] = temp;
                break;
            }
        }

        if (swapIndexA != -1 && arrayVisualizer != null)
        {
            yield return StartCoroutine(arrayVisualizer.PlaySwapAnimation(swapIndexA, swapIndexB));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (battleManager != null) battleManager.ChangeState(BattleState.EvaluateVictory);
    }
}