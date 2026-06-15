using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SOAP
{
    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save_data.json");
        private static SaveData _currentData = new SaveData();
        private static bool _isLoaded = false;

        [System.Serializable]
        public class SaveEntry
        {
            public string key;
            public string value;
        }

        [System.Serializable]
        public class SaveData
        {
            public List<SaveEntry> entries = new List<SaveEntry>();

            public void Set(string key, string value)
            {
                var entry = entries.Find(e => e.key == key);
                if (entry != null)
                {
                    entry.value = value;
                }
                else
                {
                    entries.Add(new SaveEntry { key = key, value = value });
                }
            }

            public string Get(string key)
            {
                var entry = entries.Find(e => e.key == key);
                return entry?.value;
            }
        }

        public static void RequestSave(string key, string value)
        {
            if (!_isLoaded) Load();
            _currentData.Set(key, value);
            Save();
        }

        public static string RequestLoad(string key)
        {
            if (!_isLoaded) Load();
            return _currentData.Get(key);
        }

        private static void Save()
        {
            string json = JsonUtility.ToJson(_currentData, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Saved data to {SavePath}");
        }

        private static void Load()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                _currentData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("[SaveSystem] Data loaded.");
            }
            else
            {
                _currentData = new SaveData();
                Debug.Log("[SaveSystem] No save file found, created new data.");
            }
            _isLoaded = true;
        }

        public static void Clear()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            _currentData = new SaveData();
            Debug.Log("[SaveSystem] Save data cleared.");
        }
    }
}