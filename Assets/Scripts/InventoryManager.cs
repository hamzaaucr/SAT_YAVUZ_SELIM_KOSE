using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [Header("KARAKTER SEÇÝMÝ")]
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI charClassText;
    public Button nextCharButton;
    public Button prevCharButton;

    [Header("SES SÝSTEMÝ")]
    public AudioSource sfxSource; // Inspector'dan MusicManager'ý buraya sürükle
    public AudioClip soundWeapon; // metal-clash.wav
    public AudioClip soundArmor;  // leather_inventory.wav
    public AudioClip soundRing;   // ring_inventory.wav

    [Header("Slot Ayarlarý")]
    public GameObject slotPrefab;
    public Transform gridKutusu;

    [Header("10'LU EKÝPMAN SLOTLARI (Target)")]
    // Inspector'da bu kutularýn dolu olduðundan emin ol!
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
        if (gm == null) { Debug.LogError("HATA: GameManager yok!"); return; }

        if (backButton) backButton.onClick.AddListener(() => SceneManager.LoadScene("StoryScene"));
        if (actionButton) actionButton.onClick.AddListener(SecilenEsyayiIsle);
        if (infoPanel) infoPanel.SetActive(false);

        if (nextCharButton) nextCharButton.onClick.AddListener(SonrakiKarakter);
        if (prevCharButton) prevCharButton.onClick.AddListener(OncekiKarakter);

        // Çýkarma Butonlarý
        Bagla(btnHelm, "Head"); Bagla(btnBody, "Body"); Bagla(btnLegs, "Legs"); Bagla(btnGloves, "Gloves");
        Bagla(btnHandR, "HandR"); Bagla(btnHandL, "HandL");
        Bagla(btnRing1, "Ring1"); Bagla(btnRing2, "Ring2");
        Bagla(btnUniq1, "Uniq1"); Bagla(btnUniq2, "Uniq2");

        // Slot Tanýtýmlarý
        SlotTanit(imgHelm, "Head"); SlotTanit(imgBody, "Body"); SlotTanit(imgLegs, "Legs"); SlotTanit(imgGloves, "Gloves");
        SlotTanit(imgHandR, "Weapon"); SlotTanit(imgHandL, "Shield");
        SlotTanit(imgRing1, "Ring"); SlotTanit(imgRing2, "Ring");
        SlotTanit(imgUniq1, "Unique"); SlotTanit(imgUniq2, "Unique");

        EkraniGuncelle();
    }

    void SlotTanit(Image img, string tur)
    {
        if (img == null) return;
        EquipmentSlot slot = img.GetComponent<EquipmentSlot>();
        if (slot == null) slot = img.gameObject.AddComponent<EquipmentSlot>();
        slot.slotType = tur;
        slot.manager = this;
    }

    public void TryEquipFromDrag(string itemName, string targetSlotType)
    {
        string itemType = gm.GetItemType(itemName);
        bool uygunMu = false;

        if (targetSlotType == "Weapon" && itemType == "Weapon") uygunMu = true;
        else if (targetSlotType == "Shield" && itemType == "Shield") uygunMu = true;
        else if (targetSlotType == "Ring" && itemType == "Ring") uygunMu = true;
        else if (targetSlotType == "Unique" && itemType == "Unique") uygunMu = true;
        else if (targetSlotType == itemType) uygunMu = true;

        if (uygunMu)
        {
            gm.EquipItem(itemName);

            // --- SESÝ ÇAL ---
            SesCal(itemType);
            // ----------------

            EkraniGuncelle();
            Debug.Log($"{itemName} baþarýyla kuþandý!");
        }
        else
        {
            EkraniGuncelle();
        }
    }

    public void SlotTiklandi(string slotTuru)
    {
        if (gm == null) return;
        CharacterData c = gm.CurrentChar;
        string bulunanEsya = "Yok";

        switch (slotTuru)
        {
            case "Head": bulunanEsya = c.equipHelm; break;
            case "Body": bulunanEsya = c.equipBody; break;
            case "Legs": bulunanEsya = c.equipLegs; break;
            case "Gloves": bulunanEsya = c.equipGloves; break;
            case "Weapon": bulunanEsya = c.equipHandR; break;
            case "Shield": bulunanEsya = c.equipHandL; break;
            case "Ring": bulunanEsya = (c.equipRing1 != "Yok") ? c.equipRing1 : c.equipRing2; break;
            case "Unique": bulunanEsya = (c.equipUnique1 != "Yok") ? c.equipUnique1 : c.equipUnique2; break;
        }

        if (bulunanEsya != "Yok" && !string.IsNullOrEmpty(bulunanEsya))
        {
            DetayGoster(bulunanEsya);
            if (actionButtonText) actionButtonText.text = "Geri";
        }
    }

    void SonrakiKarakter() { gm.activeMemberIndex++; if (gm.activeMemberIndex >= gm.partyMembers.Count) gm.activeMemberIndex = 0; EkraniGuncelle(); }
    void OncekiKarakter() { gm.activeMemberIndex--; if (gm.activeMemberIndex < 0) gm.activeMemberIndex = gm.partyMembers.Count - 1; EkraniGuncelle(); }

    void Bagla(Button btn, string slotAdi)
    {
        if (btn != null) { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(() => { gm.UnequipItem(slotAdi); EkraniGuncelle(); }); }
    }

    void EkraniGuncelle()
    {
        if (gm == null) return;
        CharacterData c = gm.CurrentChar;

        if (charNameText) charNameText.text = c.characterName;
        if (charClassText) charClassText.text = c.jobClass + $" (Lv {c.currentLevel})";

        if (statsText) statsText.text = $"HASAR: {gm.TotalDamage} | CAN: {gm.TotalMaxHealth} | Regen: {gm.GetTurnRegen()}";

        GuncelleResim(imgHelm, c.equipHelm); GuncelleResim(imgBody, c.equipBody);
        GuncelleResim(imgLegs, c.equipLegs); GuncelleResim(imgGloves, c.equipGloves);
        GuncelleResim(imgHandR, c.equipHandR); GuncelleResim(imgHandL, c.equipHandL);
        GuncelleResim(imgRing1, c.equipRing1); GuncelleResim(imgRing2, c.equipRing2);
        GuncelleResim(imgUniq1, c.equipUnique1); GuncelleResim(imgUniq2, c.equipUnique2);

        if (gridKutusu != null)
        {
            foreach (Transform child in gridKutusu) Destroy(child.gameObject);

            if (slotPrefab == null) return;

            foreach (string item in gm.playerInventory)
            {
                GameObject yeniSlot = Instantiate(slotPrefab, gridKutusu);
                DraggableItem dragScript = yeniSlot.GetComponent<DraggableItem>();
                if (dragScript == null) dragScript = yeniSlot.AddComponent<DraggableItem>();

                dragScript.myItemName = item;
                dragScript.manager = this;

                if (yeniSlot.transform.childCount > 0)
                {
                    Image ikon = yeniSlot.transform.GetChild(0).GetComponent<Image>();
                    if (ikon != null) { ikon.sprite = gm.ResimGetir(item); ikon.color = Color.white; }
                }
            }
        }
    }

    void GuncelleResim(Image img, string item)
    {
        if (img == null) return;
        if (item != "Yok") { img.sprite = gm.ResimGetir(item); img.enabled = true; img.color = Color.white; }
        else { img.sprite = null; img.enabled = true; img.color = new Color(0, 0, 0, 0); }
    }

    public void DetayGoster(string item)
    {
        secilenEsya = item;
        if (infoPanel) infoPanel.SetActive(true);
        if (infoIconImage) infoIconImage.sprite = gm.ResimGetir(item);
        if (infoNameText) infoNameText.text = item;
        string tur = gm.GetItemType(item); int guc = gm.GetItemPower(item);
        if (infoDescText) infoDescText.text = $"{tur} (Güç: {guc})";
        if (actionButtonText) actionButtonText.text = "KUÞAN";
    }

    void SecilenEsyayiIsle()
    {
        if (string.IsNullOrEmpty(secilenEsya)) return;
        if (actionButtonText.text == "Geri")
        {
            if (infoPanel) infoPanel.SetActive(false);
            return;
        }

        gm.EquipItem(secilenEsya);
        if (infoPanel) infoPanel.SetActive(false);
        EkraniGuncelle();
    }

    // --- SES ÇALMA FONKSÝYONU ---
    void SesCal(string itemType)
    {
        if (sfxSource == null) return;

        if (itemType == "Weapon" || itemType == "Shield")
        {
            if (soundWeapon != null) sfxSource.PlayOneShot(soundWeapon);
        }
        else if (itemType == "Ring" || itemType == "Unique")
        {
            if (soundRing != null) sfxSource.PlayOneShot(soundRing);
        }
        else
        {
            if (soundArmor != null) sfxSource.PlayOneShot(soundArmor);
        }
    }
}