using TMPro;

namespace ReflectionUI
{
    public class UTextField: UGUIData
    {
        public string text
        {
            get => _text.text;
            set => _text.text = value;
        }

        private TextMeshProUGUI _text;

        protected override void Init()
        {
            _text = gameObject.GetComponent<TextMeshProUGUI>();
            if (_text == null)
            {
                _text = gameObject.AddComponent<TextMeshProUGUI>();
;            }
        }
    }
}