using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; // 텍스트가 위로 올라가는 속도
    [SerializeField] private float fadeTime = 1f;   // 사라지는 데 걸리는 시간

    private TextMeshProUGUI textComp;

    // 외부에서 데미지 수치를 전달받아 연출을 시작하는 함수
    public void Setup(int damage)
    {
        textComp = GetComponent<TextMeshProUGUI>();
        textComp.text = $"-{damage}";
        StartCoroutine(FloatingRoutine());
    }

    private IEnumerator FloatingRoutine()
    {
        Color originalColor = textComp.color;
        float timer = 0f;

        while (timer < fadeTime)
        {
            // 위로 이동 (UI 좌표계이므로 생각보다 속도 값이 커야 할 수 있습니다)
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            
            // 서서히 투명해짐 (1 -> 0)
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            textComp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 연출이 끝나면 스스로 오브젝트를 파괴하여 메모리 확보
        Destroy(gameObject);
    }
}