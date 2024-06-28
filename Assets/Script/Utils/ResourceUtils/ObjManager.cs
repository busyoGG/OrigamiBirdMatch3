using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ResourceUtils;
using PoolUtils;
using Object = UnityEngine.Object;

namespace GameObjectUtils
{
    public class ObjManager : Singleton<ObjManager>
    {
        private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        /// <summary>
        /// 同步获取对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject Get(string name)
        {
            GameObject obj = ObjPoolManager.Ins().Get(name);

            if (obj == null)
            {
                GameObject prefab;

                _prefabs.TryGetValue(name, out prefab);

                if (prefab == null)
                {
                    prefab = ABManager.Ins().Load<GameObject>(name);
                    _prefabs.Add(name, prefab);
                }

                obj = Object.Instantiate(prefab);

                ABManager.Ins().AddReference(name);
            }
            
            return obj;
        }

        /// <summary>
        /// 同步获取对象(Resources)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetRes(string name)
        {
            GameObject obj = ObjPoolManager.Ins().Get(name);
            if (obj == null)
            {
                GameObject prefab;

                _prefabs.TryGetValue(name, out prefab);
                
                if (prefab == null)
                {
                    prefab = Resources.Load<GameObject>(name);
                    _prefabs.Add(name, prefab);
                }

                obj = Object.Instantiate(prefab);
                
                ABManager.Ins().AddReference(name);
            }

            return obj;
        }

        /// <summary>
        /// 异步获取对象，主要用于提前加载prefab
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void GetAsync(string name, Action<GameObject> callback)
        {
            GameObject prefab;

            _prefabs.TryGetValue(name, out prefab);

            if (prefab == null)
            {
                ABManager.Ins().LoadAsync<GameObject>(name, res =>
                {
                    callback?.Invoke(res);
                    _prefabs.Add(name, prefab);
                });
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void Recycle(string name, GameObject obj)
        {
            ObjPoolManager.Ins().Recycle(name, obj);
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void Release(string name, GameObject obj)
        {
            Object.Destroy(obj);
            ABManager.Ins().AddReference(name,false);
        }
        

        /// <summary>
        /// 监测空闲资源并卸载
        /// </summary>
        public void CheckRelease()
        {
            var keys = _prefabs.Keys.ToList();
            foreach (var key in keys)
            {
                int reference = ABManager.Ins().GetReference(key);
                if (reference == 0)
                {
                    ABManager.Ins().Release(key);
                    _prefabs.Remove(key);
                }
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            ObjPoolManager.Ins().Clear();;
            ABManager.Ins().Clear();
            _prefabs.Clear();
        }
    }
}