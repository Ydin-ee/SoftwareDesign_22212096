using UnityEngine;

public enum SFXType
{
    ButtonClick,    // ⚡ 모든 버튼 클릭음 통합
    PlayerHit,      // 플레이어 피격
    PlayerBuff,     // ⚡ 체력 회복 및 최대 체력 증가 통합 (Heal + StatUp)
    EnemyGimmick,   // 적 방해 공작
    GameOver        // 게임 오버
}

public enum BGMType
{
    NormalBattle,   
    BossBattle      
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip normalBattleBGM;
    [SerializeField] private AudioClip bossBattleBGM;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip playerHitSFX;
    [SerializeField] private AudioClip playerBuffSFX; // ⚡ 통합된 버프 오디오 클립
    [SerializeField] private AudioClip enemyGimmickSFX;
    [SerializeField] private AudioClip gameOverSFX;

    private float[] lastPlayTimes;
    private const float SFX_COOLDOWN = 0.08f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            lastPlayTimes = new float[System.Enum.GetValues(typeof(SFXType)).Length];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(BGMType type)
    {
        if (bgmSource == null) return;

        AudioClip targetClip = null;
        switch (type)
        {
            case BGMType.NormalBattle: targetClip = normalBattleBGM; break;
            case BGMType.BossBattle: targetClip = bossBattleBGM; break;
        }

        if (bgmSource.clip == targetClip)
        {
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
            return;
        }

        bgmSource.clip = targetClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }
    public void StopAllSFX()
    {
        if (sfxSource != null) 
        {
            sfxSource.Stop(); // 현재 SFX 오디오 소스에서 재생 중인 모든 소리를 즉시 멈춥니다.
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxSource == null) return;

        int sfxIndex = (int)type;
        if (Time.time - lastPlayTimes[sfxIndex] < SFX_COOLDOWN) return;

        AudioClip targetClip = null;
        switch (type)
        {
            case SFXType.ButtonClick: targetClip = buttonClickSFX; break;
            case SFXType.PlayerHit: targetClip = playerHitSFX; break;
            case SFXType.PlayerBuff: targetClip = playerBuffSFX; break;
            case SFXType.EnemyGimmick: targetClip = enemyGimmickSFX; break;
            case SFXType.GameOver: targetClip = gameOverSFX; break;
        }

        if (targetClip != null)
        {
            lastPlayTimes[sfxIndex] = Time.time;
            sfxSource.PlayOneShot(targetClip);
        }
    }
}