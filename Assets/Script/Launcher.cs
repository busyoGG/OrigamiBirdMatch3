using System.Collections;
using System.Collections.Generic;
using PoolUtils;
using PosTween;
using ReflectionUI;
using Script;
using Timer;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform bg;

    void Start()
    {
        TimerUtils.Init();
        ObjPoolManager.Ins().Init();
        PosTweenUtils.Init();
        
        GameManager.Ins().Init();
        
        GridManager.Ins().Init(666, 7, bg);
        GridManager.Ins().CreatePanel();

        UITweenManager.Ins().Init();
        UIManager.Ins().Init();
        UIManager.Ins().ShowUI<MainView>("Resources/UI", "MainView");

        // string json = FileUtils.ReadFile(Application.dataPath + "/Resources/Json/test.json");
        // GridManager.Ins().CreatePanel(json);
    }

    // Update is called once per frame
    void Update()
    {
    }
}