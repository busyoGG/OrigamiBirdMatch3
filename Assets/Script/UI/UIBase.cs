using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EventUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReflectionUI
{
    /// <summary>
    /// UI内部元素基类
    /// </summary>
    public class UIBase
    {
        /// <summary>
        /// 界面UI根元素
        /// </summary>
        public UGUIData main;

        /// <summary>
        /// 界面id
        /// </summary>
        public string id;

        /// <summary>
        /// 界面名
        /// </summary>
        public string name;

        /// <summary>
        /// 当前类的类型
        /// </summary>
        protected Type _type;

        /// <summary>
        /// 反射范围标志
        /// </summary>
        private readonly BindingFlags _flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                              BindingFlags.Instance;

        //----- 内建私有变量 -----

        /// <summary>
        /// 拖拽元素字典，用于保存元素的拖拽状态
        /// </summary>
        private readonly Dictionary<string, bool> _dropDic = new Dictionary<string, bool>();

        /// <summary>
        /// 代理拖拽元素
        /// </summary>
        private GameObject _copy;

        /// <summary>
        /// 当前代理拖拽脚本
        /// </summary>
        private UIDrag _uiDrag;

        /// <summary>
        /// 拖拽数据
        /// </summary>
        private readonly ArrayList _dropData = new ArrayList();

        /// <summary>
        /// 浮动窗口id
        /// </summary>
        private int _floatId = 0;

        /// <summary>
        /// 浮动窗口字典，用于保存所有浮动窗口
        /// </summary>
        private Dictionary<string, BaseView> _floatViews = new Dictionary<string, BaseView>();

        /// <summary>
        /// 正在显示的悬浮窗
        /// </summary>
        private BaseView _floatViewOnShow = null;

        protected void Bind()
        {
            _type = GetType();
            PropertyInfo[] props = _type.GetProperties(_flag);

            MethodInfo[] methods = _type.GetMethods(_flag);

            foreach (var method in methods)
            {
                var methodAttrs = method.GetCustomAttributes(true);
                foreach (var attr in methodAttrs)
                {
                    if (attr is UIActionBind)
                    {
                        BindAction(method, attr);
                    }
                    else if (attr is UIListenerBind)
                    {
                        BindListener(method, attr);
                    }
                }
            }

            foreach (var prop in props)
            {
                var propAttrs = prop.GetCustomAttributes(true);
                foreach (var attr in propAttrs)
                {
                    if (attr is UICompBind)
                    {
                        BindComp(prop, attr);
                    }
                    else if (attr is UIDataBind)
                    {
                        BindData(prop, attr);
                    }
                }
            }

            //客制化UI逻辑执行
            CustomUI();
        }

        /// <summary>
        /// 绑定组件
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="attr"></param>
        private void BindComp(PropertyInfo prop, object attr)
        {
            UICompBind uiBind = (UICompBind)attr;

            switch (uiBind._type)
            {
                case UIType.Comp:
                    UGUIData comp = UGUIUtils.GetUI<UGUIData>(main, uiBind._path);
                    prop.SetValue(this, comp);
                    break;
                case UIType.TextField:
                    // GTextField textField = FGUIUtils.GetUI<GTextField>(main, uiBind._path);
                    UTextField uTextField = UGUIUtils.GetUI<UTextField>(main, uiBind._path);
                    prop.SetValue(this, uTextField);
                    break;
                // case UIType.TextInput:
                //     GTextInput textInput = FGUIUtils.GetUI<GTextInput>(main, uiBind._path);
                //     prop.SetValue(this, textInput);
                //     break;
                // case UIType.Image:
                //     GImage image = FGUIUtils.GetUI<GImage>(main, uiBind._path);
                //     prop.SetValue(this, image);
                //     break;
                // case UIType.Loader:
                //     GLoader loader = FGUIUtils.GetUI<GLoader>(main, uiBind._path);
                //     prop.SetValue(this, loader);
                //     break;
                // case UIType.List:
                //     GList list = FGUIUtils.GetUI<GList>(main, uiBind._path);
                //     prop.SetValue(this, list);
                //     break;
                // case UIType.Slider:
                //     GSlider slider = FGUIUtils.GetUI<GSlider>(main, uiBind._path);
                //     prop.SetValue(this, slider);
                //     break;
                // case UIType.ComboBox:
                //     GComboBox comboBox = FGUIUtils.GetUI<GComboBox>(main, uiBind._path);
                //     prop.SetValue(this, comboBox);
                //     break;
            }
        }

        /// <summary>
        /// 绑定 组件-数据
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="attr"></param>
        private void BindData(PropertyInfo prop, object attr)
        {
            UIDataBind uiBind = (UIDataBind)attr;

            //获取双向绑定委托
            var onValueChange = prop.PropertyType.GetField("_onValueChange", _flag);
            var onUIChange = prop.PropertyType.GetField("_onUIChange", _flag);

            var value = prop.GetValue(this);
            if (value == null)
            {
                //初始化当前属性的值
                Type propType = prop.PropertyType;
                if (propType == typeof(StringUIProp))
                {
                    value = new StringUIProp();
                }
                else if (propType == typeof(DoubleUIProp))
                {
                    value = new DoubleUIProp();
                }
                else
                {
                    //创建List
                    Type genericType = typeof(UIListProp<>).MakeGenericType(prop.PropertyType.GenericTypeArguments);
                    value = Activator.CreateInstance(genericType);
                }

                prop.SetValue(this, value);
            }

            switch (uiBind._type)
            {
                case UIType.TextField:
                    // GTextField textField = FGUIUtils.GetUI<GTextField>(main, uiBind._path);

                    UTextField textField = UGUIUtils.GetUI<UTextField>(main, uiBind._path);
                    
                    void ActionText(string data)
                    {
                        textField.text = data;
                    }

                    string ActionTextUI()
                    {
                        return textField.text;
                    }

                    onValueChange?.SetValue(value, (Action<string>)ActionText);
                    onUIChange?.SetValue(value, (Func<string>)ActionTextUI);
                    break;
                // case UIType.TextInput:
                //     GTextInput textInput = FGUIUtils.GetUI<GTextInput>(main, uiBind._path);
                //
                //     void ActionInput(string data)
                //     {
                //         textInput.text = data;
                //     }
                //
                //     string ActionInputUI()
                //     {
                //         return textInput.text;
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<string>)ActionInput);
                //     onUIChange?.SetValue(value, (Func<string>)ActionInputUI);
                //     break;
                // case UIType.Image:
                //     GImage image = FGUIUtils.GetUI<GImage>(main, uiBind._path);
                //
                //     void ActionImage(string data)
                //     {
                //         image.icon = data;
                //     }
                //
                //     string ActionImageUI()
                //     {
                //         return image.icon;
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<string>)ActionImage);
                //     onUIChange?.SetValue(value, (Func<string>)ActionImageUI);
                //     break;
                // case UIType.Loader:
                //     GLoader loader = FGUIUtils.GetUI<GLoader>(main, uiBind._path);
                //
                //     void ActionLoader(string data)
                //     {
                //         loader.url = data;
                //
                //         // ConsoleUtils.Log("替换图片", loader?.url);
                //     }
                //
                //     string ActionLoaderUI()
                //     {
                //         return loader.url;
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<string>)ActionLoader);
                //     onUIChange?.SetValue(value, (Func<string>)ActionLoaderUI);
                //     break;
                // case UIType.List:
                //     GList list = FGUIUtils.GetUI<GList>(main, uiBind._path);
                //
                //     void ActionList(int data)
                //     {
                //         list.SetVirtual();
                //         list.numItems = data;
                //
                //         UIAdaptive uiAdaptive = prop.GetCustomAttribute<UIAdaptive>();
                //
                //         if (uiAdaptive != null)
                //         {
                //             switch (uiAdaptive.GetAdaptiveType())
                //             {
                //                 case AdaptiveType.Height:
                //                     if (list.numChildren > 0)
                //                     {
                //                         list.height = data * list.GetChildAt(0).height + list.lineGap * (data - 1) +
                //                                       list.margin.top + list.margin.bottom;
                //                     }
                //
                //                     break;
                //                 case AdaptiveType.Width:
                //                     if (list.numChildren > 0)
                //                     {
                //                         list.width = data * list.GetChildAt(0).width + list.columnGap * (data - 1) +
                //                                      list.margin.left + list.margin.right;
                //                     }
                //
                //                     break;
                //             }
                //         }
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<int>)ActionList);
                //     break;
                // case UIType.Slider:
                //     GSlider slider = FGUIUtils.GetUI<GSlider>(main, uiBind._path);
                //
                //     void ActionSlider(double data)
                //     {
                //         slider.value = data;
                //     }
                //
                //     double ActionSliderUI()
                //     {
                //         return slider.value;
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<double>)ActionSlider);
                //     onUIChange?.SetValue(value, (Func<double>)ActionSliderUI);
                //     break;
                // case UIType.ComboBox:
                //     GComboBox comboBox = FGUIUtils.GetUI<GComboBox>(main, uiBind._path);
                //
                //     UIOptions uiOptions = prop.GetCustomAttribute<UIOptions>();
                //
                //     comboBox.items = uiOptions.GetOptions();
                //
                //     void ActionComboBox(double data)
                //     {
                //         comboBox.selectedIndex = (int)data;
                //     }
                //
                //     double ActionComboBoxUI()
                //     {
                //         return comboBox.selectedIndex;
                //     }
                //
                //     onValueChange?.SetValue(value, (Action<double>)ActionComboBox);
                //     onUIChange?.SetValue(value, (Func<double>)ActionComboBoxUI);
                //
                //     break;
            }
        }

        /// <summary>
        /// 绑定 组件-行为
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attr"></param>
        private void BindAction(MethodInfo method, object attr)
        {
            UIActionBind uiBind = (UIActionBind)attr;
            // UGUIData obj = FGUIUtils.GetUI<UGUIData>(main, uiBind._path);
            UGUIData obj = UGUIUtils.GetUI<UGUIData>(main, uiBind._path);
            
            //获取方法的参数
            ParameterInfo[] methodParamsList;
            bool isAgent;

            Delegate action;

            UICondition condition = method.GetCustomAttribute<UICondition>();

            switch (uiBind._type)
            {
                case UIAction.Click:
                    action = Delegate.CreateDelegate(typeof(Action<PointerEventData>), this, method);
                    obj.onClick = (Action<PointerEventData>)action;

                    break;
                // case UIAction.ListRender:
                //     action = Delegate.CreateDelegate(typeof(ListItemRenderer), this, method);
                //     obj.asList.itemRenderer = (ListItemRenderer)action;
                //     break;
                // case UIAction.ListProvider:
                //     action = Delegate.CreateDelegate(typeof(ListItemProvider), this, method);
                //     obj.asList.itemProvider = (ListItemProvider)action;
                //     break;
                // case UIAction.ListClick:
                //     methodParamsList = method.GetParameters();
                //     if (methodParamsList.Length == 0)
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback0), this, method);
                //         obj.asList.onClickItem.Set((EventCallback0)action);
                //     }
                //     else
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback1), this, method);
                //         obj.asList.onClickItem.Set((EventCallback1)action);
                //     }
                //
                //     break;
                // case UIAction.DragStart:
                //     obj.draggable = true;
                //     isAgent = condition == null || condition.GetBool();
                //     SetDragListener(obj, 0, method, isAgent);
                //     break;
                // case UIAction.DragHold:
                //     obj.draggable = true;
                //     isAgent = condition == null || condition.GetBool();
                //     SetDragListener(obj, 1, method, isAgent);
                //     break;
                // case UIAction.DragEnd:
                //     obj.draggable = true;
                //     isAgent = condition == null || condition.GetBool();
                //     SetDragListener(obj, 2, method, isAgent);
                //     break;
                // case UIAction.Drop:
                //     action = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), this, method);
                //     _dropDic[obj.id] = true;
                //     EventManager.AddListening(obj.id, "OnDrop_" + obj.id,
                //         data => ((Action<object>)action).Invoke(data));
                //     break;
                // case UIAction.Hover:
                //     methodParamsList = method.GetParameters();
                //     if (methodParamsList.Length == 0)
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback0), this, method);
                //         obj.onRollOver.Set((EventCallback0)action);
                //     }
                //     else
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback1), this, method);
                //         obj.onRollOver.Set((EventCallback1)action);
                //     }
                //
                //     obj.onRollOver.Add(() =>
                //     {
                //         if (_floatViewOnShow != null)
                //         {
                //             if (_floatViewOnShow.main.displayObject.gameObject.GetComponent<UIFollow>() != null)
                //             {
                //                 _floatViewOnShow.main.xy = FGUIUtils.GetMousePosition();
                //             }
                //             else
                //             {
                //                 _floatViewOnShow.main.xy = obj.xy;
                //             }
                //
                //             _floatViewOnShow.Show();
                //         }
                //     });
                //
                //     //退出隐藏
                //     obj.onRollOut.Set(() =>
                //     {
                //         _floatViewOnShow?.Hide();
                //         _floatViewOnShow = null;
                //     });
                //     break;
                // case UIAction.Slider:
                //     methodParamsList = method.GetParameters();
                //     if (methodParamsList.Length == 0)
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback0), this, method);
                //         obj.asSlider.onChanged.Set((EventCallback0)action);
                //     }
                //     else
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback1), this, method);
                //         obj.asSlider.onChanged.Set((EventCallback1)action);
                //     }
                //
                //     break;
                // case UIAction.ComboBox:
                //     methodParamsList = method.GetParameters();
                //     if (methodParamsList.Length == 0)
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback0), this, method);
                //         obj.asComboBox.onChanged.Set((EventCallback0)action);
                //     }
                //     else
                //     {
                //         action = Delegate.CreateDelegate(typeof(EventCallback1), this, method);
                //         obj.asComboBox.onChanged.Set((EventCallback1)action);
                //     }
                //
                //     break;
            }
        }

        /// <summary>
        /// 绑定监听
        /// </summary>
        /// <param name="method"></param>
        /// <param name="attr"></param>
        private void BindListener(MethodInfo method, object attr)
        {
            UIListenerBind uiBind = (UIListenerBind)attr;
            var eventFunc = Delegate.CreateDelegate(typeof(Action<ArrayList>), this, method);
            EventManager.AddListening(id, uiBind._name, (Action<ArrayList>)eventFunc);
        }


        // private void ClearDropData()
        // {
        //     _dropData.Clear();
        // }
        //
        // /// <summary>
        // /// 添加放置数据
        // /// </summary>
        // /// <param name="data"></param>
        // protected void AddDropData(object data)
        // {
        //     _dropData.Add(data);
        // }
        //
        // /// <summary>
        // /// 设置拖拽，用于list内元素
        // /// </summary>
        // /// <param name="type"></param>
        // /// <param name="action"></param>
        // /// <param name="obj"></param>
        // protected void SetDrag(UIAction type, UGUIData obj, Action dragAction)
        // {
        //     obj.draggable = true;
        //     Action action = () =>
        //     {
        //         //停止本次滚动
        //         obj.parent.asList.scrollPane.CancelDragging();
        //         dragAction();
        //     };
        //     switch (type)
        //     {
        //         case UIAction.DragStart:
        //             SetDragListener(obj, 0, action);
        //             break;
        //         case UIAction.DragHold:
        //             SetDragListener(obj, 1, action);
        //             break;
        //         case UIAction.DragEnd:
        //             SetDragListener(obj, 2, action);
        //             break;
        //     }
        // }
        //
        // protected void SetDrop(UGUIData obj, Action<object> action)
        // {
        //     _dropDic[obj.id] = true;
        //     EventManager.AddListening(obj.id, "OnDrop_" + obj.id, data => action(_dropData));
        // }
        //
        // /// <summary>
        // /// 添加放置数据
        // /// </summary>
        // /// <param name="datas"></param>
        // protected void AddDropData(params object[] datas)
        // {
        //     foreach (var data in datas)
        //     {
        //         _dropData.Add(data);
        //     }
        // }

        // /// <summary>
        // /// 展示悬浮窗
        // /// </summary>
        // /// <param name="name">悬浮窗名</param>
        // /// <param name="follow">是否跟随</param>
        // /// <typeparam name="T"></typeparam>
        // protected void ShowFloatView<T>(string name, bool follow = false) where T : BaseView, new()
        // {
        //     BaseView view;
        //     _floatViews.TryGetValue(name, out view);
        //     if (view == null)
        //     {
        //         view = new T();
        //         string uiName = typeof(T).Name;
        //         view.id = "float_view_" + _floatId++;
        //         view.name = name;
        //         view.main = UIPackage.CreateObject("Test", uiName).asCom;
        //         view.main.touchable = false;
        //
        //         if (follow)
        //         {
        //             UIFollow uiFollow = view.main.displayObject.gameObject.AddComponent<UIFollow>();
        //             uiFollow.SetObj(view.main, main);
        //         }
        //
        //         main.AddChild(view.main);
        //         view.OnAwake();
        //         _floatViews.Add(name, view);
        //     }
        //
        //     _floatViewOnShow = view;
        // }

        /// <summary>
        /// 客制化 UI
        /// </summary>
        protected virtual void CustomUI()
        {
        }

        // /// <summary>
        // /// 添加拖拽监听代理
        // /// </summary>
        // /// <param name="obj">拖拽UI</param>
        // /// <param name="type">拖拽类型 0:start,1:hold,2:end</param>
        // /// <param name="method">拖拽回调</param>
        // /// <param name="isAgent">是否代理拖拽</param>
        // private void SetDragListener(UGUIData obj, int type, MethodInfo method, bool isAgent)
        // {
        //     ParameterInfo[] methodParamsList = method.GetParameters();
        //
        //     var drag = Delegate.CreateDelegate(
        //         methodParamsList.Length == 0 ? typeof(EventCallback0) : typeof(EventCallback1), this, method);
        //
        //     if (isAgent)
        //     {
        //         obj.onDragStart.Add(context =>
        //         {
        //             context.PreventDefault();
        //             //复制UI
        //             GameObject origin = obj.displayObject.gameObject;
        //             _copy = GameObject.Instantiate(origin, main.displayObject.gameObject.transform, true);
        //             CompClone(_copy.transform, origin.transform);
        //
        //             //同步属性
        //             _copy.transform.localPosition = origin.transform.localPosition;
        //             _copy.transform.localScale = origin.transform.localScale;
        //             _copy.transform.localRotation = origin.transform.localRotation;
        //
        //             //拖拽跟随逻辑
        //             _uiDrag = _copy.AddComponent<UIDrag>();
        //             _uiDrag.SetOriginMousePos();
        //
        //             Action action = () =>
        //             {
        //                 //清除放置数据
        //                 ClearDropData();
        //                 if (methodParamsList.Length == 0)
        //                 {
        //                     ((EventCallback0)drag).Invoke();
        //                 }
        //                 else
        //                 {
        //                     ((EventCallback1)drag).Invoke(null);
        //                 }
        //             };
        //
        //             switch (type)
        //             {
        //                 case 0:
        //                     _uiDrag.SetStart(action);
        //                     break;
        //                 case 1:
        //                     _uiDrag.SetUpdate(action);
        //                     break;
        //                 case 2:
        //                     _uiDrag.SetEnd(action);
        //                     break;
        //             }
        //
        //             AddDropListener(obj);
        //             RemoveDragAgent();
        //         });
        //     }
        //     else
        //     {
        //         if (methodParamsList.Length == 0)
        //         {
        //             EventCallback0 action = () =>
        //             {
        //                 //清除放置数据
        //                 ClearDropData();
        //                 ((EventCallback0)drag).Invoke();
        //             };
        //             //监听鼠标拖拽
        //             switch (type)
        //             {
        //                 case 0:
        //                     obj.onDragStart.Set(action);
        //                     break;
        //                 case 1:
        //                     obj.onDragMove.Set(action);
        //                     break;
        //                 case 2:
        //                     obj.onDragEnd.Set(action);
        //                     break;
        //             }
        //         }
        //         else
        //         {
        //             EventCallback1 action = context =>
        //             {
        //                 //清除放置数据
        //                 ClearDropData();
        //                 ((EventCallback1)drag).Invoke(context);
        //             };
        //             //监听鼠标拖拽
        //             switch (type)
        //             {
        //                 case 0:
        //                     obj.onDragStart.Set(action);
        //                     break;
        //                 case 1:
        //                     obj.onDragMove.Set(action);
        //                     break;
        //                 case 2:
        //                     obj.onDragEnd.Set(action);
        //                     break;
        //             }
        //         }
        //
        //         AddDropListener(obj);
        //     }
        // }
        //
        // /// <summary>
        // /// 设置拖拽监听
        // /// </summary>
        // /// <param name="obj">拖拽UI</param>
        // /// <param name="type">拖拽类型 0:start,1:hold,2:end</param>
        // /// <param name="dragAction">拖拽回调</param>
        // private void SetDragListener(UGUIData obj, int type, Action dragAction)
        // {
        //     obj.onDragStart.Add(context =>
        //     {
        //         context.PreventDefault();
        //         //复制UI
        //         GameObject origin = obj.displayObject.gameObject;
        //         _copy = GameObject.Instantiate(origin, main.displayObject.gameObject.transform, true);
        //         CompClone(_copy.transform, origin.transform);
        //
        //         //同步属性
        //         _copy.transform.position = origin.transform.position;
        //         _copy.transform.localScale = origin.transform.localScale;
        //         _copy.transform.rotation = origin.transform.rotation;
        //
        //         //拖拽跟随逻辑
        //         _uiDrag = _copy.AddComponent<UIDrag>();
        //         _uiDrag.SetOriginMousePos();
        //
        //         Action action = () =>
        //         {
        //             //清除放置数据
        //             ClearDropData();
        //             dragAction.Invoke();
        //         };
        //
        //         switch (type)
        //         {
        //             case 0:
        //                 _uiDrag.SetStart(action);
        //                 break;
        //             case 1:
        //                 _uiDrag.SetUpdate(action);
        //                 break;
        //             case 2:
        //                 _uiDrag.SetEnd(action);
        //                 break;
        //         }
        //
        //         AddDropListener(obj);
        //         RemoveDragAgent();
        //     });
        // }
        //
        // /// <summary>
        // /// 添加放置监听
        // /// </summary>
        // /// <param name="obj">放置UI</param>
        // private void AddDropListener(UGUIData obj)
        // {
        //     if (_uiDrag)
        //     {
        //         _uiDrag.AddEnd(() =>
        //         {
        //             UGUIData target = GRoot.inst.touchTarget;
        //             while (target != null)
        //             {
        //                 if (_dropDic.ContainsKey(target.id))
        //                 {
        //                     EventManager.TriggerEvent("OnDrop_" + target.id, _dropData);
        //                     return;
        //                 }
        //
        //                 target = target.parent;
        //             }
        //         });
        //     }
        //     else
        //     {
        //         obj.onDragEnd.Add(() =>
        //         {
        //             UGUIData target = GRoot.inst.touchTarget;
        //             while (target != null)
        //             {
        //                 if (_dropDic.ContainsKey(target.id))
        //                 {
        //                     EventManager.TriggerEvent("OnDrop_" + target.id, _dropData);
        //                     return;
        //                 }
        //
        //                 target = target.parent;
        //             }
        //         });
        //     }
        // }
        //
        // /// <summary>
        // /// 移除拖拽监听代理
        // /// </summary>
        // private void RemoveDragAgent()
        // {
        //     if (_uiDrag)
        //     {
        //         _uiDrag.AddEnd(() =>
        //         {
        //             _copy = null;
        //             _uiDrag = null;
        //         });
        //     }
        // }
        //
        // /// <summary>
        // /// 代理组件克隆及处理
        // /// </summary>
        // /// <param name="transCopy">克隆体transform</param>
        // /// <param name="transOrigin">原型transform</param>
        // private void CompClone(Transform transCopy, Transform transOrigin)
        // {
        //     MeshFilter filter = transCopy.GetComponent<MeshFilter>();
        //     MeshRenderer renderer = transCopy.GetComponent<MeshRenderer>();
        //     if (filter)
        //     {
        //         filter.mesh = transOrigin.GetComponent<MeshFilter>().mesh;
        //     }
        //
        //     if (renderer)
        //     {
        //         // renderer.materials = transOrigin.GetComponent<MeshRenderer>().materials;
        //         Material[] origin = transOrigin.GetComponent<MeshRenderer>().materials;
        //         Material[] copy = new Material[origin.Length];
        //         for (int i = 0; i < origin.Length; i++)
        //         {
        //             copy[i] = new Material(origin[i]);
        //         }
        //
        //         renderer.materials = copy;
        //         renderer.sortingOrder = 9999;
        //     }
        //
        //     if (transCopy.childCount > 0)
        //     {
        //         for (int i = 0; i < transCopy.childCount; i++)
        //         {
        //             CompClone(transCopy.GetChild(i), transOrigin.GetChild(i));
        //         }
        //     }
        // }
    }
}