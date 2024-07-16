using System;
using System.Collections.Generic;
using GameObjectUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReflectionUI
{
    public class UIManager : Singleton<UIManager>
    {
        private UINode _root = new UINode();

        private int _id = 0;

        private List<UGUIData> _layer = new List<UGUIData>();

        private Dictionary<string, UINode> _savedView = new Dictionary<string, UINode>();

        private UGUIData _canvasRoot;

        public void Init()
        {
            // 创建一个新的 GameObject 作为 Canvas
            GameObject canvasObject = new GameObject("Canvas");

            // 添加 Canvas 组件
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // 设置 Canvas 渲染模式

            // 添加 CanvasScaler 组件（可选，根据需要）
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; // 根据屏幕大小缩放
            canvasScaler.matchWidthOrHeight = 1f;

            // 添加 GraphicRaycaster 组件（可选，用于 UI 交互）
            canvasObject.AddComponent<GraphicRaycaster>();

            // 设置 Canvas 的 RectTransform（例如设置大小和位置）
            RectTransform rectTransform = canvasObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height); // 设置 Canvas 大小为屏幕大小
            
            _canvasRoot = canvasObject.gameObject.AddComponent<UGUIData>();

            GameObject eventObj = new GameObject("EventSystem");

            eventObj.AddComponent<EventSystem>();
            eventObj.AddComponent<StandaloneInputModule>();
        }

        public UGUIData GetRoot()
        {
            return _canvasRoot;
        }

        /// <summary>
        /// 展示UI
        /// </summary>
        /// <param name="folder">UI所在文件夹</param>
        /// <param name="package">UI包名</param>
        /// <param name="name">自定义名称</param>
        /// <param name="parent">父节点</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UINode ShowUI<T>(string folder, string name, UINode parent = null)
            where T : BaseView, new()
        {
            //有保存的UI直接展示并返回
            if (_savedView.TryGetValue(name, out var node))
            {
                node.ui.Show();
                return node;
            }

            //包加载逻辑暂时加载Resources文件夹内文件 如有需要可自行修改
            string uiPath = folder + "/" + name;
            //创建UI
            T view = new T();
            view.id = "ui_" + _id++;
            view.name = name;
            if (uiPath.Contains("Resources"))
            {
                view.main = ObjManager.Ins().GetRes(uiPath.Replace("Resources/","")).GetComponent<UGUIData>();
            }

            //创建UI节点
            UINode ui = new UINode();
            ui.ui = view;

            if (parent != null)
            {
                int layerIndex = parent.layer + 1;
                if (_layer.Count - 1 == parent.layer)
                {
                    UGUIData layer = new GameObject().AddComponent<UGUIData>();
                    layer.gameObject.name = "Layer_" + layerIndex;
                    _canvasRoot.AddChild(layer);
                    _layer.Add(layer);
                    // layer.xy = Vector2.zero;
                }

                _layer[layerIndex].AddChild(view.main);

                parent.children.Add(view.id, ui);
                ui.parent = parent;
                ui.layer = layerIndex;
            }
            else
            {
                if (_layer.Count == 0)
                {
                    UGUIData layer = new GameObject().AddComponent<UGUIData>();
                    layer.gameObject.name = "Layer_0";
                    _canvasRoot.AddChild(layer);
                    _layer.Add(layer);
                    // layer.xy = Vector2.zero;
                }

                _layer[0].AddChild(view.main);

                _root.children.Add(view.id, ui);
                ui.parent = _root;
                ui.layer = 0;
            }

            // view.main.xy = Vector2.zero;
            view.uiNode = ui;

            view.OnAwake();
            view.Show();

            return ui;
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="ui">UI节点</param>
        public void HideUI(UINode ui)
        {
            foreach (var child in ui.children)
            {
                UINode uiChild = child.Value;
                HideUI(uiChild);
            }

            ui.ui.Hide();
        }

        /// <summary>
        /// 根据名字获取UI
        /// </summary>
        /// <param name="name">自定义名称</param>
        /// <param name="parent">父节点</param>
        /// <returns></returns>
        public UINode GetUI(string name, UINode parent = null)
        {
            if (parent == null)
            {
                parent = _root;
            }

            if (parent.ui != null && name == parent.ui.name)
            {
                return parent;
            }

            UINode node;

            foreach (var child in parent.children)
            {
                node = GetUI(name, child.Value);
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="ui">UI节点</param>
        public void DisposeUI(UINode ui)
        {
            foreach (var child in ui.children)
            {
                UINode uiChild = child.Value;
                DisposeUI(uiChild);
            }

            //移除保存的节点
            _savedView.Remove(ui.ui.name);
            ui.children = null;
            ui.parent = null;
            ui.ui.Dispose();
        }

        /// <summary>
        /// 重新置于上层
        /// </summary>
        /// <param name="ui">UI节点</param>
        public void ResetTop(UINode ui)
        {
            _layer[ui.layer].AddChild(ui.ui.main);
            Debug.Log("重新置顶");
        }

        /// <summary>
        /// 设置模态背景
        /// </summary>
        /// <param name="ui">UI节点</param>
        /// <param name="model">模态背景对象</param>
        public void SetModel(UINode ui, UImage model)
        {
            UGUIData layer = _layer[ui.layer];
            int index = layer.GetChildIndex(ui.ui.main);
            layer.AddChildAt(model, index);
        }

        /// <summary>
        /// 保存节点 需要ui名称唯一
        /// </summary>
        /// <param name="name">自定义名称</param>
        /// <param name="ui">UI节点</param>
        public void SaveNode(string name, UINode ui)
        {
            _savedView[name] = ui;
        }
    }
}