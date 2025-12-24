using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    private GameManager gm;

    [Header("--- DİYALOG PANELİ ---")]
    public GameObject konusmaPanel;
    public Button devamButonu; // Tıklayınca ilerleten buton

    [Header("--- SOL TARAF (OYUNCU) ---")]
    public TextMeshProUGUI diyalogIsim;   // Sol İsim
    public Image solKarakterResmi;
    public Color oyuncuIsimRengi = Color.green;

    [Header("--- SAĞ TARAF (YAN KARAKTER/DÜŞMAN) ---")]
    public TextMeshProUGUI yanKarakterIsimText; // Sağ İsim
    public Image sagKarakterResmi;
    public Color yanKarakterIsimRengi = Color.yellow;

    [Header("--- GENEL ---")]
    public TextMeshProUGUI diyalogMetin; // Ortadaki hikaye yazısı
    public Color pasifResimRengi = new Color(0.5f, 0.5f, 0.5f, 1f);

    private Queue<HikayeSatiri> cumleKuyrugu = new Queue<HikayeSatiri>();

    [Header("DİĞER UI BAĞLANTILARI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI enemyInfoText;
    public TextMeshProUGUI goldText;

    [Header("BUTONLAR")]
    public Button fightButton;
    public TextMeshProUGUI fightBtnText;
    public Button shopButton;
    public Button inventoryButton;
    public Button skillsButton;
    public Button ayarlarButton;
    public Button menuButton;

    void Start()
    {
        gm = GameManager.Instance;
        if (gm == null) return;

        // --- BUTON BAĞLANTILARI ---
        if (fightButton) fightButton.onClick.AddListener(SavasaBasla);
        if (shopButton) shopButton.onClick.AddListener(() => SceneManager.LoadScene("ShopScene"));
        if (inventoryButton) inventoryButton.onClick.AddListener(() => SceneManager.LoadScene("InventoryScene"));
        if (skillsButton) skillsButton.onClick.AddListener(() => SceneManager.LoadScene("SkillScene"));
        if (menuButton) menuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        if (ayarlarButton) ayarlarButton.onClick.AddListener(() => gm.ToggleLowCpuMode());

        // --- DEVAM BUTONU (Otomatik Tamir) ---
        if (devamButonu)
        {
            devamButonu.onClick.RemoveAllListeners();
            devamButonu.onClick.AddListener(SiradakiCumle);
        }
        else
        {
            Debug.LogWarning("UYARI: 'Devam Butonu' Inspector'da boş bırakılmış!");
        }

        // --- HİKAYEYİ BAŞLAT ---
        SenaryoBolumu aktifSenaryo = gm.GetCurrentScenario();
        UpdateUI(aktifSenaryo);

        if (aktifSenaryo != null && aktifSenaryo.diyaloglar != null && aktifSenaryo.diyaloglar.Count > 0)
        {
            DiyalogBaslat(aktifSenaryo.diyaloglar);
        }
        else
        {
            if (konusmaPanel) konusmaPanel.SetActive(false);
        }
    }

    public void DiyalogBaslat(List<HikayeSatiri> hikaye)
    {
        if (konusmaPanel) konusmaPanel.SetActive(true);
        cumleKuyrugu.Clear();
        foreach (var satir in hikaye) cumleKuyrugu.Enqueue(satir);
        SiradakiCumle();
    }

    public void SiradakiCumle()
    {
        // Kuyruk bittiyse paneli kapat
        if (cumleKuyrugu.Count == 0)
        {
            DiyalogBitti();
            return;
        }

        HikayeSatiri satir = cumleKuyrugu.Dequeue();

        // Metni Yaz
        if (diyalogMetin) diyalogMetin.text = satir.cumle;

        if (satir.sagTarafKonusuyor)
        {
            // >>> SAĞDAKİ KONUŞUYOR <<<

            // 1. İsim Ayarı
            if (yanKarakterIsimText)
            {
                yanKarakterIsimText.gameObject.SetActive(true);
                yanKarakterIsimText.text = satir.konusanIsim; // GameManager'daki doğru değişken
                yanKarakterIsimText.color = yanKarakterIsimRengi;
            }
            if (diyalogIsim) diyalogIsim.gameObject.SetActive(false);

            // 2. Resim Ayarı
            if (sagKarakterResmi)
            {
                sagKarakterResmi.gameObject.SetActive(true);
                if (satir.karakterResmi != null) sagKarakterResmi.sprite = satir.karakterResmi;
                sagKarakterResmi.color = Color.white;
            }
            if (solKarakterResmi) solKarakterResmi.color = pasifResimRengi;
        }
        else
        {
            // >>> SOLDAKİ KONUŞUYOR <<<

            // 1. İsim Ayarı
            if (diyalogIsim)
            {
                diyalogIsim.gameObject.SetActive(true);
                diyalogIsim.text = satir.konusanIsim; // GameManager'daki doğru değişken
                diyalogIsim.color = oyuncuIsimRengi;
            }
            if (yanKarakterIsimText) yanKarakterIsimText.gameObject.SetActive(false);

            // 2. Resim Ayarı
            if (solKarakterResmi)
            {
                solKarakterResmi.gameObject.SetActive(true);
                if (satir.karakterResmi != null) solKarakterResmi.sprite = satir.karakterResmi;
                solKarakterResmi.color = Color.white;
            }
            if (sagKarakterResmi) sagKarakterResmi.color = pasifResimRengi;
        }
    }

    void DiyalogBitti()
    {
        if (konusmaPanel) konusmaPanel.SetActive(false);
    }

    void UpdateUI(SenaryoBolumu senaryo)
    {
        if (gm == null) return;

        if (goldText) goldText.text = $"{gm.playerGold} Altın";
        if (titleText) titleText.text = $"BÖLÜM {gm.currentChapter}";

        string dusmanAdi = gm.GetCurrentEnemyName();
        if (enemyInfoText) enemyInfoText.text = $"Sıradaki Tehdit: <color=#FF0000>{dusmanAdi}</color>";

        if (fightBtnText)
        {
            if (gm.currentFight >= gm.totalFightsPerChapter)
            {
                fightBtnText.text = "BOSS SAVAŞI 💀";
                fightBtnText.color = Color.red;
            }
            else
            {
                fightBtnText.text = $"SAVAŞ ({gm.currentFight}/{gm.totalFightsPerChapter})";
                fightBtnText.color = Color.black;
            }
        }

        if (storyText)
        {
            if (senaryo != null && !string.IsNullOrEmpty(senaryo.hikayeMetni))
                storyText.text = senaryo.hikayeMetni;
            else
                storyText.text = "Etraf sessiz... (Hikaye Yok)";
        }
    }

    void SavasaBasla()
    {
        if (gm.currentFight >= gm.totalFightsPerChapter) gm.isBossFight = true;
        else gm.isBossFight = false;
        SceneManager.LoadScene("BattleScene");
    }
}