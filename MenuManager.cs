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

        // Butonları otomatik bul
        ButonlariOtomatikBul();
        ButonlariBagla();

        // Test için buton renklerini değiştir
        ButonlariTestEt();
    }

    void ButonlariOtomatikBul()
    {
        if (yeniOyunButton == null)
            yeniOyunButton = GameObject.Find("YeniOyunButton")?.GetComponent<Button>();
        if (yukleOyunButton == null)
            yukleOyunButton = GameObject.Find("YukleOyunButton")?.GetComponent<Button>();
        if (ayarlarButton == null)
            ayarlarButton = GameObject.Find("AyarlarButton")?.GetComponent<Button>();
        if (cikisButton == null)
            cikisButton = GameObject.Find("CikisButton")?.GetComponent<Button>();

        Debug.Log($"YeniOyunButton: {yeniOyunButton != null}");
        Debug.Log($"YukleOyunButton: {yukleOyunButton != null}");
        Debug.Log($"AyarlarButton: {ayarlarButton != null}");
        Debug.Log($"CikisButton: {cikisButton != null}");
    }

    void ButonlariBagla()
    {
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

        Debug.Log("Buton bağlantıları tamamlandı!");
    }

    void ButonlariTestEt()
    {
        // YENİ METOT: FindObjectsByType kullan
        Button[] butonlar = FindObjectsByType<Button>(FindObjectsSortMode.None);
        Debug.Log($"Toplam buton sayısı: {butonlar.Length}");

        foreach (Button btn in butonlar)
        {
            Debug.Log($"Buton: {btn.name} - Interactable: {btn.interactable}");
        }
    }

    public void YeniOyunBaslat()
    {
        Debug.Log("🎮 YENİ OYUN BAŞLATILDI!");
        SceneManager.LoadScene("StoryScene");
    }

    public void OyunYukle() => Debug.Log("📂 Oyun yüklenecek...");
    public void AyarlariAc() => Debug.Log("⚙️ Ayarlar açılacak...");
    public void OyundanCik()
    {
        Debug.Log("🚪 Oyundan çıkılıyor...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
