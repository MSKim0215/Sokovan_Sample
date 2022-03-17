using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private GameManager gameMgr;

    protected override void Awake()
    {
        base.Awake();
        gameMgr = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        Move();
    }

    protected override void Move()
    {
        if (gameMgr.IsGameOver) return;

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
            targetPos = new Vector2(transform.position.x - 1, transform.position.y);
            MoveEvent(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
            targetPos = new Vector2(transform.position.x + 1, transform.position.y);
            MoveEvent(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            targetPos = new Vector2(transform.position.x, transform.position.y + 1);
            MoveEvent(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            targetPos = new Vector2(transform.position.x, transform.position.y - 1);
            MoveEvent(Vector2.down);
        }
    }

    private void MoveEvent(Vector2 _vec)
    {
        if (MoveCheck((int)targetPos.x, (int)targetPos.y, _vec))
        {
            transform.position = Vector2.Lerp(transform.position, targetPos, 1f);
            gameMgr.RemovePath();
        }
    }

    private bool MoveCheck(int _x, int _y, Vector2 _vec)
    {
        if(_x >= gameMgr.BottomLeft.x && _x < gameMgr.TopRight.x + 1 && _y >= gameMgr.BottomLeft.y && _y < gameMgr.TopRight.y + 1)
        {
            if(!gameMgr.Nodes[_x - gameMgr.BottomLeft.x, _y - gameMgr.BottomLeft.y].isWall)     // 벽 체크
            {
                for (int i = 0; i < gameMgr.Active_ItemBoxes.Count; i++)
                {
                    if(gameMgr.Active_ItemBoxes[i].transform.position.x == _x && gameMgr.Active_ItemBoxes[i].transform.position.y == _y)
                    {
                        if (gameMgr.Active_ItemBoxes[i].MoveCheck(_vec)) return true;
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        return false;
    }
}
