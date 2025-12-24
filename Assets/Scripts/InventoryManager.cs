using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [Header("KARAKTER SEÇÝMÝ")]
    public TextMeshProUGUI charNameText; // Karakterin adý yazacak
    public TextMeshProUGUI charClassText; // Sýnýfý yazacak
    public Button nextCharButton; // Ýleri ok
    public Button prevCharButton; // Geri ok

    [Header("Slot Ayarlarý")]
    public GameObject slotPrefab;
    public Transform gridKutusu;

    [Header("10'LU EKÝPMAN SLOTLARI")]
    public Image imgHelm, imgBody, imgLegs, imgGloves;
    public Image imgHandR, imgHandL;
    public Image imgRing1, imgRing2, imgUniq1, imgUniq2;

    [Header("ÇIKARMA BUTONLARI")]
    public Button btnHelm, btnBody, btnLegs, btnGloves;
    public Button btnHandR, btnHandL;
    public Button btnRing1, btnRing2, btnUniq1, btnUniq2;

    [Header("UI Genel")]
    public TextMeshProUGUI statsText;
    public Button backButton;

    [Header("AÇIKLAMA PANELÝ")]
    public GameObject infoPanel;
    public Image infoIconImage;
    public TextMeshProUGUI infoNameText, infoDescText, infoStatText, actionButtonText;
    public Button actionButton;

    private GameManager gm;
    private string secilenEsya;

    void Start()
    {
        gm = GameManager.Instance;

        if (backButton) backButton.onClick.AddListener(() => SceneManager.LoadScene("StoryScene"));
        if (actionButton) actionButton.onClick.AddListener(SecilenEsyayiIsle);
        if (infoPanel) infoPanel.SetActive(false);

        // Karakter Deðiþtirme Butonlarý
        if (nextCharButton) nextCharButton.onClick.AddListener(SonrakiKarakter);
        if (prevCharButton) prevCharButton.onClick.AddListener(OncekiKarakter);

        // Çýkarma Butonlarýný Baðla
        Bagla(btnHelm, "Head"); Bagla(btnBody, "Body"); Bagla(btnLegs, "Legs"); Bagla(btnGloves, "Gloves");
        Bagla(btnHandR, "HandR"); Bagla(btnHandL, "HandL");
        Bagla(btnRing1, "Ring1"); Bagla(btnRing2, "Ring2");
        Bagla(btnUniq1, "Uniq1"); Bagla(btnUniq2, "Uniq2");

        EkraniGuncelle();
    }

    // --- KARAKTER DEÐÝÞTÝRME ---
    void SonrakiKarakter()
    {
        gm.activeMemberIndex++;
        if (gm.activeMemberIndex >= gm.partyMembers.Count) gm.activeMemberIndex = 0; // Baþa dön
        EkraniGuncelle();
    }

    void OncekiKarakter()
    {
        gm.activeMemberIndex--;
        if (gm.activeMemberIndex < 0) gm.activeMemberIndex = gm.partyMembers.Count - 1; // Sona dön
        EkraniGuncelle();
    }

    void Bagla(Button btn, string slotAdi)
    {
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { gm.UnequipItem(slotAdi); EkraniGuncelle(); });
        }
    }

    void EkraniGuncelle()
    {
        // Þu anki karakteri al
        CharacterData c = gm.CurrentChar;

        // Ýsim ve Sýnýf Yaz
        if (charNameText) charNameText.text = c.characterName;
        if (charClassText) charClassText.text = c.jobClass + $" (Lv {c.currentLevel})";

        // Statlar (Artýk seçili karaktere göre hesaplanýyor)
        string pasifler = "";
        if (gm.GetTurnRegen() > 0) pasifler += $" | HP Regen: {gm.GetTurnRegen()}";
        if (statsText) statsText.text = $"HASAR: {gm.TotalDamage} | CAN: {gm.TotalMaxHealth}{pasifler}";

        // 10 SLOTU GÜNCELLE (Seçili karakterin eþyalarýna göre)
        GuncelleResim(imgHelm, c.equipHelm); GuncelleResim(imgBody, c.equipBody);
        GuncelleResim(imgLegs, c.equipLegs); GuncelleResim(imgGloves, c.equipGloves);
        GuncelleResim(imgHandR, c.equipHandR); GuncelleResim(imgHandL, c.equipHandL);
        GuncelleResim(imgRing1, c.equipRing1); GuncelleResim(imgRing2, c.equipRing2);
        GuncelleResim(imgUniq1, c.equipUnique1); GuncelleResim(imgUniq2, c.equipUnique2);

        // ENVANTER LÝSTESÝ
        if (gridKutusu != null)
        {
            foreach (Transform child in gridKutusu) Destroy(child.gameObject);
            foreach (string item in gm.playerInventory)
            {
                GameObject yeniSlot = Instantiate(slotPrefab, gridKutusu);
                Image ikon = yeniSlot.transform.GetChild(0).GetComponent<Image>();
                Sprite resim = gm.ResimGetir(item);
                if (resim != null) { ikon.sprite = resim; ikon.color = Color.white; }
                string kopya = item;
                yeniSlot.GetComponent<Button>().onClick.AddListener(() => DetayGoster(kopya));
            }
        }
    }

    void GuncelleResim(Image img, string item)
    {
        if (img == null) return;
        if (item != "Yok") { img.sprite = gm.ResimGetir(item); img.enabled = true; img.color = Color.white; }
        else { img.enabled = false; }
    }

    // --- DETAY VE ÝÞLEM --- (Öncekiyle ayný)
    void DetayGoster(string item)
    {
        secilenEsya = item; infoPanel.SetActive(true);
        if (infoIconImage) infoIconImage.sprite = gm.ResimGetir(item);
        if (infoNameText) infoNameText.text = item;

        string tur = gm.GetItemType(item); int guc = gm.GetItemPower(item);
        if (infoDescText) infoDescText.text = $"{tur} (Güç: {guc})";
        if (actionButtonText) actionButtonText.text = "KUÞAN (" + gm.CurrentChar.characterName + ")";
    }

    void SecilenEsyayiIsle()
    {
        if (string.IsNullOrEmpty(secilenEsya)) return;
        gm.EquipItem(secilenEsya);
        infoPanel.SetActive(false);
        EkraniGuncelle();
    }
}