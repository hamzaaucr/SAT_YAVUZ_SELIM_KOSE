using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("SES AYARLARI (YENİ)")]
    public Slider musicSlider;  // Müzik sesini kıstığımız çubuk
    public Toggle musicToggle;  // "Sessiz" kutucuğu

    [Header("DİĞER AYARLAR (ESKİ)")]
    public TextMeshProUGUI cpuButtonText;
    public Button lowCpuButton;
    public Button resetButton;
    public Button backButton;

    void Start()
    {
        // --- 1. ESKİ AYARLARI YÜKLE ---
        UpdateUI(); // CPU yazısını güncelle

        if (lowCpuButton) lowCpuButton.onClick.AddListener(OnLowCpuClicked);
        if (resetButton) resetButton.onClick.AddListener(OnResetClicked);
        if (backButton) backButton.onClick.AddListener(OnBackClicked);

        // --- 2. YENİ SES AYARLARINI YÜKLE ---
        // Hafızadaki (PlayerPrefs) son ses ayarını çekiyoruz
        float savedVol = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        bool isMuted = PlayerPrefs.GetInt("MusicMute", 0) == 1;

        // Slider varsa ayarla
        if (musicSlider != null)
        {
            musicSlider.value = savedVol; // Çubuğu doğru yere getir
            musicSlider.onValueChanged.AddListener(OnVolumeChanged); // Oynatınca çalışacak fonksiyon
        }

        // Toggle varsa ayarla
        if (musicToggle != null)
        {
            musicToggle.isOn = isMuted; // Tıkı koy veya kaldır
            musicToggle.onValueChanged.AddListener(OnMuteChanged); // Tıklayınca çalışacak fonksiyon
        }
    }

    // --- YENİ SES FONKSİYONLARI ---

    // Slider oynatılınca burası çalışır
    void OnVolumeChanged(float value)
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetVolume(value);
        }
    }

    // Kutucuğa tıklanınca burası çalışır
    void OnMuteChanged(bool isMuted)
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetMute(isMuted);
        }
    }

    // --- ESKİ FONKSİYONLARIN (AYNEN KORUNDU) ---

    void OnLowCpuClicked()
    {
        GameManager.Instance.ToggleLowCpuMode();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance.isLowCpuMode)
        {
            cpuButtonText.text = "Düşük Güç: AÇIK [ON]";
            cpuButtonText.color = Color.green;
        }
        else
        {
            cpuButtonText.text = "Düşük Güç: KAPALI [OFF]";
            cpuButtonText.color = Color.white;
        }
    }

    void OnResetClicked()
    {
        GameManager.Instance.ResetGame();
        Debug.Log("Oyun Sıfırlandı!");
        SceneManager.LoadScene("MenuScene");
    }

    void OnBackClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }
}