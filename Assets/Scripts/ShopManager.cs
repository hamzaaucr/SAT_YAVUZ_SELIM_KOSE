using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Sahne değişimi için şart

public class ShopManager : MonoBehaviour
{
    [Header("UI BAĞLANTILARI")]
    public Transform shopGrid;
    public Transform playerGrid;
    public GameObject slotPrefab;
    public TextMeshProUGUI goldText;
    public Button backButton; // <-- Inspector'da bunu bağlamayı unutma!

    [Header("DETAY PANELİ")]
    public GameObject infoPanel;
    public Image infoIcon;
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoDesc;
    public TextMeshProUGUI infoPrice;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    private GameManager gm;
    private string selectedItem;
    private bool isBuyingMode;

    private string[] shopItems = {
        "Demir Kılıç", "Kraliyet Zırhı", "Altın Miğfer", "Gümüş Miğfer",
        "Yakut Yüzük", "Safir Yüzük", "Zümrüt Yüzük",
        "Şövalye Kalkanı", "Savaş Kalkanı", "Efsanevi Kılıç"
    };

    void Start()
    {
        gm = GameManager.Instance;

        // 1. GERİ BUTONU AYARI (StoryScene'e Götürür) 🔙
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Önceki hatalı bağlantıları temizle
            backButton.onClick.AddListener(() => {
                Debug.Log("Hikayeye dönülüyor...");
                SceneManager.LoadScene("StoryScene"); // <-- HEDEF SAHNE
            });
        }
        else
        {
            Debug.LogError("UYARI: Back Button (Geri Tuşu) Inspector'da boş! Lütfen sürükle.");
        }

        // GameManager yoksa hata vermesin, dursun (Test güvenliği)
        if (gm == null) return;

        if (actionButton) actionButton.onClick.AddListener(IslemYap);
        if (infoPanel) infoPanel.SetActive(false);

        RefreshUI();
    }

    void RefreshUI()
    {
        if (gm == null) return;

        // Altın Güncelle
        if (goldText) goldText.text = $"{gm.playerGold} Altın";

        // Sol Taraf (SATICI)
        Temizle(shopGrid);
        foreach (string item in shopItems)
        {
            // Prefab ve Buton kontrolü yaparak ekle
            if (slotPrefab)
            {
                GameObject slot = Instantiate(slotPrefab, shopGrid);
                ResimAyarla(slot, item);

                Button btn = slot.GetComponent<Button>();
                if (btn)
                {
                    string kopya = item;
                    btn.onClick.AddListener(() => UrunSec(kopya, true));
                }
            }
        }

        // Sağ Taraf (OYUNCU)
        Temizle(playerGrid);
        if (gm.playerInventory != null)
        {
            foreach (string item in gm.playerInventory)
            {
                if (slotPrefab)
                {
                    GameObject slot = Instantiate(slotPrefab, playerGrid);
                    ResimAyarla(slot, item);

                    Button btn = slot.GetComponent<Button>();
                    if (btn)
                    {
                        string kopya = item;
                        btn.onClick.AddListener(() => UrunSec(kopya, false));
                    }
                }
            }
        }
    }

    // --- DİĞER FONKSİYONLAR (Aynı) ---
    void UrunSec(string item, bool buying)
    {
        selectedItem = item;
        isBuyingMode = buying;
        infoPanel.SetActive(true);

        if (infoIcon) infoIcon.sprite = gm.ResimGetir(item);
        if (infoName) infoName.text = item;

        int fiyat = GetPrice(item);

        if (buying)
        {
            if (infoDesc) infoDesc.text = "Satıcı ürünü.";
            if (infoPrice) infoPrice.text = $"Fiyat: {fiyat}";
            if (actionButtonText) actionButtonText.text = "SATIN AL";
        }
        else
        {
            int satis = fiyat / 2;
            if (infoDesc) infoDesc.text = "Senin eşyan.";
            if (infoPrice) infoPrice.text = $"Değer: {satis}";
            if (actionButtonText) actionButtonText.text = "SAT";
        }
    }

    void IslemYap()
    {
        if (string.IsNullOrEmpty(selectedItem) || gm == null) return;
        int fiyat = GetPrice(selectedItem);

        if (isBuyingMode)
        {
            if (gm.playerGold >= fiyat)
            {
                gm.playerGold -= fiyat;
                gm.playerInventory.Add(selectedItem);
            }
        }
        else
        {
            gm.playerGold += (fiyat / 2);
            gm.playerInventory.Remove(selectedItem);
            gm.UnequipItem(gm.GetItemType(selectedItem));
        }
        gm.SaveGame();
        infoPanel.SetActive(false);
        RefreshUI();
    }

    int GetPrice(string item)
    {
        if (item.Contains("Tahta")) return 50;
        if (item.Contains("Demir")) return 150;
        if (item.Contains("Çelik")) return 300;
        if (item.Contains("Kraliyet")) return 1000;
        if (item.Contains("Yüzük")) return 250;
        if (item.Contains("Efsanevi")) return 5000;
        return 100;
    }

    void Temizle(Transform grid) { foreach (Transform child in grid) Destroy(child.gameObject); }

    void ResimAyarla(GameObject slot, string item)
    {
        if (slot.transform.childCount > 0)
        {
            Image ikon = slot.transform.GetChild(0).GetComponent<Image>();
            Sprite resim = gm.ResimGetir(item);
            if (resim != null) { ikon.sprite = resim; ikon.color = Color.white; }
        }
    }
}