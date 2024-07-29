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

        UITweenManager.Ins().Init();
        UIManager.Ins().Init();
        
        int seed = DateTime.Now.Ticks.GetHashCode();
        GridManager.Ins().Init(seed, 7, bg);

        StartMenuView menu = UIManager.Ins().ShowUI<StartMenuView>("Resources/UI", "StartMenu").ui as StartMenuView;
        menu.bg = bg;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_WEBGL
        TimerUtils.Update();
#endif
    }
}