using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOAP

{
    public class SetTextFromFloat : MonoBehaviour
    {
        public TextMeshProUGUI textElement;
        public string prefix = "HP: ";
        public string suffix = "/";

        public void SetText(float value)
        {
            if (textElement != null) textElement.text = $"{prefix}{value}{suffix}{targetVariable.maxValue}";
        }

        [Header("Data Configuration")] [Tooltip("The Global Variable asset to get on Start (e.g., PlayerHealth).")]
        public FloatVariable targetVariable;

        private void Start()
        {
            SetText(targetVariable.currentValue);
            //GetComponent<Slider>().SetValueWithoutNotify(targetVariable.currentValue);
        }
    }
}