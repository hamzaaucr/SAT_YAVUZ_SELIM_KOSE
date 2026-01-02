using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Savaş UI")]
    public TextMeshProUGUI battleText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyNameText;

    [Header("Savaş Butonları")]
    public Button attackButton;
    public Button skillButton;
    public Button defendButton;
    public Button itemButton;
    public Button escapeButton;

    private int enemyHealth;
    private int enemyMaxHealth;
    private string enemyName;
    private int enemyDamage;
    private bool playerTurn = true;

    void Start()
    {
        ButonlariBagla();
        SavasBaslat();
    }

    void ButonlariBagla()
    {
        attackButton.onClick.AddListener(Saldir);
        defendButton.onClick.AddListener(Savun);
        escapeButton.onClick.AddListener(Kac);
    }

    void SavasBaslat()
    {
        string[] enemies = { "Vahşi Kurt", "Ork Savaşçı", "Zehirli Örümcek", "Kara Hayalet" };
        enemyName = enemies[Random.Range(0, enemies.Length)];
        enemyMaxHealth = 30 + (GameManager.Instance.playerLevel * 10);
        enemyHealth = enemyMaxHealth;
        enemyDamage = 8 + (GameManager.Instance.playerLevel * 2);

        battleText.text = $"{enemyName} ile karşılaştın!\nSavaş başlıyor!";
        EkraniGuncelle();
    }

    void EkraniGuncelle()
    {
        playerHealthText.text = $"Can: {GameManager.Instance.playerHealth}/{GameManager.Instance.playerMaxHealth}";
        enemyHealthText.text = $"Can: {enemyHealth}/{enemyMaxHealth}";
        enemyNameText.text = enemyName;
    }

    public void Saldir()
    {
        if (!playerTurn) return;

        int damage = Random.Range(10, 20) + (GameManager.Instance.playerLevel * 2);
        enemyHealth -= damage;
        battleText.text = $"{enemyName}'a saldırdın!\n{damage} hasar verdin!";

        if (enemyHealth <= 0)
        {
            SavasBitti(true);
            return;
        }

        playerTurn = false;
        Invoke("DusmanSaldirisi", 1.5f);
    }

    void DusmanSaldirisi()
    {
        int damage = Random.Range(enemyDamage - 2, enemyDamage + 3);
        GameManager.Instance.playerHealth -= damage;
        battleText.text = $"{enemyName} saldırdı!\n{damage} hasar aldın!";

        if (GameManager.Instance.playerHealth <= 0)
        {
            SavasBitti(false);
            return;
        }

        playerTurn = true;
        EkraniGuncelle();
    }

    void Savun()
    {
        if (!playerTurn) return;
        battleText.text = "Savunma pozisyonu aldın!\nGelen hasar azalacak.";
        playerTurn = false;
        Invoke("DusmanSaldirisi", 1.5f);
    }

    void Kac()
    {
        battleText.text = "Savaştan kaçtın!";
        Invoke("StorySahnesineDon", 2f);
    }

    void SavasBitti(bool kazandi)
    {
        if (kazandi)
        {
            int kazanilanPara = 20 + (GameManager.Instance.playerLevel * 5);
            GameManager.Instance.playerMoney += kazanilanPara;
            GameManager.Instance.playerXP += 25;

            battleText.text = $"{enemyName}'i yendin!\n+{kazanilanPara} altın kazandın!\n+25 XP kazandın!";
        }
        else
        {
            battleText.text = "Yenildin! Bilinçsizce uyanıyorsun...";
            GameManager.Instance.playerHealth = GameManager.Instance.playerMaxHealth / 2;
        }

        Invoke("StorySahnesineDon", 3f);
    }

    void StorySahnesineDon()
    {
        GameManager.Instance.StorySahnesineGit();
    }
}
