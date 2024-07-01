using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridScript : MonoBehaviour
{
    public int x;

    public int y;

    public int gridType;

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