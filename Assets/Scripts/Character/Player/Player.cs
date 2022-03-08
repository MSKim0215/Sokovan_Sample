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
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetPos = new Vector2(transform.position.x - 1, transform.position.y);
            MoveEvent();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetPos = new Vector2(transform.position.x + 1, transform.position.y);
            MoveEvent();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            targetPos = new Vector2(transform.position.x, transform.position.y + 1);
            MoveEvent();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            targetPos = new Vector2(transform.position.x, transform.position.y - 1);
            MoveEvent();
        }
    }

    private void MoveEvent()
    {
        if (MoveCheck((int)targetPos.x, (int)targetPos.y))
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 1f);
        }
    }

    private bool MoveCheck(int _x, int _y)
    {
        if(_x >= gameMgr.BottomLeft.x && _x < gameMgr.TopRight.x + 1 && _y >= gameMgr.BottomLeft.y && _y < gameMgr.TopRight.y + 1)
        {
            if(!gameMgr.Nodes[_x - gameMgr.BottomLeft.x, _y - gameMgr.BottomLeft.y].isWall)
            {
                Debug.Log("이동 가능");
                return true;
            }
            Debug.Log("벽임");
            return false;
        }
        Debug.Log("범위 밖임");
        return false;
    }
}
