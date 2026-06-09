using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.3f)
    {
        Debug.Log($"카메라 흔들림 호출됨! (시간: {duration}, 강도: {magnitude})"); 
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        // 🔥 어떤 오브젝트에 스크립트가 붙어있든, 무조건 메인 카메라를 찾아냅니다.
        Transform camTransform = Camera.main.transform;
        
        // 카메라의 원래 위치를 기억합니다.
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // 메인 카메라를 직접 흔듭니다.
            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; 
        }

        // 흔들림이 끝나면 제자리로 돌려놓습니다.
        camTransform.localPosition = originalPos;
    }
}