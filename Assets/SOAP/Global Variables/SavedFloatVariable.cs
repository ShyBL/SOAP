using System.Globalization;
using UnityEngine;

namespace SOAP
{
    public class SavedFloatVariable : FloatVariable
    {
        [Header("Persistence")]
        public string saveKey;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            LoadFromSave();
        }

        public void AddAndSave(float value)
        {
            Add(value);
            SaveToSystem();
        }
        
        public void SubtractAndSave(float value)
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
                if (!string.IsNullOrEmpty(loaded) && float.TryParse(loaded, out var val))
                {
                    currentValue = val;
                }
            }
        }
    }
}