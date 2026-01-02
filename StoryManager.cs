using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [Header("UI Elementleri")]
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI inventoryText;

    [Header("Butonlar")]
    public Button kesfetButton;
    public Button savasButton;
    public Button inventoryButton;
    public Button menuButton;

    void Start()
    {
        // BUTONLARI ÖNCE BUL, SONRA BAĞLA
        ButonlariOtomatikBul();
        ButonlariBagla();
        EkraniGuncelle();
    }

    void ButonlariOtomatikBul()
    {
        // Eğer inspector'da atanmamışsa, otomatik bul
        if (kesfetButton == null) kesfetButton = GameObject.Find("KesfetButton")?.GetComponent<Button>();
        if (savasButton == null) savasButton = GameObject.Find("SavasButton")?.GetComponent<Button>();
        if (inventoryButton == null) inventoryButton = GameObject.Find("InventoryButton")?.GetComponent<Button>();
        if (menuButton == null) menuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();

        // Text'leri de bul
        if (storyText == null) storyText = GameObject.Find("StoryText")?.GetComponent<TextMeshProUGUI>();
        if (locationText == null) locationText = GameObject.Find("LocationText")?.GetComponent<TextMeshProUGUI>();
        if (healthText == null) healthText = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        if (moneyText == null) moneyText = GameObject.Find("MoneyText")?.GetComponent<TextMeshProUGUI>();
        if (levelText == null) levelText = GameObject.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
        if (inventoryText == null) inventoryText = GameObject.Find("InventoryText")?.GetComponent<TextMeshProUGUI>();
    }

    void ButonlariBagla()
    {
        // Null kontrolü yap
        if (kesfetButton != null) kesfetButton.onClick.AddListener(Kesfet);
        else Debug.LogError("KesfetButton bulunamadı!");

        if (savasButton != null) savasButton.onClick.AddListener(SavasaBasla);
        else Debug.LogError("SavasButton bulunamadı!");

        if (inventoryButton != null) inventoryButton.onClick.AddListener(InventoryAc);
        else Debug.LogError("InventoryButton bulunamadı!");

        if (menuButton != null) menuButton.onClick.AddListener(MenuyeDon);
        else Debug.LogError("MenuButton bulunamadı!");

        Debug.Log("Buton bağlantıları tamamlandı!");
    }

    void EkraniGuncelle()
    {
        // Null kontrolü yap
        if (locationText != null) locationText.text = "Lokasyon: Karanlık Orman";
        if (healthText != null) healthText.text = "Can: 100/100";
        if (moneyText != null) moneyText.text = "Para: 0";
        if (levelText != null) levelText.text = "Level: 1";
        if (inventoryText != null) inventoryText.text = "Envanter: Boş";
        if (storyText != null) storyText.text = "Karanlık Orman'a hoş geldiniz...";
    }

    public void Kesfet() => Debug.Log("Keşfet butonu çalıştı!");
    public void SavasaBasla() => SceneManager.LoadScene("BattleScene");
    public void InventoryAc() => SceneManager.LoadScene("LevelUpScene");
    public void MenuyeDon() => SceneManager.LoadScene("MenuScene");
}
