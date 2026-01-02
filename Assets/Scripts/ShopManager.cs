using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("UI BAĞLANTILARI")]
    public Transform shopGrid;
    public Transform playerGrid;
    public GameObject slotPrefab;
    public TextMeshProUGUI goldText;
    public Button backButton;

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
    private int currentTransactionPrice; // İşlem anındaki fiyat

    void Start()
    {
        gm = GameManager.Instance;
        if (backButton != null) { backButton.onClick.RemoveAllListeners(); backButton.onClick.AddListener(() => { SceneManager.LoadScene("StoryScene"); }); }
        if (gm == null) return;
        if (actionButton) actionButton.onClick.AddListener(IslemYap);
        if (infoPanel) infoPanel.SetActive(false);
        RefreshUI();
    }

    void RefreshUI()
    {
        if (gm == null) return;
        if (goldText) goldText.text = $"{gm.playerGold} Altın";

        // DÜKKAN LİSTESİ
        Temizle(shopGrid);
        foreach (var data in gm.tumEsyalarDatabase)
        {
            if (slotPrefab)
            {
                GameObject slot = Instantiate(slotPrefab, shopGrid);
                ResimAyarla(slot, data.esyaIsmi);
                Button btn = slot.GetComponent<Button>();
                if (btn) { string kopya = data.esyaIsmi; btn.onClick.AddListener(() => UrunSec(kopya, true)); }
            }
        }

        // OYUNCU ENVANTERİ
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
                    if (btn) { string kopya = item; btn.onClick.AddListener(() => UrunSec(kopya, false)); }
                }
            }
        }
    }

    void UrunSec(string item, bool buying)
    {
        selectedItem = item;
        isBuyingMode = buying;
        infoPanel.SetActive(true);

        if (infoIcon) infoIcon.sprite = gm.ResimGetir(item);
        if (infoName) infoName.text = item;

        // --- YENİ FİYAT SİSTEMİ ---
        var veri = gm.GetItemData(item);
        int guc = gm.GetItemPower(item);

        if (veri != null)
        {
            if (buying)
            {
                // Dükkandan alırken "satinAlmaFiyati" geçerli
                currentTransactionPrice = veri.satinAlmaFiyati;
                if (infoDesc) infoDesc.text = $"Güç: {guc}\n(Dükkan Ürünü)";
                if (infoPrice) infoPrice.text = $"Fiyat: {currentTransactionPrice} Altın";
                if (actionButtonText) actionButtonText.text = "SATIN AL";
            }
            else
            {
                // Satarken "satisFiyati" geçerli
                currentTransactionPrice = veri.satisFiyati;
                if (infoDesc) infoDesc.text = $"Güç: {guc}\n(Senin Eşyan)";
                if (infoPrice) infoPrice.text = $"Değer: {currentTransactionPrice} Altın";
                if (actionButtonText) actionButtonText.text = "SAT";
            }
        }
        else
        {
            // Hata koruması (Veritabanında yoksa)
            currentTransactionPrice = 0;
            if (infoPrice) infoPrice.text = "Fiyat Bilinmiyor";
        }
    }

    void IslemYap()
    {
        if (string.IsNullOrEmpty(selectedItem) || gm == null) return;

        if (isBuyingMode)
        {
            // ALMA
            if (gm.playerGold >= currentTransactionPrice)
            {
                gm.playerGold -= currentTransactionPrice;
                gm.playerInventory.Add(selectedItem);
            }
            else
            {
                Debug.Log("Paran yetmiyor!");
                return;
            }
        }
        else
        {
            // SATMA
            gm.playerGold += currentTransactionPrice;
            gm.playerInventory.Remove(selectedItem);
            gm.UnequipItem(gm.GetItemType(selectedItem));
        }

        gm.SaveGame();
        infoPanel.SetActive(false);
        RefreshUI();
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