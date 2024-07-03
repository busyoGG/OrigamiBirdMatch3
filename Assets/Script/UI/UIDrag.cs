using System;
using UnityEngine;

namespace ReflectionUI
{
    public class UIDrag : MonoBehaviour
    {
        private Vector3 _originMousePos;

        private Vector3 _originPos;

        private Action _onStart;

        private Action _onUpdate;

        private Action _onEnd;

        // Start is called before the first frame update
        void Start()
        {
            _originPos = transform.localPosition;
            _onStart?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                transform.localPosition = _originPos + (Input.mousePosition - _originMousePos);
                _onUpdate?.Invoke();
            }
            else
            {
                _onEnd?.Invoke();
                Destroy(gameObject);
            }
        }

        public void SetOriginMousePos()
        {
            _originMousePos = Input.mousePosition;
        }

        public void AddStart(Action start)
        {
            _onStart += start;
        }

        public void SetStart(Action start)
        {
            _onStart = start;
        }

        public void AddUpdate(Action update)
        {
            _onUpdate += update;
        }

        public void SetUpdate(Action update)
        {
            _onUpdate = update;
        }

        public void AddEnd(Action end)
        {
            _onEnd += end;
        }

        public void SetEnd(Action end)
        {
            _onEnd = end;
        }
    }
}