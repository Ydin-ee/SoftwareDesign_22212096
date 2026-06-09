using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; 
    [SerializeField] private float fadeTime = 1f;   

    private TextMeshProUGUI textComp;

    public void Setup(int damage)
    {
        textComp = GetComponent<TextMeshProUGUI>();
        
        if (textComp != null)
        {
            textComp.text = $"-{damage}";
            textComp.color = Color.red; 
            StartCoroutine(FloatingRoutine());
        }
    }

    private IEnumerator FloatingRoutine()
    {
        Color originalColor = textComp.color;
        float timer = 0f;

        while (timer < fadeTime)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            textComp.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            timer += Time.deltaTime;
            yield return null; 
        }

        Destroy(gameObject);
    }
}