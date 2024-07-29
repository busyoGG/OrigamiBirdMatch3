using System;
using System.Collections;
using EventUtils;
using GameObjectUtils;
using ReflectionUI;
using Timer;
using UnityEngine;

public class AiGridScript: IGrid
{
    public string id;

    public int x;

    public int y;

    public int gridType;

    public BlockType blockType;

    public EffectType effect;
    
    public void SetEffect(EffectType eff)
    {
        effect = eff;
    }

    public void Register()
    {
        if (blockType == BlockType.Ice)
        {
            EventManager.AddListening(id, "AI_Ice_Break", arr =>
            {
                AiGridScript removedOne = arr[0] as AiGridScript;
                int checkX = Math.Abs(x - removedOne.x);
                int checkY = Math.Abs(y - removedOne.y);
                if (checkX == 1 && checkY == 0 || checkX == 0 && checkY == 1)
                {
                    //在旁边爆炸
                    AIManager.Ins().Remove(this);
                }
            });
        }
    }

    public void UnRegister()
    {
        //加延迟 防止foreach报错
        TimerUtils.Once("ai_grid",100, () => { EventManager.RemoveAll(id); });
    }

    public void OnRemove(bool doAction = true)
    {
        if (doAction)
        {
            if (blockType == BlockType.Fruit)
            {
                EventManager.TriggerEvent("AI_Ice_Break", new ArrayList() { this });
            }

            AIManager.Ins().DoEffect(this);
            
            SkillManager.Ins().AddCount(this,"rival");
        }

        effect = EffectType.None;
    }
}