using System.Collections;
using EventUtils;
using ReflectionUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class MainView: BaseView
    {
        // private GameObject _mainView;

        [UIDataBind(UIType.TextField, "1")]
        private StringUIProp _selfStep { get; set; }

        [UIDataBind(UIType.TextField, "2")]
        private StringUIProp _otherStep{ get; set; }
        
        [UIDataBind(UIType.TextField, "3")]
        private StringUIProp _selfScore{ get; set; }
        
        [UIDataBind(UIType.TextField, "4")]
        private StringUIProp _otherScore{ get; set; }

        protected override void InitData()
        {
            _selfStep.Set(GameManager.Ins().GetStep().ToString());
            
            _selfScore.Set(GameManager.Ins().GetScore().ToString());
            
            _otherStep.Set("0");
            
            _otherScore.Set("0");
        }

        [UIListenerBind("MainViewUpdate")]
        public void Update(ArrayList arr)
        {
            _selfStep.Set(GameManager.Ins().GetStep().ToString());
            
            _selfScore.Set(GameManager.Ins().GetScore().ToString());
        }
    }
}