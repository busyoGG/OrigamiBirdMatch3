using System;
using ReflectionUI;
using Script;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultView: BaseView
{
    public bool win = true;

    [UIDataBind(UIType.TextField,"0")]
    private StringUIProp _text { get; set; }

    protected override void OnShow()
    {
        if (win)
        {
            _text.Set("You Win");
        }
        else
        {
            _text.Set("You Lose");
        }
    }

    [UIActionBind(UIAction.Click,"1")]
    public void BackToMenu(PointerEventData data)
    {
        UIManager.Ins().HideUI(uiNode);
        UIManager.Ins().ShowUI("StartMenu");
    }
}