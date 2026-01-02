using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI Elementleri")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI inventoryText;

    [Header("Butonlar")]
    public Button canArttirButton;
    public Button manaArttirButton;
    public Button geriDonButton;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        ButonlariBagla();
        EkraniGuncelle();
    }

    void ButonlariBagla()
    {
        canArttirButton.onClick.AddListener(CanArttir);
        manaArttirButton.onClick.AddListener(ManaArttir);
        geriDonButton.onClick.AddListener(StorySahnesineDon);
    }

    void EkraniGuncelle()
    {
        levelText.text = $"Level: {gameManager.playerLevel}";
        xpText.text = $"XP: {gameManager.playerXP}/100";
        healthText.text = $"Can: {gameManager.playerMaxHealth}";
        manaText.text = $"Mana: {gameManager.playerMaxMana}";

        if (gameManager.playerInventory.Count > 0)
            inventoryText.text = "Envanter: " + string.Join(", ", gameManager.playerInventory);
        else
            inventoryText.text = "Envanter: Boş";
    }

    public void CanArttir()
    {
        if (gameManager.playerLevel > 1)
        {
            gameManager.playerMaxHealth += 10;
            EkraniGuncelle();
        }
    }

    public void ManaArttir()
    {
        if (gameManager.playerLevel > 1)
        {
            gameManager.playerMaxMana += 5;
            EkraniGuncelle();
        }
    }

    void StorySahnesineDon()
    {
        SceneManager.LoadScene("StoryScene");
    }
}
