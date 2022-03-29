using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : Object
{
    private GameManager gameMgr;

    private bool isOveraped = false;

    // Renderer
    private Color originColor;
    private Color triggerColor;

    [Header("Overaped Finish")]
    [SerializeField] private ItemBox overapedFinish = null;

    protected override void Awake()
    {
        base.Awake();
        gameMgr = FindObjectOfType<GameManager>();
    }

    protected override void Start()
    {
        base.Start();

        if (type == Type.Box) SetColor();
    }

    #region ItemBox
    private void SetColor()
    {
        originColor = render.material.color;
        triggerColor = Color.red;
    }

    public bool MoveCheck(Vector2 _vec)
    {
        Vector2Int targetPos = new Vector2Int((int)(transform.position.x + _vec.x), (int)(transform.position.y + _vec.y));      // 이동할 위치
        if (targetPos.x >= gameMgr.BottomLeft.x && targetPos.x < gameMgr.TopRight.x + 1 && targetPos.y >= gameMgr.BottomLeft.y && targetPos.y < gameMgr.TopRight.y + 1)
        {
            if (!gameMgr.Nodes[targetPos.x - gameMgr.BottomLeft.x, targetPos.y - gameMgr.BottomLeft.y].isWall)     // 벽 체크
            {
                if (!CheckTriggerBox(targetPos)) return false;
                else
                {
                    transform.position = Vector2.Lerp(transform.position, targetPos, 1f);
                    return true;
                }                
            }
            return false;
        }
        return false;
    }

    private bool CheckTriggerBox(Vector2Int _pos)
    {
        for(int i = 0; i < gameMgr.Active_ItemBoxes.Count;i++)
        {
            ItemBox itembox = gameMgr.Active_ItemBoxes[i];
            if(itembox != this)
            {
                if(itembox.transform.position.x == _pos.x && itembox.transform.position.y == _pos.y)
                {   // 밀려나는 방향에 상자가 있는 상태
                    return false;
                }
            }
        }
        return CheckTriggerFinish(_pos);
    }

    private bool CheckTriggerFinish(Vector2Int _pos)
    {
        for (int i = 0; i < gameMgr.Active_Finishes.Count; i++)
        {
            ItemBox finish = gameMgr.Active_Finishes[i];
            if (finish.transform.position.x == _pos.x && finish.transform.position.y == _pos.y)
            {   // 깃발이랑 같은 위치라면, 클리어 상태
                isOveraped = true;
                ChangeToMaterialColor(triggerColor);
                gameMgr.ProcessIncreaseLogic(this, finish);
                overapedFinish = finish;
                return true;
            }
        }
        if(isOveraped)
        {
            isOveraped = false;
            ChangeToMaterialColor(originColor);
            gameMgr.ProcessDecreaseLogic(this, overapedFinish);
            overapedFinish = null;
        }
        return true;
    }
    #endregion
}
