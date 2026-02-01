using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "run_save.json");

    public static void Save(SaveData data)
    {
        data.hasRun = true;
        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
    }

    public static bool TryLoad(out SaveData data)
    {
        data = null;

        if (!File.Exists(SavePath))
            return false;

        data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        return data != null && data.hasRun;
    }

    public static bool HasSave() => File.Exists(SavePath);

    public static void Clear()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
