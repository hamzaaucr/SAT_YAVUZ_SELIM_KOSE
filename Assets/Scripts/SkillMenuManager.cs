using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkillMenuManager : MonoBehaviour
{
    [Header("UI Genel")]
    public TextMeshProUGUI skillPointsText;
    public Button backButton;
    public Transform contentPanel;
    public GameObject skillItemPrefab;

    [Header("AÇIKLAMA PANELİ (ALT)")]
    public GameObject infoPanel;
    public Image infoIconImage;
    public TextMeshProUGUI infoNameText;
    public TextMeshProUGUI infoDescText;
    public TextMeshProUGUI infoStatText;

    // --- YENİ: SAĞ ÜST İSTATİSTİK ---
    [Header("İSTATİSTİK PANELİ (SAĞ ÜST)")]
    public TextMeshProUGUI statLevelText;  // Level
    public TextMeshProUGUI statHealthText; // Toplam Can
    public TextMeshProUGUI statManaText;   // Toplam Mana
    public TextMeshProUGUI statDamageText; // Toplam Hasar
    public TextMeshProUGUI statXPText;     // Mevcut XP

    void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(MenuyeDon);
        if (infoPanel != null) infoPanel.SetActive(false);

        ListeyiYenile();
        StatlariGuncelle(); // <-- Başlangıçta statları yaz
    }

    // --- YENİ STAT GÜNCELLEME FONKSİYONU ---
    void StatlariGuncelle()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        if (statLevelText != null) statLevelText.text = $"SEVİYE: {gm.playerLevel}";
        if (statHealthText != null) statHealthText.text = $"CAN: {gm.TotalMaxHealth}"; // Eşyalar dahil can
        if (statManaText != null) statManaText.text = $"MANA: {gm.playerMaxMana}";
        if (statDamageText != null) statDamageText.text = $"HASAR: {gm.TotalDamage}"; // Kılıç dahil hasar
        if (statXPText != null) statXPText.text = $"XP: {gm.playerXP} / {gm.playerLevel * 100}";
    }

    void ListeyiYenile()
    {
        // Puanı güncelle
        if (skillPointsText != null)
            skillPointsText.text = "PUAN: " + GameManager.Instance.skillPoints;

        // Statları da her işlemde tazeleyelim
        StatlariGuncelle();

        foreach (Transform child in contentPanel) Destroy(child.gameObject);

        foreach (Skill skill in GameManager.Instance.gameSkills)
        {
            GameObject item = Instantiate(skillItemPrefab, contentPanel);

            Transform nameTr = item.transform.Find("NameText");
            Transform costTr = item.transform.Find("CostText");
            Transform iconTr = item.transform.Find("Icon");
            Transform buyBtnTr = item.transform.Find("BuyButton");

            if (nameTr == null || costTr == null || iconTr == null || buyBtnTr == null)
            {
                Debug.LogError("Prefab eksik!"); return;
            }

            TextMeshProUGUI nameTxt = nameTr.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costTxt = costTr.GetComponent<TextMeshProUGUI>();
            Image iconImg = iconTr.GetComponent<Image>();
            Button buyBtn = buyBtnTr.GetComponent<Button>();
            TextMeshProUGUI btnTxt = buyBtn.GetComponentInChildren<TextMeshProUGUI>();

            // Arka plana tıklayınca detay göster
            Button itemButton = item.GetComponent<Button>();
            if (itemButton == null) itemButton = item.AddComponent<Button>();
            itemButton.onClick.AddListener(() => DetayGoster(skill));

            nameTxt.text = skill.skillName;
            iconImg.sprite = skill.skillIcon;

            if (skill.isUnlocked)
            {
                costTxt.text = "AÇIK";
                costTxt.color = Color.green;
                buyBtn.interactable = false;
                if (btnTxt != null) btnTxt.text = "ALINDI";
            }
            else
            {
                costTxt.text = $"Bedel: {skill.pointCost} Puan\n(Lv: {skill.requiredLevel})";
                bool canBuy = (GameManager.Instance.skillPoints >= skill.pointCost) &&
                              (GameManager.Instance.playerLevel >= skill.requiredLevel);

                if (canBuy)
                {
                    buyBtn.interactable = true;
                    if (btnTxt != null) btnTxt.text = "SATIN AL";
                    buyBtn.onClick.AddListener(() => YetenekSatinAl(skill));
                }
                else
                {
                    buyBtn.interactable = false;
                    costTxt.color = Color.red;
                    if (btnTxt != null) btnTxt.text = "KİLİTLİ";
                }
            }
        }
    }

    void DetayGoster(Skill skill)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            if (infoIconImage != null) infoIconImage.sprite = skill.skillIcon;
            if (infoNameText != null) infoNameText.text = skill.skillName;
            if (infoDescText != null) infoDescText.text = skill.description;
            if (infoStatText != null)
            {
                string tur = skill.damage > 0 ? "Hasar" : "İyileştirme";
                infoStatText.text = $"{tur}: {Mathf.Abs(skill.damage)} | Mana: {skill.manaCost}";
            }
        }
    }

    void YetenekSatinAl(Skill skill)
    {
        bool basarili = GameManager.Instance.TryUnlockSkill(skill);
        if (basarili) ListeyiYenile();
    }

    void MenuyeDon() => SceneManager.LoadScene("StoryScene");
}