using System;
using System.Collections;
using System.Collections.Generic;
using EventUtils;
using Timer;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridScript : MonoBehaviour
{
    public enum BlockType
    {
        Fruit,
        Ice
    }

    public string id;
    
    public int x;

    public int y;

    public int gridType;

    public BlockType blockType;

    public Vector3 pos
    {
        get => transform.localPosition;
        // set
        // {
        //     Vector3 temp = value;
        //     temp.y = -temp.y;
        //     transform.position = temp;
        // }
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
    
    public void Register()
    {
        if (blockType == BlockType.Ice)
        {
            EventManager.AddListening(id,"Ice_Break", arr =>
            {
                GridScript removedOne = arr[0] as GridScript;
                if (Math.Abs(x - removedOne.x) <= 1 && Math.Abs(y - removedOne.y) <= 1)
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
        TimerUtils.Once(100, () =>
        {
            Debug.Log("取消注册");
            EventManager.RemoveAll(id);
        });
    }

    public void OnRemove()
    {
        if (blockType == BlockType.Fruit)
        {
            Debug.Log("触发特殊事件");
            EventManager.TriggerEvent("Ice_Break", new ArrayList() { this });
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