using System;
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
        SkillManager.Ins().Init();
        
        int seed = DateTime.Now.Ticks.GetHashCode();
        
        GridManager.Ins().Init(seed, 7, bg);
        GridManager.Ins().CreatePanel();

        // string json = FileUtils.ReadFile(Application.dataPath + "/Resources/Json/test.json");
        // GridManager.Ins().CreatePanel(json);

        UITweenManager.Ins().Init();
        UIManager.Ins().Init();
        UIManager.Ins().ShowUI<MainView>("Resources/UI", "MainView");
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_WEBGL
        TimerUtils.Update();
#endif
    }
}