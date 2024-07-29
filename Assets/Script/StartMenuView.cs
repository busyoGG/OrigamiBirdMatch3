using System;
using ReflectionUI;
using Script;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuView: BaseView
{
    public Transform bg;

    // [UICompBind(UIType.Comp,"0")]
    // private Button _btnStart { get; set; }


    [UIActionBind(UIAction.Click,"0")]
    public void StartGame(PointerEventData data)
    {
        Debug.Log("开始游戏");
        bg.gameObject.SetActive(true);
        GameManager.Ins().Init();
        SkillManager.Ins().Init();
        
        GridManager.Ins().CreatePanel();

        // string json = FileUtils.ReadFile(Application.dataPath + "/Resources/Json/test.json");
        // GridManager.Ins().CreatePanel(json);
        MainView mainView = UIManager.Ins().ShowUI<MainView>("Resources/UI", "MainView").ui as MainView;
        mainView.bg = bg;
        UIManager.Ins().HideUI(uiNode);
    }
}