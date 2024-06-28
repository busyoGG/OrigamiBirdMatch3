using System.Collections.Generic;
using System.Linq;
using GameObjectUtils;
using UnityEngine;

namespace PoolUtils
{
    public class ObjPoolManager : Singleton<ObjPoolManager>
    {
        enum ReleaseFlag
        {
            New,
            Old
        }
        
        private Transform _root;

        private Dictionary<string, Stack<GameObject>> _pool = new Dictionary<string, Stack<GameObject>>();

        private Dictionary<string, ReleaseFlag> _releaseFlags = new Dictionary<string, ReleaseFlag>();

        /// <summary>
        /// 水位线
        /// </summary>
        private int _waterline = 10;

        public void Init()
        {
            _root = new GameObject().transform;
            _root.name = "ObjPoolRoot";
            _root.gameObject.SetActive(false);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject Get(string name)
        {
            Stack<GameObject> stack;

            GameObject obj = null;

            _pool.TryGetValue(name, out stack);

            stack?.TryPop(out obj);

            _releaseFlags[name] = ReleaseFlag.New;

            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void Recycle(string name, GameObject obj)
        {
            //回收到对象池
            Stack<GameObject> stack;
            _pool.TryGetValue(name, out stack);

            if (stack == null)
            {
                stack = new Stack<GameObject>();
                _pool[name] = stack;
            }

            stack.Push(obj);

            //设置对象到对象池根节点

            obj.transform.SetParent(_root);
            
            _releaseFlags[name] = ReleaseFlag.New;
        }

        /// <summary>
        /// 检测闲置对象池并清除
        /// </summary>
        public void CheckRelease()
        {
            var keys = _pool.Keys.ToList();

            foreach (var key in keys)
            {
                Stack<GameObject> stack = _pool[key];

                ReleaseFlag flag = _releaseFlags[key];

                switch (flag)
                {
                    case ReleaseFlag.New:
                
                        while (stack.Count > _waterline)
                        {
                            GameObject obj = stack.Pop();
                            ObjManager.Ins().Release(key, obj);
                        }

                        _releaseFlags[key] = ReleaseFlag.Old;
                        
                        break;
                    case ReleaseFlag.Old:

                        while (stack.Count > 0)
                        {
                            GameObject obj = stack.Pop();
                            ObjManager.Ins().Release(key, obj);
                        }

                        _pool.Remove(key);
                        _releaseFlags.Remove(key);
                        
                        break;
                }
            }
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            foreach (var data in _pool)
            {
                foreach (var obj in data.Value)
                {
                    ObjManager.Ins().Release(data.Key,obj);
                }
            }
            
            _pool.Clear();
        }
    }
}