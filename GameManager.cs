using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Oyun Verileri")]
    public int playerHealth = 100;
    public int playerMaxHealth = 100;
    public int playerMoney = 0;
    public int playerLevel = 1;
    public int playerXP = 0;
    public List<string> playerInventory = new List<string>();
    public int playerMana = 50;
    public int playerMaxMana = 50;

    public string currentLocation = "Karanlık Orman";
    public bool inCombat = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Sahne yönetimi
    public void MenuSahnesineGit() => SceneManager.LoadScene("MenuScene");
    public void StorySahnesineGit() => SceneManager.LoadScene("StoryScene");
    public void BattleSahnesineGit() => SceneManager.LoadScene("BattleScene");
    public void LevelUpSahnesineGit() => SceneManager.LoadScene("LevelUpScene");
}
