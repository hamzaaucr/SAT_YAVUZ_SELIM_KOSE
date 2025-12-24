using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Butonlar")]
    public Button yeniOyunButton;
    public Button yukleOyunButton;
    public Button ayarlarButton;
    public Button cikisButton;

    void Start()
    {
        Debug.Log("MenuManager başladı!");
        ButonlariOtomatikBul();
        ButonlariBagla();
    }

    void ButonlariOtomatikBul()
    {
        // Eğer Inspector'da boş bıraktıysan isimden bulmaya çalışır
        if (yeniOyunButton == null) yeniOyunButton = GameObject.Find("YeniOyunButton")?.GetComponent<Button>();
        if (yukleOyunButton == null) yukleOyunButton = GameObject.Find("YukleOyunButton")?.GetComponent<Button>();
        if (ayarlarButton == null) ayarlarButton = GameObject.Find("AyarlarButton")?.GetComponent<Button>();
        if (cikisButton == null) cikisButton = GameObject.Find("CikisButton")?.GetComponent<Button>();
    }

    void ButonlariBagla()
    {
        // Eski bağlantıları temizle ve yenilerini ekle (Çift tıklama hatasını önler)
        if (yeniOyunButton != null)
        {
            yeniOyunButton.onClick.RemoveAllListeners();
            yeniOyunButton.onClick.AddListener(YeniOyunBaslat);
        }

        if (yukleOyunButton != null)
        {
            yukleOyunButton.onClick.RemoveAllListeners();
            yukleOyunButton.onClick.AddListener(OyunYukle);
        }

        if (ayarlarButton != null)
        {
            ayarlarButton.onClick.RemoveAllListeners();
            ayarlarButton.onClick.AddListener(AyarlariAc);
        }

        if (cikisButton != null)
        {
            cikisButton.onClick.RemoveAllListeners();
            cikisButton.onClick.AddListener(OyundanCik);
        }
    }

    public void YeniOyunBaslat()
    {
        Debug.Log("🆕 YENİ OYUN BAŞLATILDI!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.SaveGame();
        }

        SceneManager.LoadScene("StoryScene");
    }

    public void OyunYukle()
    {
        Debug.Log("📂 KAYITLI OYUN YÜKLENİYOR...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }

        SceneManager.LoadScene("StoryScene");
    }

    // --- GÜNCELLENEN KISIM ---
    public void AyarlariAc()
    {
        Debug.Log("⚙️ Ayarlar açılıyor...");
        // Artık SettingsScene sahnesine gidiyoruz!
        SceneManager.LoadScene("SettingsScene");
    }

    public void OyundanCik()
    {
        Debug.Log("👋 Oyundan çıkılıyor...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}