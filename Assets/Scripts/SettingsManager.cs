using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI cpuButtonText; // Butonun üzerindeki yazı
    public Button lowCpuButton;
    public Button resetButton;
    public Button backButton;

    void Start()
    {
        // Sahne açılınca mevcut ayara göre yazıyı güncelle
        UpdateUI();

        // Butonlara görevlerini ver
        lowCpuButton.onClick.AddListener(OnLowCpuClicked);
        resetButton.onClick.AddListener(OnResetClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnLowCpuClicked()
    {
        // GameManager'daki ayarı değiştir
        GameManager.Instance.ToggleLowCpuMode();

        // Ekrandaki yazıyı güncelle
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance.isLowCpuMode)
        {
            cpuButtonText.text = "Düşük Güç: AÇIK 🔋";
            cpuButtonText.color = Color.green;
        }
        else
        {
            cpuButtonText.text = "Düşük Güç: KAPALI 🚀";
            cpuButtonText.color = Color.white;
        }
    }

    void OnResetClicked()
    {
        // Emin misin diye sormadan direkt sıfırlıyoruz (İstersen panel ekleyebiliriz)
        GameManager.Instance.ResetGame();
        Debug.Log("Oyun Sıfırlandı!");
        // Sıfırlanınca ana menüye dönsün
        SceneManager.LoadScene("MenuScene");
    }

    void OnBackClicked()
    {
        // Ana Menüye dön
        SceneManager.LoadScene("MenuScene");
    }
}