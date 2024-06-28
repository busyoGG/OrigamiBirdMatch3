using System;
using System.Collections.Generic;

namespace ResourceUtils
{
    public class ABManager : Singleton<ABManager>
    {
        private Dictionary<string, ABLoader> _loaders = new Dictionary<string, ABLoader>();

        private Dictionary<string, int> _resourceCount = new Dictionary<string, int>();

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="loader"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(string name, Action<T> callback) where T : class
        {
            ABLoader loader;
            _loaders.TryGetValue(name, out loader);

            if (loader == null)
            {
                loader = new ABLoader(name);
                _loaders.Add(name, loader);
            }

            loader.LoadAsync(callback);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string name) where T : class
        {
            ABLoader loader;
            _loaders.TryGetValue(name, out loader);

            if (loader == null)
            {
                loader = new ABLoader(name);
                _loaders.Add(name, loader);
            }

            T obj = loader.Load<T>();
            return obj;
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="add">true 添加，false 减少</param>
        public void AddReference(string name, bool add = true)
        {
            if (_loaders.ContainsKey(name))
            {
                _loaders[name].AddReference(add);
            }
            else
            {
                if (!_resourceCount.TryAdd(name, 1))
                {
                    if (add)
                    {
                        _resourceCount[name]++;
                    }
                    else
                    {
                        _resourceCount[name]--;
                    }
                }
            }
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetReference(string name)
        {
            if (_loaders.ContainsKey(name))
            {
                return _loaders[name].GetReference();
            }
            else
            {
                return _resourceCount[name];
            }
        }

        /// <summary>
        /// 移除加载器
        /// </summary>
        /// <param name="name"></param>
        public void Release(string name)
        {
            ABLoader loader;
            _loaders.TryGetValue(name, out loader);
            if (loader != null)
            {
                loader.Release();
            }

            _loaders.Remove(name);
        }

        /// <summary>
        /// 完全清除
        /// </summary>
        public void Clear()
        {
            foreach (var loader in _loaders)
            {
                loader.Value.Release();
            }

            _loaders = null;
        }
    }
}