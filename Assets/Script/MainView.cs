using EventUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class MainView
    {
        private GameObject _mainView;

        private TextMeshProUGUI _selfStep;

        private TextMeshProUGUI _otherStep;

        private TextMeshProUGUI _selfScore;
        
        private TextMeshProUGUI _otherScore;

        public MainView(GameObject mainView)
        {
            _mainView = mainView;
            
            _selfStep = mainView.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            _otherStep = mainView.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            _selfScore = mainView.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

            _otherScore = mainView.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

            _selfStep.text = GameManager.Ins().GetStep().ToString();
            
            _selfScore.text = GameManager.Ins().GetScore().ToString();

            _otherStep.text = "0";
            
            _otherScore.text = "0";
            
            Register();
        }

        public void Update()
        {
            _selfScore.text = GameManager.Ins().GetScore().ToString();
            _selfStep.text = GameManager.Ins().GetStep().ToString();
        }

        public void Register()
        {
            EventManager.AddListening("MainView","MainViewUpdate", arr =>
            {
                Update();
            });
        }
    }
}