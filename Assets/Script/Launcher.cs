using System.Collections;
using System.Collections.Generic;
using PoolUtils;
using PosTween;
using Script;
using Timer;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform bg;

    public GameObject mainView;

    void Start()
    {
        TimerUtils.Init();
        ObjPoolManager.Ins().Init();
        PosTweenUtils.Init();
        
        GameManager.Ins().Init();
        
        GridManager.Ins().Init(666, 7, bg);
        GridManager.Ins().CreatePanel();

        new MainView(mainView);

        // string json = FileUtils.ReadFile(Application.dataPath + "/Resources/Json/test.json");
        // GridManager.Ins().CreatePanel(json);
    }

    // Update is called once per frame
    void Update()
    {
    }
}