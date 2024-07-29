using System.Collections;
using EventUtils;
using GameObjectUtils;
using ReflectionUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script
{
    public class MainView : BaseView
    {
        // private GameObject _mainView;
        public Transform bg;

        [UIDataBind(UIType.TextField, "1")]
        private StringUIProp _selfStep { get; set; }

        [UIDataBind(UIType.TextField, "2")]
        private StringUIProp _otherStep { get; set; }

        [UIDataBind(UIType.TextField, "3")]
        private StringUIProp _selfScore { get; set; }

        [UIDataBind(UIType.TextField, "4")]
        private StringUIProp _otherScore { get; set; }

        [UICompBind(UIType.Comp, "5")]
        private UGUIData _skillPanel { get; set; }

        [UIDataBind(UIType.TextField, "7")]
        private StringUIProp _selfHp { get; set; }

        [UIDataBind(UIType.TextField, "8")]
        private StringUIProp _otherHp { get; set; }

        protected override void OnShow()
        {
            _selfStep.Set(GameManager.Ins().GetStep());

            _selfScore.Set(GameManager.Ins().GetScore());
            
            _otherScore.Set(GameManager.Ins().GetRivalScore());
            
            _otherStep.Set(GameManager.Ins().GetRivalStep());
            
            _selfHp.Set(GameManager.Ins().GetHp());
            
            _otherHp.Set(GameManager.Ins().GetRivalHp());
        }

        [UIListenerBind("MainViewUpdate")]
        public void Update(ArrayList arr)
        {
            _selfStep.Set(GameManager.Ins().GetStep());

            _selfScore.Set(GameManager.Ins().GetScore());
            
            _selfHp.Set(GameManager.Ins().GetHp());

            int skillCount = SkillManager.Ins().GetCount("self");

            int skillMax = SkillManager.Ins().GetMax("self");

            if (_skillPanel.childCount > skillCount)
            {
                for (int i = _skillPanel.childCount - 1; i >= skillCount; i--)
                {
                    var child = _skillPanel.RemoveChildAt(i);
                    ObjManager.Ins().Recycle(child.name, child.gameObject);
                }
            }
            else if (_skillPanel.childCount < skillCount && skillCount <= skillMax)
            {
                for (int i = _skillPanel.childCount; i < skillCount; i++)
                {
                    var obj = ObjManager.Ins().GetRes("UI/SkillIcon");
                    var objUI = obj.GetComponent<UGUIData>();
                    _skillPanel.AddChild(objUI);

                    objUI.y = i * 20;
                }
            }
        }

        [UIListenerBind("MainViewUpdateRival")]
        public void UpdateRival(ArrayList arr)
        {
            _otherScore.Set(GameManager.Ins().GetRivalScore());
            
            _otherStep.Set(GameManager.Ins().GetRivalStep());
            
            _otherHp.Set(GameManager.Ins().GetRivalHp());
        }

        [UIActionBind(UIAction.Click,"6")]
        public void BackToMenu(PointerEventData data)
        {
            UIManager.Ins().HideUI(uiNode);
            GridManager.Ins().Clear();
            AIManager.Ins().Clear();
            SkillManager.Ins().Clear();
            bg.gameObject.SetActive(false);
            UIManager.Ins().ShowUI("StartMenu");
        }

        [UIListenerBind("CheckResult")]
        public void CheckResult(ArrayList data)
        {
            UIManager.Ins().HideUI(uiNode);
            GridManager.Ins().Clear();
            AIManager.Ins().Clear();
            SkillManager.Ins().Clear();
            bg.gameObject.SetActive(false);
            
            ResultView res = UIManager.Ins().ShowUI<ResultView>("Resources/UI","Result").ui as ResultView;
            res.win = (bool)data[0];
        }
    }
}