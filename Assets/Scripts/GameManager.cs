using UnityEngine;
using System.Collections.Generic;

// --- HİKAYE VERİ YAPILARI (GLOBAL) ---
[System.Serializable]
public class HikayeSatiri
{
    public string konusanIsim;
    [TextArea] public string cumle;
    public bool sagTarafKonusuyor;
    public Sprite karakterResmi;
}

[System.Serializable]
public class SenaryoBolumu
{
    public string bolumAdi; // Örn: "Bölüm 1 - Giriş" (Hatırlatıcı)
    public int chapterNo;   // 1
    public int fightNo;     // 1

    [TextArea(3, 10)]       // Inspector'da büyük kutu açar
    public string hikayeMetni; // Parşömende yazacak ana metin

    public List<HikayeSatiri> diyaloglar; // Karakter konuşmaları
}

// --- DİĞER VERİ YAPILARI ---
[System.Serializable] public class EsyaGorseli { public string esyaIsmi; public Sprite esyaResmi; }
[System.Serializable] public class Skill { public string skillName; public Sprite skillIcon; public int damage; public int manaCost; public string description; public bool isUnlocked; public int requiredLevel; public int pointCost; }
[System.Serializable] public class CharacterData { public string characterName; public string jobClass; public string equipHelm = "Yok"; public string equipBody = "Yok"; public string equipLegs = "Yok"; public string equipGloves = "Yok"; public string equipHandR = "Yok"; public string equipHandL = "Yok"; public string equipRing1 = "Yok"; public string equipRing2 = "Yok"; public string equipUnique1 = "Yok"; public string equipUnique2 = "Yok"; public int baseDamage = 5; public int baseHealth = 100; public int currentXP = 0; public int currentLevel = 1; public int maxMana = 100; }
[System.Serializable] public class PartyDataWrapper { public List<CharacterData> partyList; }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("--- SENARYO YÖNETİMİ ---")]
    public List<SenaryoBolumu> tumSenaryo; // TÜM HİKAYEYİ BURAYA GİRECEKSİN

    [Header("PARTİ SİSTEMİ")]
    public List<CharacterData> partyMembers = new List<CharacterData>();
    public int activeMemberIndex = 0;
    public CharacterData CurrentChar { get { if (partyMembers == null || partyMembers.Count == 0) ResetGame(); if (activeMemberIndex >= partyMembers.Count) activeMemberIndex = 0; return partyMembers[activeMemberIndex]; } }

    // Köprü Değişkenler
    public int playerLevel { get { return CurrentChar.currentLevel; } set { CurrentChar.currentLevel = value; } }
    public int playerXP { get { return CurrentChar.currentXP; } set { CurrentChar.currentXP = value; } }
    public int playerMaxMana { get { return CurrentChar.maxMana; } set { CurrentChar.maxMana = value; } }
    public int baseHealth { get { return CurrentChar.baseHealth; } set { CurrentChar.baseHealth = value; } }
    public int baseDamage { get { return CurrentChar.baseDamage; } set { CurrentChar.baseDamage = value; } }

    [Header("Ayarlar")]
    public bool isLowCpuMode = false;

    [Header("Veritabanı & Genel")]
    public int playerGold = 500;
    public List<string> playerInventory = new List<string>();
    public List<EsyaGorseli> tumEsyaResimleri;
    public List<Skill> gameSkills = new List<Skill>();
    public int skillPoints = 3;
    public bool isBossFight = false;
    public int currentChapter = 1;
    public int currentFight = 1;
    public int totalFightsPerChapter = 6;
    private string[] chapter1Enemies = { "Acemi Haydut", "Yabani Kurt", "Zehirli Örümcek", "Haydut Gözcüsü", "Karanlık Gölge", "Hantal HP" };

    public string equipHelm => CurrentChar.equipHelm;
    public string equipBody => CurrentChar.equipBody;

    void Awake() { if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); LoadGame(); } else Destroy(gameObject); }

    // --- SENARYO ÇEKME FONKSİYONU ---
    public SenaryoBolumu GetCurrentScenario()
    {
        foreach (var senaryo in tumSenaryo)
        {
            if (senaryo.chapterNo == currentChapter && senaryo.fightNo == currentFight)
            {
                return senaryo;
            }
        }
        return null;
    }

    // --- ESKİ FONKSİYONLAR (Aynen Devam) ---
    public int TotalDamage { get { var c = CurrentChar; int dmg = c.baseDamage; dmg += GetItemPower(c.equipHandR); if (!IsShield(c.equipHandL)) dmg += GetItemPower(c.equipHandL); dmg += GetItemPower(c.equipRing1) + GetItemPower(c.equipRing2); return dmg; } }
    public int TotalMaxHealth { get { var c = CurrentChar; int hp = c.baseHealth; hp += GetItemPower(c.equipHelm) + GetItemPower(c.equipBody); hp += GetItemPower(c.equipLegs) + GetItemPower(c.equipGloves); if (IsShield(c.equipHandL)) hp += GetItemPower(c.equipHandL); return hp; } }
    public void EquipItem(string item) { CharacterData c = CurrentChar; string type = GetItemType(item); if (type == "Head") SwapItem(ref c.equipHelm, item); else if (type == "Body") SwapItem(ref c.equipBody, item); else if (type == "Legs") SwapItem(ref c.equipLegs, item); else if (type == "Gloves") SwapItem(ref c.equipGloves, item); else if (type == "Weapon") { if (c.equipHandR == "Yok") SwapItem(ref c.equipHandR, item); else if (c.equipHandL == "Yok") SwapItem(ref c.equipHandL, item); else SwapItem(ref c.equipHandR, item); } else if (type == "Shield") SwapItem(ref c.equipHandL, item); else if (type == "Ring") { if (c.equipRing1 == "Yok") SwapItem(ref c.equipRing1, item); else if (c.equipRing2 == "Yok") SwapItem(ref c.equipRing2, item); else SwapItem(ref c.equipRing1, item); } else if (type == "Unique") { if (c.equipUnique1 == "Yok") SwapItem(ref c.equipUnique1, item); else if (c.equipUnique2 == "Yok") SwapItem(ref c.equipUnique2, item); else SwapItem(ref c.equipUnique1, item); } SaveGame(); }
    void SwapItem(ref string slot, string newItem) { if (slot != "Yok") playerInventory.Add(slot); slot = newItem; playerInventory.Remove(newItem); }
    public void UnequipItem(string slotName) { CharacterData c = CurrentChar; if (slotName == "Head" && c.equipHelm != "Yok") SwapItem(ref c.equipHelm, "Yok"); else if (slotName == "Body" && c.equipBody != "Yok") SwapItem(ref c.equipBody, "Yok"); else if (slotName == "Legs" && c.equipLegs != "Yok") SwapItem(ref c.equipLegs, "Yok"); else if (slotName == "Gloves" && c.equipGloves != "Yok") SwapItem(ref c.equipGloves, "Yok"); else if (slotName == "HandR" && c.equipHandR != "Yok") SwapItem(ref c.equipHandR, "Yok"); else if (slotName == "HandL" && c.equipHandL != "Yok") SwapItem(ref c.equipHandL, "Yok"); else if (slotName == "Ring1" && c.equipRing1 != "Yok") SwapItem(ref c.equipRing1, "Yok"); else if (slotName == "Ring2" && c.equipRing2 != "Yok") SwapItem(ref c.equipRing2, "Yok"); else if (slotName == "Uniq1" && c.equipUnique1 != "Yok") SwapItem(ref c.equipUnique1, "Yok"); else if (slotName == "Uniq2" && c.equipUnique2 != "Yok") SwapItem(ref c.equipUnique2, "Yok"); SaveGame(); }
    public bool IsShield(string item) { return item.Contains("Kalkan"); }
    public int GetItemPower(string item) { if (item == "Yok") return 0; if (item.Contains("Tahta")) return 5; if (item.Contains("Demir")) return 10; if (item.Contains("Çelik")) return 15; if (item.Contains("Efsanevi")) return 30; if (item.Contains("Yüzük")) return 5; return 0; }
    public string GetItemType(string item) { if (item.Contains("Kafalık") || item.Contains("Miğfer")) return "Head"; if (item.Contains("Zırh") || item.Contains("Gövde")) return "Body"; if (item.Contains("Bacak")) return "Legs"; if (item.Contains("Eldiven")) return "Gloves"; if (item.Contains("Kılıç") || item.Contains("Balta")) return "Weapon"; if (item.Contains("Kalkan")) return "Shield"; if (item.Contains("Yüzük")) return "Ring"; if (item.Contains("Tılsım") || item.Contains("Taş") || item.Contains("Kan")) return "Unique"; return "Unknown"; }
    public int GetTurnRegen() { var c = CurrentChar; int r = 0; if (c.equipUnique1.Contains("Troll") || c.equipUnique2.Contains("Troll")) r += 10; return r; }
    public Sprite ResimGetir(string isim) { foreach (var k in tumEsyaResimleri) if (k.esyaIsmi == isim) return k.esyaResmi; return null; }
    public void AddXP(int xp) { CurrentChar.currentXP += xp; if (CurrentChar.currentXP >= CurrentChar.currentLevel * 100) LevelUp(); else SaveGame(); }
    public void LevelUp() { CurrentChar.currentLevel++; skillPoints += 3; CurrentChar.baseHealth += 5; CurrentChar.currentXP -= (CurrentChar.currentLevel - 1) * 100; SaveGame(); }
    public List<Skill> GetUnlockedSkills() { List<Skill> acik = new List<Skill>(); foreach (var s in gameSkills) if (s.isUnlocked) acik.Add(s); return acik; }
    public bool TryUnlockSkill(Skill s) { if (s.isUnlocked || CurrentChar.currentLevel < s.requiredLevel || skillPoints < s.pointCost) return false; skillPoints -= s.pointCost; s.isUnlocked = true; SaveGame(); return true; }
    public void ToggleLowCpuMode() { isLowCpuMode = !isLowCpuMode; ApplyGraphicsSettings(); SaveGame(); }
    public void ApplyGraphicsSettings() { QualitySettings.vSyncCount = 0; Application.targetFrameRate = isLowCpuMode ? 30 : 60; }
    public void SaveGame() { PartyDataWrapper wrapper = new PartyDataWrapper { partyList = partyMembers }; string json = JsonUtility.ToJson(wrapper); PlayerPrefs.SetString("PartyData", json); PlayerPrefs.SetString("Inventory", string.Join(",", playerInventory)); PlayerPrefs.SetInt("PlayerGold", playerGold); PlayerPrefs.SetInt("SkillPoints", skillPoints); PlayerPrefs.SetInt("Chapter", currentChapter); PlayerPrefs.SetInt("Fight", currentFight); PlayerPrefs.SetInt("LowCpuMode", isLowCpuMode ? 1 : 0); string sStr = ""; foreach (var s in gameSkills) sStr += s.isUnlocked ? "1" : "0"; PlayerPrefs.SetString("SkillsUnlocked", sStr); PlayerPrefs.Save(); }
    public void LoadGame() { if (PlayerPrefs.HasKey("PartyData")) { string json = PlayerPrefs.GetString("PartyData"); PartyDataWrapper wrapper = JsonUtility.FromJson<PartyDataWrapper>(json); partyMembers = wrapper.partyList; string inv = PlayerPrefs.GetString("Inventory"); if (!string.IsNullOrEmpty(inv)) playerInventory = new List<string>(inv.Split(',')); playerGold = PlayerPrefs.GetInt("PlayerGold", 500); skillPoints = PlayerPrefs.GetInt("SkillPoints"); currentChapter = PlayerPrefs.GetInt("Chapter", 1); currentFight = PlayerPrefs.GetInt("Fight", 1); isLowCpuMode = PlayerPrefs.GetInt("LowCpuMode", 0) == 1; string sk = PlayerPrefs.GetString("SkillsUnlocked"); if (!string.IsNullOrEmpty(sk) && sk.Length == gameSkills.Count) for (int i = 0; i < gameSkills.Count; i++) gameSkills[i].isUnlocked = (sk[i] == '1'); } else { ResetGame(); } ApplyGraphicsSettings(); }
    public void ResetGame() { PlayerPrefs.DeleteAll(); partyMembers.Clear(); CharacterData hero = new CharacterData { characterName = "Selim", jobClass = "Savaşçı", baseDamage = 10, baseHealth = 100 }; hero.equipHandR = "Tahta Kılıç"; partyMembers.Add(hero); CharacterData friend = new CharacterData { characterName = "Dostum", jobClass = "Büyücü", baseDamage = 5, baseHealth = 80 }; friend.equipBody = "Deri Zırh"; partyMembers.Add(friend); playerInventory.Clear(); playerInventory.Add("Tahta Kalkan"); playerInventory.Add("Demir Kılıç"); playerGold = 500; currentChapter = 1; currentFight = 1; skillPoints = 3; isLowCpuMode = false; foreach (var s in gameSkills) s.isUnlocked = false; SaveGame(); }
    public string GetCurrentEnemyName() { int i = currentFight - 1; return (i < chapter1Enemies.Length) ? chapter1Enemies[i] : "Rastgele"; }
    public void AdvanceStage() { currentFight++; if (currentFight > totalFightsPerChapter) { currentFight = 1; currentChapter++; } SaveGame(); }
    public void SavasKazanildi() { AdvanceStage(); }
}