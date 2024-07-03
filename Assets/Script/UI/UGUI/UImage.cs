using UnityEngine;
using UnityEngine.UI;

namespace ReflectionUI
{
    public class UImage: UGUIData
    {
        public Sprite image
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        public Color color
        {
            get => _image.color;
            set => _image.color = value;
        }

        private Image _image;
        
        protected override void Init()
        {
            _image = gameObject.GetComponent<Image>();
            if (_image == null)
            {
                _image = gameObject.AddComponent<Image>();
            }
        }
    }
}