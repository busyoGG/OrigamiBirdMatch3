using System;
using System.Collections;
using System.Collections.Generic;
using EventUtils;
using GameObjectUtils;
using ReflectionUI;
using Timer;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EffectType
{
    None,
    BombH,
    BombV,
    BombCross,
    BombRect,
    Clear,
    Bonus
}

public enum BlockType
{
    Fruit,
    Ice
}

public class GridScript : MonoBehaviour,IGrid
{

    public string id;

    public int x;

    public int y;

    public int gridType;

    public BlockType blockType;

    public EffectType effect;

    public Vector3 pos
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }

    public void SetFront()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void SetBack()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public void Reset()
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void SetEffect(EffectType eff)
    {
        effect = eff;
        GameObject obj = null;
        switch (eff)
        {
            case EffectType.BombH:
                obj = ObjManager.Ins().GetRes("Prefabs/horizontal");
                obj.name = "Prefabs/horizontal";
                break;
            case EffectType.BombV:
                obj = ObjManager.Ins().GetRes("Prefabs/vertical");
                obj.name = "Prefabs/vertical";
                break;
        }

        if (obj != null)
        {
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
        }
    }

    public void Register()
    {
        if (blockType == BlockType.Ice)
        {
            EventManager.AddListening(id, "Ice_Break", arr =>
            {
                GridScript removedOne = arr[0] as GridScript;
                int checkX = Math.Abs(x - removedOne.x);
                int checkY = Math.Abs(y - removedOne.y);
                if (checkX == 1 && checkY == 0 || checkX == 0 && checkY == 1)
                {
                    //在旁边爆炸
                    GridManager.Ins().Remove(this);
                }
            });
        }
    }

    public void UnRegister()
    {
        //加延迟 防止foreach报错
        TimerUtils.Once(100, () => { EventManager.RemoveAll(id); });
    }

    public void OnRemove(bool doAction = true)
    {
        if (doAction)
        {
            if (blockType == BlockType.Fruit)
            {
                EventManager.TriggerEvent("Ice_Break", new ArrayList() { this });
            }

            GridManager.Ins().DoEffect(this);
            
            SkillManager.Ins().AddCount(this,"self");
        }

        if (transform.childCount > 0)
        {
            var child = transform.GetChild(0).gameObject;
            ObjManager.Ins().Recycle(child.name, child);
            effect = EffectType.None;
        }
    }

    private void OnMouseDown()
    {
        GridManager.Ins().SetSelect(this);
    }

    private void OnMouseUp()
    {
        GridManager.Ins().Release();
    }

    private void OnMouseOver()
    {
        GridManager.Ins().SetHover(this);
    }

    private void OnMouseExit()
    {
    }
}