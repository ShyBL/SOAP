using System.Globalization;
using UnityEngine;

namespace SOAP
{
    public class SavedIntVariable : IntVariable
    {
        [Header("Persistence")]
        public string saveKey;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            LoadFromSave();
        }

        public void AddAndSave(int value)
        {
            Add(value);
            SaveToSystem();
        }
        
        public void SubtractAndSave(int value)
        {
            Subtract(value);
            SaveToSystem();
        }
        
        private void SaveToSystem()
        {
            if (Application.isPlaying && !string.IsNullOrEmpty(saveKey))
            {
                SaveSystem.RequestSave(saveKey, currentValue.ToString(CultureInfo.CurrentCulture));
            }
        }

        private void LoadFromSave()
        {
            if (Application.isPlaying && !string.IsNullOrEmpty(saveKey))
            {
                var loaded = SaveSystem.RequestLoad(saveKey);
                if (!string.IsNullOrEmpty(loaded) && int.TryParse(loaded, out var val))
                {
                    currentValue = val;
                }
            }
        }
    }
}