using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Item
{
    private GameManager gameMgr;

    private bool isOveraped = false;

    // Renderer
    private Color originColor;
    private Color triggerColor;

    protected override void Awake()
    {
        base.Awake();
        gameMgr = FindObjectOfType<GameManager>();
    }

    protected override void Start()
    {
        base.Start();
        SetColor();
    }

    private void SetColor()
    {
        originColor = render.material.color;
        triggerColor = Color.red;
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameMgr.IsGameOver) return;

        if(other.tag == "Finish" && !isOveraped)
        {
            isOveraped = true;
            ChangeToMaterialColor(triggerColor);

            gameMgr.IncreaseItemBox(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameMgr.IsGameOver) return;

        if(other.tag == "Finish" && isOveraped)
        {
            isOveraped = false;
            ChangeToMaterialColor(originColor);

            gameMgr.DecreaseItemBox(this);
        }
    }
}
