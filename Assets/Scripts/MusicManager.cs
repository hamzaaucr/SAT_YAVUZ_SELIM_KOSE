using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private AudioSource audioSource;

    // Ayarlar için anahtarlar (Hata yapmamak için sabitledik)
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string MUSIC_MUTE_KEY = "MusicMute";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // --- KAYITLI AYARLARI YÜKLE ---
        // Daha önce ses ayarý yapýlmýþ mý? Yoksa varsayýlan 0.3 olsun.
        float kayitliSes = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0.3f);

        // Daha önce mute (sessiz) yapýlmýþ mý? (1=Evet, 0=Hayýr)
        bool isMuted = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;

        audioSource.volume = kayitliSes;
        audioSource.mute = isMuted;

        if (!audioSource.isPlaying) audioSource.Play();
    }

    // Slider'dan gelen deðeri uygula
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            // Ayarý hafýzaya kaydet
            PlayerPrefs.SetFloat(MUSIC_VOL_KEY, volume);
        }
    }

    // Toggle'dan gelen deðeri uygula
    public void SetMute(bool isMuted)
    {
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
            // Ayarý hafýzaya kaydet (Bool kaydedilmez, 1 veya 0 olarak kaydederiz)
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, isMuted ? 1 : 0);
        }
    }
}