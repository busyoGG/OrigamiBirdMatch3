using UnityEngine;

namespace ReflectionUI
{
    public class UIFollow : MonoBehaviour
    {
        private UGUIData _obj;

        private UGUIData _parent;

        void Update()
        {
            if (_obj.visible)
            {
                // _obj.xy = FGUIUtils.GetMousePosition(_parent);
            }
        }

        public void SetObj(UGUIData obj, UGUIData parent)
        {
            _obj = obj;
            _parent = parent;
        }
    }
}