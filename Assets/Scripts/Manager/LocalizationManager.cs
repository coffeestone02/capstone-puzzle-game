using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language
    {
        English,
        SimplifiedChinese,
        TraditionalChinese,
        Hindi,
        Spanish,
        French,
        Arabic,
        Indonesian,
        Portuguese,
        German,
        Japanese,
        Turkish,
        Vietnamese,
        Korean
    }

    [Header("CSV File")]
    public string csvFileName = "DAB_Localization";

    [Header("Fonts - CSV Order")]
    public TMP_FontAsset englishFont;
    public TMP_FontAsset simplifiedChineseFont;
    public TMP_FontAsset traditionalChineseFont;
    public TMP_FontAsset hindiFont;
    public TMP_FontAsset spanishFont;
    public TMP_FontAsset frenchFont;
    public TMP_FontAsset arabicFont;
    public TMP_FontAsset indonesianFont;
    public TMP_FontAsset portugueseFont;
    public TMP_FontAsset germanFont;
    public TMP_FontAsset japaneseFont;
    public TMP_FontAsset turkishFont;
    public TMP_FontAsset vietnameseFont;
    public TMP_FontAsset koreanFont;

    public TMP_FontAsset fallbackFont;

    [Header("Test Override")]
    public bool useTestLanguage = false;
    public Language testLanguage = Language.English;

    private Dictionary<string, string> table = new Dictionary<string, string>();
    private bool isLoaded = false;
    private Language currentLanguage = Language.English;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reload()
    {
        table.Clear();
        isLoaded = false;
        Load();
    }

    public void Load()
    {
        if (isLoaded) return;

        currentLanguage = GetCurrentSystemOrTestLanguage();

        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);
        if (csvFile == null)
        {
            Debug.LogError($"{csvFileName}.csv 파일을 찾을 수 없음");
            isLoaded = true;
            return;
        }

        int columnIndex = GetColumnIndex(currentLanguage);

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = line.Split(',');

            if (values.Length <= columnIndex)
                continue;

            string key = values[0].Trim();
            string value = values[columnIndex].Trim();

            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }

            value = value.Replace("\"\"", "\"");

            if (!table.ContainsKey(key))
                table.Add(key, value);
        }

        isLoaded = true;
    }

    public string GetText(string key)
    {
        if (!isLoaded)
            Load();

        if (table.TryGetValue(key, out string value))
            return value;

        Debug.LogWarning("번역 키를 찾지 못함: " + key);
        return key;
    }

    public Language GetCurrentLanguage()
    {
        if (!isLoaded)
            Load();

        return currentLanguage;
    }

    public TMP_FontAsset GetCurrentFont()
    {
        switch (GetCurrentLanguage())
        {
            case Language.English: return englishFont != null ? englishFont : fallbackFont;
            case Language.SimplifiedChinese: return simplifiedChineseFont != null ? simplifiedChineseFont : fallbackFont;
            case Language.TraditionalChinese: return traditionalChineseFont != null ? traditionalChineseFont : fallbackFont;
            case Language.Hindi: return hindiFont != null ? hindiFont : fallbackFont;
            case Language.Spanish: return spanishFont != null ? spanishFont : fallbackFont;
            case Language.French: return frenchFont != null ? frenchFont : fallbackFont;
            case Language.Arabic: return arabicFont != null ? arabicFont : fallbackFont;
            case Language.Indonesian: return indonesianFont != null ? indonesianFont : fallbackFont;
            case Language.Portuguese: return portugueseFont != null ? portugueseFont : fallbackFont;
            case Language.German: return germanFont != null ? germanFont : fallbackFont;
            case Language.Japanese: return japaneseFont != null ? japaneseFont : fallbackFont;
            case Language.Turkish: return turkishFont != null ? turkishFont : fallbackFont;
            case Language.Vietnamese: return vietnameseFont != null ? vietnameseFont : fallbackFont;
            case Language.Korean: return koreanFont != null ? koreanFont : fallbackFont;
            default: return fallbackFont;
        }
    }

    private Language GetCurrentSystemOrTestLanguage()
    {
        if (useTestLanguage)
            return testLanguage;

        switch (Application.systemLanguage)
        {
            case SystemLanguage.ChineseSimplified: return Language.SimplifiedChinese;
            case SystemLanguage.ChineseTraditional: return Language.TraditionalChinese;
            case SystemLanguage.Hindi: return Language.Hindi;
            case SystemLanguage.Spanish: return Language.Spanish;
            case SystemLanguage.French: return Language.French;
            case SystemLanguage.Arabic: return Language.Arabic;
            case SystemLanguage.Indonesian: return Language.Indonesian;
            case SystemLanguage.Portuguese: return Language.Portuguese;
            case SystemLanguage.German: return Language.German;
            case SystemLanguage.Japanese: return Language.Japanese;
            case SystemLanguage.Turkish: return Language.Turkish;
            case SystemLanguage.Vietnamese: return Language.Vietnamese;
            case SystemLanguage.Korean: return Language.Korean;
            default: return Language.English;
        }
    }

    private int GetColumnIndex(Language language)
    {
        switch (language)
        {
            case Language.English: return 1;
            case Language.SimplifiedChinese: return 2;
            case Language.TraditionalChinese: return 3;
            case Language.Hindi: return 4;
            case Language.Spanish: return 5;
            case Language.French: return 6;
            case Language.Arabic: return 7;
            case Language.Indonesian: return 8;
            case Language.Portuguese: return 9;
            case Language.German: return 10;
            case Language.Japanese: return 11;
            case Language.Turkish: return 12;
            case Language.Vietnamese: return 13;
            case Language.Korean: return 14;
            default: return 1;
        }
    }
}