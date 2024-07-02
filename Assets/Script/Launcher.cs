using System.Collections;
using System.Collections.Generic;
using PoolUtils;
using PosTween;
using Timer;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TimerUtils.Init();
        ObjPoolManager.Ins().Init();
        PosTweenUtils.Init();
        GridManager.Ins().Init(666,8);
        // GridManager.Ins().CreatePanel();

        string json = FileUtils.ReadFile(Application.dataPath + "/Resources/Json/test.json");
        GridManager.Ins().CreatePanel(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
