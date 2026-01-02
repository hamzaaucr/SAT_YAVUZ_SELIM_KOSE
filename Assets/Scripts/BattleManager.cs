using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("--- ANIMATOR ---")]
    public Animator playerAnimator;
    public Animator enemyAnimator;

    [Header("--- UI ---")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI battleLogText;
    public Slider playerHealthSlider, playerManaSlider, enemyHealthSlider;

    [Header("--- PANELS ---")]
    public GameObject victoryPanel, defeatPanel;
    public TextMeshProUGUI victoryRewardText;

    [Header("--- SKILL WHEEL ---")]
    public SkillWheelUI skillWheelUI;

    [Header("--- VISUAL ---")]
    public SpriteRenderer playerSprite;
    public SpriteRenderer enemySprite;

    private GameManager gm;

    private int playerHealth, playerMana;
    private int enemyHealth, enemyMaxHealth;
    private string currentEnemyName;

    private int rewardXP;
    private int rewardGold;
    private int enemyMinDmg, enemyMaxDmg; // Düşman hasarı da artık değişken

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

        DusmaniYukle(); // Artık GameManager'dan her şeyi çekiyor
        EkraniGuncelle();

        BattleLogEkle(">> SAVAŞ BAŞLADI!");
        BattleLogEkle(">> Hamle sırası sende!");
    }

    public void KarakterTiklandi(bool isPlayerClicked)
    {
        if (!isPlayerTurn || actionLocked) return;
        if (skillWheelUI == null) return;

        var unlocked = gm.GetUnlockedSkills();
        if (unlocked == null) return;

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
            BattleLogEkle(">> Uygun skill yok veya mana yetersiz.");
            return;
        }

        actionLocked = true;
        skillWheelUI.Open(Input.mousePosition, list, SkillKullanSlotlu);
    }

    void SkillKullanSlotlu(Skill s, int slotIndex)
    {
        if (s == null) { actionLocked = false; return; }

        playerMana -= s.manaCost;
        EkraniGuncelle();

        if (s.damage > 0)
        {
            string trigger = PlayerSlotToTrigger(slotIndex);
            if (playerAnimator) playerAnimator.SetTrigger(trigger);
            StartCoroutine(DamageDelay(isPlayerAttacking: true, delay: 0.55f, bonusDamage: s.damage));
        }
        else
        {
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

            BattleLogEkle($"> Vurdun: {totalDmg} HASAR!");

            if (enemyAnimator) enemyAnimator.SetTrigger("Hurt");
            StartCoroutine(FlashColor(enemySprite, Color.red));

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                EkraniGuncelle();
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
            // --- YENİ HASAR SİSTEMİ (LİSTEDEN GELEN) ---
            int dmg = Random.Range(enemyMinDmg, enemyMaxDmg + 1);

            playerHealth -= dmg;

            BattleLogEkle($"! {currentEnemyName} vurdu: -{dmg} HP");

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
            BattleLogEkle(">> Sıra sende!");
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

    // --- OTOMATİK DÜŞMAN YÜKLEME ---
    void DusmaniYukle()
    {
        // GameManager'dan o levelin düşman bilgisini komple çekiyoruz
        EnemyInfo info = gm.GetCurrentEnemyInfo();

        currentEnemyName = info.dusmanAdi;
        enemyMaxHealth = info.maxCan;
        enemyMinDmg = info.minHasar;
        enemyMaxDmg = info.maxHasar;
        rewardXP = info.verilecekXP;
        rewardGold = info.verilecekGold;
        gm.isBossFight = info.bossMu;

        enemyHealth = enemyMaxHealth;

        // UI Güncellemeleri
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

        BattleLogEkle($">> {currentEnemyName} (Güç: {enemyMinDmg}-{enemyMaxDmg})");
        BattleLogEkle($">> Ödül: {rewardXP}XP, {rewardGold}G");
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
        gm.playerGold += rewardGold;
        gm.GainExperience(rewardXP);

        BattleLogEkle($"*** ZAFER! ***");
        BattleLogEkle($"+{rewardXP} XP kazanıldı.");
        BattleLogEkle($"+{rewardGold} Altın kazanıldı.");

        if (victoryRewardText)
        {
            victoryRewardText.text = $"TEBRİKLER!\n\nKazanılan:\n{rewardXP} XP\n{rewardGold} Altın";
        }

        gm.SavasKazanildi();
        if (victoryPanel) victoryPanel.SetActive(true);
        CancelInvoke();
        Invoke(nameof(GoToStory), 2.5f);
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