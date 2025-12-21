using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public static class SaveSystem
{
    // 저장 파일 경로
    private static string SavePath
    {
        get
        {
            return Path.Combine(UnityEngine.Application.persistentDataPath, "run_save.json");
        }
    }

    // 저장
    public static void Save(SaveData data)
    {
        data.hasRun = true;
        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
    }

    // 불러오기
    public static bool TryLoad(out SaveData data)
    {
        data = null;

        if (!File.Exists(SavePath))
            return false;

        data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        return data != null && data.hasRun;
    }

    // 저장 파일 존재 여부
    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    // 저장 삭제
    public static void Clear()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}

