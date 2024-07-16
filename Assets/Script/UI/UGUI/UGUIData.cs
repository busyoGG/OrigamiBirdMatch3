using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReflectionUI
{
    public class UGUIData: MonoBehaviour,IPointerClickHandler
    {
        private RectTransform _rect;

        private CanvasGroup _cGroup;

        private GameObject _obj;

        // public string id;

        public int childCount
        {
            get => _obj.transform.childCount;
        }

        public UGUIData parent
        {
            get => _parent == null ? _obj.transform.parent.GetComponent<UGUIData>() : _parent;
            set => _parent = value;
        }

        public bool visible
        {
            get => Math.Abs(_cGroup.alpha - 1) < float.Epsilon;
            set => _cGroup.alpha = value ? 1 : 0;
        }

        private UGUIData _parent;

        public Action<PointerEventData> onClick;

        public float x
        {
            get => _rect.anchoredPosition.x;
            set
            {
                var temp = _rect.anchoredPosition;
                temp.x = value;
                _rect.anchoredPosition = temp;
            }
        }

        public float y
        {
            get => _rect.anchoredPosition.y;
            set
            {
                var temp = _rect.anchoredPosition;
                temp.y = value;
                _rect.anchoredPosition = temp;
            }
        }

        public Vector2 xy
        {
            get => _rect.anchoredPosition;
            set { _rect.anchoredPosition = value; }
        }

        public float scaleX
        {
            get => _rect.localScale.x;
            set
            {
                var temp = _rect.localScale;
                temp.x = value;
                _rect.localScale = temp;
            }
        }

        public float scaleY
        {
            get => _rect.localScale.y;
            set
            {
                var temp = _rect.localScale;
                temp.y = value;
                _rect.localScale = temp;
            }
        }

        public Vector2 scale
        {
            get => _rect.localScale;
            set => _rect.localScale = value;
        }

        public float rotation
        {
            get => _rect.localEulerAngles.z;
            set
            {
                var temp = _rect.localEulerAngles;
                temp.x = value;
                _rect.localEulerAngles = temp;
            }
        }

        public float alpha
        {
            get => _cGroup.alpha;
            set => _cGroup.alpha = value;
        }

        public float width
        {
            get => _rect.sizeDelta.x;
            set
            {
                var temp = _rect.sizeDelta;
                temp.x = value;
                _rect.sizeDelta = temp;
            }
        }

        public float height
        {
            get => _rect.sizeDelta.y;
            set
            {
                var temp = _rect.sizeDelta;
                temp.y = value;
                _rect.sizeDelta = temp;
            }
        }

        public Vector2 size
        {
            get => _rect.sizeDelta;
            set => _rect.sizeDelta = value;
        }

        private void Awake()
        {
            _obj = gameObject;

            _rect = gameObject.GetComponent<RectTransform>();
            if (_rect == null)
            {
                _rect = gameObject.AddComponent<RectTransform>();
            }

            _cGroup = gameObject.GetComponent<CanvasGroup>();
            if (_cGroup == null)
            {
                _cGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            Init();
        }

        protected virtual void Init()
        {
            
        }

        public UGUIData GetChildAt(int index)
        {
            return _obj.transform.GetChild(index).GetComponent<UGUIData>();
        }

        public UGUIData GetChild(string name)
        {
            return _obj.transform.Find(name).GetComponent<UGUIData>();
        }

        public List<UGUIData> GetChildren()
        {
            List<UGUIData> res = new();
            for (int i = 0; i < _obj.transform.childCount; i++)
            {
                var child = GetChildAt(i);
                if (child != null)
                {
                    res.Add(child);
                }
            }

            return res;
        }

        public int GetChildIndex(UGUIData node)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (node.transform.Equals(GetChildAt(i).transform))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddChild(UGUIData child)
        {
            // child.transform.parent = transform;
            child.transform.SetParent(transform,false);
        }

        public void AddChildAt(UGUIData child,int index)
        {
            AddChild(child);
            child._rect.SetSiblingIndex(index);
        }

        public UGUIData RemoveChildAt(int index)
        {
            var child = transform.GetChild(index);
            child.SetParent(null);
            return child.GetComponent<UGUIData>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(eventData);
        }

        public void Dispose()
        {
            Destroy(this);
        }

    }
}