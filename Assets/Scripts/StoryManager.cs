using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    private GameManager gm;

    [Header("--- DİYALOG PANELİ ---")]
    public GameObject konusmaPanel;
    public Button devamButonu;

    [Header("--- MENU BUTONLARI ---")]
    public Button fightButton;
    public Button shopButton;
    public Button inventoryButton;
    public Button skillsButton;
    public Button ayarlarButton;
    public Button menuButton;

    [Header("--- KONUŞMA UI (İSİMLER & RESİMLER) ---")]
    public TextMeshProUGUI diyalogIsim;
    public Image solKarakterResmi;
    public TextMeshProUGUI yanKarakterIsimText;
    public Image sagKarakterResmi;
    public TextMeshProUGUI diyalogMetin;

    [Header("--- SAHNE UI (BAŞLIK & BİLGİLER) ---")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI enemyInfoText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI fightBtnText;

    [Header("--- AYARLAR ---")]
    public Color oyuncuIsimRengi = Color.green;
    public Color yanKarakterIsimRengi = Color.yellow;
    public Color pasifResimRengi = new Color(0.5f, 0.5f, 0.5f, 1f);
    public float yaziHizi = 0.05f;

    private Queue<HikayeSatiri> cumleKuyrugu = new Queue<HikayeSatiri>();
    private bool isTyping = false;
    private string suAnkiTamMetin = "";

    void Start()
    {
        gm = GameManager.Instance;
        if (gm == null) return;

        // Buton Dinleyicileri
        if (fightButton) fightButton.onClick.AddListener(SavasaBasla);
        if (shopButton) shopButton.onClick.AddListener(() => SceneManager.LoadScene("ShopScene"));
        if (inventoryButton) inventoryButton.onClick.AddListener(() => SceneManager.LoadScene("InventoryScene"));
        if (skillsButton) skillsButton.onClick.AddListener(() => SceneManager.LoadScene("SkillScene"));
        if (menuButton) menuButton.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        if (ayarlarButton) ayarlarButton.onClick.AddListener(() => gm.ToggleLowCpuMode());

        if (devamButonu)
        {
            devamButonu.onClick.RemoveAllListeners();
            devamButonu.onClick.AddListener(HandleContinueClick);
        }

        SenaryoBolumu aktifSenaryo = gm.GetCurrentScenario();
        UpdateUI(aktifSenaryo);

        // --- HİKAYE KONTROLÜ ---
        // Eğer senaryo varsa VE hikaye daha önce okunmamışsa başlat
        if (aktifSenaryo != null && aktifSenaryo.diyaloglar != null && aktifSenaryo.diyaloglar.Count > 0 && !gm.hikayeOkundu)
        {
            DiyalogBaslat(aktifSenaryo.diyaloglar);
        }
        else
        {
            // Hikaye yoksa veya zaten okunduysa:
            if (konusmaPanel) konusmaPanel.SetActive(false);

            // Kilitleri kontrol et (Eğitim Modu)
            MenuButonlariKilitle(false);
        }
    }

    void MenuButonlariKilitle(bool konusmaVar)
    {
        if (konusmaVar)
        {
            // Konuşma varken her şeyi kapatıyoruz
            ButonDurumu(fightButton, false);
            ButonDurumu(shopButton, false);
            ButonDurumu(inventoryButton, false);
            ButonDurumu(skillsButton, false);
            ButonDurumu(menuButton, false);
            ButonDurumu(ayarlarButton, false);
        }
        else
        {
            // Konuşma yoksa, şartlara göre butonları açıyoruz
            ButonDurumu(shopButton, true);
            ButonDurumu(inventoryButton, true);
            ButonDurumu(menuButton, true);
            ButonDurumu(ayarlarButton, true);

            // 1. YETENEKLER BUTONU: 
            // Oyuncu dükkandan veya Elias'tan herhangi bir silah aldıysa açılsın
            bool silahVarMi = gm.CurrentChar.equipHandR != "Yok" || gm.CurrentChar.equipHandL != "Yok";
            ButonDurumu(skillsButton, silahVarMi);

            // 2. SAVAŞ BUTONU: 
            // Sadece "Normal Saldırı" yeteneği açıldığında aktif olsun
            bool saldiriAcikMi = false;
            foreach (var s in gm.gameSkills)
            {
                // İsim kontrolünü senin veritabanındaki isme göre yap (Normal Saldırı)
                if (s.skillName == "Hızlı Vuruş" && s.isUnlocked)
                {
                    saldiriAcikMi = true;
                    break;
                }
            }
            ButonDurumu(fightButton, saldiriAcikMi);
        }
    }

    void ButonDurumu(Button btn, bool aktif)
    {
        if (btn != null) btn.interactable = aktif;
    }

    public void HandleContinueClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            diyalogMetin.text = suAnkiTamMetin;
            isTyping = false;
        }
        else
        {
            SiradakiCumle();
        }
    }

    public void DiyalogBaslat(List<HikayeSatiri> hikaye)
    {
        MenuButonlariKilitle(true); // Konuşma başladı, KİLİTLE

        if (konusmaPanel) konusmaPanel.SetActive(true);
        cumleKuyrugu.Clear();
        foreach (var satir in hikaye) cumleKuyrugu.Enqueue(satir);
        SiradakiCumle();
    }

    public void SiradakiCumle()
    {
        if (cumleKuyrugu.Count == 0)
        {
            DiyalogBitti();
            return;
        }

        HikayeSatiri satir = cumleKuyrugu.Dequeue();
        suAnkiTamMetin = satir.cumle;
        KarakterUIHazirla(satir);

        StopAllCoroutines();
        StartCoroutine(YaziEfekti(satir.cumle));
    }

    IEnumerator YaziEfekti(string cumle)
    {
        isTyping = true;
        diyalogMetin.text = "";
        foreach (char harf in cumle.ToCharArray())
        {
            diyalogMetin.text += harf;
            yield return new WaitForSeconds(yaziHizi);
        }
        isTyping = false;
    }

    void DiyalogBitti()
    {
        if (konusmaPanel) konusmaPanel.SetActive(false);

        // HİKAYE BİTTİ, HAFIZAYA KAYDET
        gm.hikayeOkundu = true;

        // Kilitleri kontrol ederek aç
        MenuButonlariKilitle(false);
    }

    void KarakterUIHazirla(HikayeSatiri satir)
    {
        if (satir.sagTarafKonusuyor)
        {
            SetKarakterUI(yanKarakterIsimText, sagKarakterResmi, satir, yanKarakterIsimRengi);
            DisableKarakterUI(diyalogIsim, solKarakterResmi);
        }
        else
        {
            SetKarakterUI(diyalogIsim, solKarakterResmi, satir, oyuncuIsimRengi);
            DisableKarakterUI(yanKarakterIsimText, sagKarakterResmi);
        }
    }

    void SetKarakterUI(TextMeshProUGUI isimTxt, Image resimImg, HikayeSatiri satir, Color isimRenk)
    {
        if (isimTxt) { isimTxt.gameObject.SetActive(true); isimTxt.text = satir.konusanIsim; isimTxt.color = isimRenk; }
        if (resimImg) { resimImg.gameObject.SetActive(true); if (satir.karakterResmi != null) resimImg.sprite = satir.karakterResmi; resimImg.color = Color.white; }
    }

    void DisableKarakterUI(TextMeshProUGUI isimTxt, Image resimImg)
    {
        if (isimTxt) isimTxt.gameObject.SetActive(false);
        if (resimImg) resimImg.color = pasifResimRengi;
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
            if (gm.currentFight >= gm.totalFightsPerChapter) { fightBtnText.text = "BOSS SAVAŞI 💀"; fightBtnText.color = Color.red; }
            else { fightBtnText.text = $"SAVAŞ ({gm.currentFight}/{gm.totalFightsPerChapter})"; fightBtnText.color = Color.black; }
        }
        if (storyText) storyText.text = (senaryo != null && !string.IsNullOrEmpty(senaryo.hikayeMetni)) ? senaryo.hikayeMetni : "Etraf sessiz... (Hikaye Yok)";
    }

    void SavasaBasla()
    {
        if (gm.currentFight >= gm.totalFightsPerChapter) gm.isBossFight = true;
        else gm.isBossFight = false;
        SceneManager.LoadScene("BattleScene");
    }
}