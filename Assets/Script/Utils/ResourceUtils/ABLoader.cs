using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ResourceUtils
{
    public class ABLoader
    {
        private ResInfo _res;

        private bool _isLoaded = false;
        
        // private List<>

        public ABLoader(string name)
        {
            _res = new ResInfo();
            _res.name = name;
        }

        /// <summary>
        /// 增加引用
        /// </summary>
        /// <param name="add">true 增加，false 减少</param>
        public void AddReference(bool add = true)
        {
            if (add)
            {
                _res.reference++;
            }
            else
            {
                _res.reference--;
            }
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        /// <returns></returns>
        public int GetReference()
        {
            return _res.reference;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(Action<T> callback = null) where T:class
        {
            if (_isLoaded)
            {
                if (_res.handle.IsDone)
                {
                    callback?.Invoke(_res.handle.Result as T);
                }
                else
                {
                    Loading(callback);
                }
            }
            else
            {
                Loading(callback);
            }
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>() where T:class
        {
            _isLoaded = true;
            _res.handle = Addressables.LoadAssetAsync<T>(_res.name);
            T obj = _res.handle.WaitForCompletion() as T;
            _isLoaded = false;
            Debug.Log("同步加载完成");
            
            return obj;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            if (_isLoaded)
            {
                _isLoaded = false;
                Addressables.Release(_res.handle);
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        private void Loading<T>(Action<T> callback) where T:class
        {
            _isLoaded = true;
            _res.handle = Addressables.LoadAssetAsync<T>(_res.name);
            _res.handle.Completed += result =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    T obj = result.Result as T;
                    callback?.Invoke(obj);
                }
                else
                {
                    callback?.Invoke(null);
                    Debug.LogError(_res.name + " 加载失败");
                }
            };
        }
    }
}