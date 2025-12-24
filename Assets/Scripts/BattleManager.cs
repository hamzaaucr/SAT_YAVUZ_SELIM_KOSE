using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("--- ANIMATOR ---")]
    public Animator playerAnimator; // Attack1/Attack2/Attack3
    public Animator enemyAnimator;  // Attack (tek)

    [Header("--- UI ---")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI battleLogText;
    public Slider playerHealthSlider, playerManaSlider, enemyHealthSlider;

    [Header("--- PANELS ---")]
    public GameObject victoryPanel, defeatPanel;

    [Header("--- SKILL WHEEL ---")]
    public SkillWheelUI skillWheelUI;

    [Header("--- VISUAL ---")]
    public SpriteRenderer playerSprite;
    public SpriteRenderer enemySprite;

    private GameManager gm;

    private int playerHealth, playerMana;
    private int enemyHealth, enemyMaxHealth;
    private string currentEnemyName;

    private bool isPlayerTurn = true;
    private bool actionLocked = false;

    void Start()
    {
        gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager.Instance bulunamadı!");
            SceneManager.LoadScene("MenuScene");
            return;
        }

        playerHealth = gm.TotalMaxHealth;
        playerMana = gm.playerMaxMana;

        if (playerHealthSlider) playerHealthSlider.maxValue = gm.TotalMaxHealth;
        if (playerManaSlider) playerManaSlider.maxValue = gm.playerMaxMana;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);

        DusmaniYukle();
        EkraniGuncelle();

        BattleLogEkle(">> Düşmana tıkla: saldırı skilleri (slot1=Attack1, slot2=Attack2, slot3=Attack3)");
        BattleLogEkle(">> Kendine tıkla: heal/buff skilleri");
        BattleLogEkle(">> Hamle sırası sende!");
    }

    // ClickDetector çağırır:
    // isPlayerClicked=true => heal/buff (damage<=0)
    // isPlayerClicked=false => attack (damage>0)
    public void KarakterTiklandi(bool isPlayerClicked)
    {
        if (!isPlayerTurn || actionLocked) return;

        if (skillWheelUI == null)
        {
            Debug.LogError("BattleManager: skillWheelUI atanmadı!");
            return;
        }

        var unlocked = gm.GetUnlockedSkills();
        if (unlocked == null)
        {
            Debug.LogError("GameManager.GetUnlockedSkills() null!");
            return;
        }

        List<Skill> list = new List<Skill>();

        foreach (var s in unlocked)
        {
            if (s == null) continue;

            bool uygun = (isPlayerClicked && s.damage <= 0) || (!isPlayerClicked && s.damage > 0);
            if (!uygun) continue;

            if (playerMana < s.manaCost) continue;

            list.Add(s);
        }

        if (list.Count == 0)
        {
            BattleLogEkle(">> Uygun skill yok (mana yetmiyor ya da yanlış hedef).");
            return;
        }

        actionLocked = true;
        skillWheelUI.Open(Input.mousePosition, list, SkillKullanSlotlu);
    }

    // slotIndex = 1..N (wheel sırasi)
    void SkillKullanSlotlu(Skill s, int slotIndex)
    {
        if (s == null) { actionLocked = false; return; }

        playerMana -= s.manaCost;
        EkraniGuncelle();

        if (s.damage > 0)
        {
            // PLAYER: slot 1->Attack1, slot 2->Attack2, slot 3+->Attack3
            string trigger = PlayerSlotToTrigger(slotIndex);
            if (playerAnimator) playerAnimator.SetTrigger(trigger);

            StartCoroutine(DamageDelay(isPlayerAttacking: true, delay: 0.55f, bonusDamage: s.damage));
        }
        else
        {
            // HEAL/BUFF
            int heal = Mathf.Abs(s.damage);
            playerHealth = Mathf.Min(playerHealth + heal, gm.TotalMaxHealth);
            BattleLogEkle($"> {s.skillName}: +{heal} HP");
            EkraniGuncelle();
            SiraDegistir();
        }
    }

    string PlayerSlotToTrigger(int slotIndex)
    {
        if (slotIndex <= 1) return "Attack1";
        if (slotIndex == 2) return "Attack2";
        return "Attack3";
    }

    IEnumerator DamageDelay(bool isPlayerAttacking, float delay, int bonusDamage)
    {
        yield return new WaitForSeconds(delay);

        if (isPlayerAttacking)
        {
            int totalDmg = gm.TotalDamage + bonusDamage;
            enemyHealth -= totalDmg;

            BattleLogEkle($"> Selim vurdu: {totalDmg} HASAR!");

            // Enemy Hurt (varsa)
            if (enemyAnimator) enemyAnimator.SetTrigger("Hurt");
            StartCoroutine(FlashColor(enemySprite, Color.red));

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                EkraniGuncelle();

                // Enemy Death (varsa)
                if (enemyAnimator) enemyAnimator.SetTrigger("Death");
                yield return new WaitForSeconds(1.2f);
                Victory();
                yield break;
            }

            EkraniGuncelle();
            SiraDegistir();
        }
        else
        {
            int dmg = gm.isBossFight ? Random.Range(15, 30) : Random.Range(5, 12);
            playerHealth -= dmg;

            BattleLogEkle($"! {currentEnemyName} saldırdı: -{dmg} HP");

            if (playerAnimator) playerAnimator.SetTrigger("Hurt");
            StartCoroutine(FlashColor(playerSprite, Color.red));

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                EkraniGuncelle();

                if (playerAnimator) playerAnimator.SetTrigger("Death");
                yield return new WaitForSeconds(1.5f);
                Defeat();
                yield break;
            }

            isPlayerTurn = true;
            actionLocked = false;
            BattleLogEkle(">> Hamle sırası sende!");
            EkraniGuncelle();
        }
    }

    void SiraDegistir()
    {
        isPlayerTurn = false;
        actionLocked = true;

        CancelInvoke(nameof(EnemyTurn));
        Invoke(nameof(EnemyTurn), 1.1f);
    }

    void EnemyTurn()
    {
        if (enemyHealth <= 0 || playerHealth <= 0) return;

        // ENEMY: tek saldırı -> "Attack"
        if (enemyAnimator) enemyAnimator.SetTrigger("Attack");

        StartCoroutine(DamageDelay(isPlayerAttacking: false, delay: 0.55f, bonusDamage: 0));
    }

    IEnumerator FlashColor(SpriteRenderer sr, Color color)
    {
        if (sr == null) yield break;
        sr.color = color;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
    }

    void BattleLogEkle(string msg)
    {
        if (!battleLogText) return;
        battleLogText.text += "\n" + msg;
    }

    void DusmaniYukle()
    {
        currentEnemyName = gm.GetCurrentEnemyName();

        switch (currentEnemyName)
        {
            case "Acemi Haydut": enemyMaxHealth = 300; gm.isBossFight = false; break;
            case "Haydut": enemyMaxHealth = 300; gm.isBossFight = false; break;
            case "Zehirli Örümcek": enemyMaxHealth = 120; gm.isBossFight = false; break;
            case "Hantal HP": enemyMaxHealth = 500; gm.isBossFight = true; break;
            default: enemyMaxHealth = 100; gm.isBossFight = false; break;
        }

        enemyHealth = enemyMaxHealth;

        if (enemyHealthText)
        {
            enemyHealthText.text = currentEnemyName;
            enemyHealthText.color = gm.isBossFight ? Color.red : Color.white;
        }

        if (enemyHealthSlider)
        {
            enemyHealthSlider.maxValue = enemyMaxHealth;
            enemyHealthSlider.value = enemyHealth;
        }

        BattleLogEkle($">> {currentEnemyName} ile karşı karşıyasın!");
    }

    void EkraniGuncelle()
    {
        if (playerHealthSlider) playerHealthSlider.value = playerHealth;
        if (playerManaSlider) playerManaSlider.value = playerMana;
        if (enemyHealthSlider) enemyHealthSlider.value = enemyHealth;

        if (playerHealthText) playerHealthText.text = $"HP: {playerHealth}";
        if (playerManaText) playerManaText.text = $"MP: {playerMana}";
    }

    void Victory()
    {
        gm.SavasKazanildi();
        if (victoryPanel) victoryPanel.SetActive(true);
        CancelInvoke();
        Invoke(nameof(GoToStory), 2f);
    }

    void Defeat()
    {
        if (defeatPanel) defeatPanel.SetActive(true);
        CancelInvoke();
        Invoke(nameof(GoToMenu), 3f);
    }

    void GoToStory() => SceneManager.LoadScene("StoryScene");
    void GoToMenu() => SceneManager.LoadScene("MenuScene");
}
