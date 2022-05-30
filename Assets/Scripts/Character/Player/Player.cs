using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private GameManager gameMgr;
    private Dictionary<KeyCode, Action<KeyCode>> keyDic;

    protected override void Awake()
    {
        base.Awake();    
    }

    protected override void Init()
    {
        base.Init();

        gameMgr = FindObjectOfType<GameManager>();
        SetKeyEvent();
        SetSprites();
    }

    private void SetKeyEvent()
    {
        keyDic = new Dictionary<KeyCode, Action<KeyCode>>
        {
            { KeyCode.LeftArrow, CheckArrow },     // Left Down
            { KeyCode.RightArrow, CheckArrow },    // Right Down
            { KeyCode.UpArrow, CheckArrow },       // Up Down
            { KeyCode.DownArrow, CheckArrow },     // Down Down
        };
    }   // 키 이벤트 세팅 함수 , 김민섭_220329

    private void SetSprites()
    {
        Sprite[] fronts = Resources.LoadAll<Sprite>("Sprites/Characters/Player_Front");
        Sprite[] rights = Resources.LoadAll<Sprite>("Sprites/Characters/Player_Right");

        sprites = new Sprite[fronts.Length + rights.Length];

        for(int i = 0; i < sprites.Length; i++)
        {
            if (i < fronts.Length) sprites[i] = fronts[i];
            else sprites[i] = rights[i - fronts.Length];
        }
    }   // 캐릭터 스프라이트 초기화 함수 , 김민섭_220326

    private void Update()
    {
        Move();
    }

    protected override void Move()
    {
        if (gameMgr.IsGameOver) return;

        if (Input.anyKeyDown)
        {
            foreach (var dic in keyDic)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value(dic.Key);
                    break;
                }
            }
        }
    }

    private void CheckArrow(KeyCode _keyCode)
    {
        switch (_keyCode)
        {
            case KeyCode.LeftArrow:
                {
                    spriteRenderer.flipX = true;
                    targetPos = ((Vector2)transform.position + Vector2.left);
                    StartCoroutine(CoroutineMoveEvent(Vector2.left));
                    spriteRenderer.sprite = sprites[6];
                }
                break;
            case KeyCode.RightArrow:
                {
                    spriteRenderer.flipX = false;
                    targetPos = ((Vector2)transform.position + Vector2.right);
                    StartCoroutine(CoroutineMoveEvent(Vector2.right));
                    spriteRenderer.sprite = sprites[4];
                }
                break;
            case KeyCode.UpArrow:
                {
                    targetPos = ((Vector2)transform.position + Vector2.up);
                    StartCoroutine(CoroutineMoveEvent(Vector2.up));
                    spriteRenderer.sprite = sprites[3];
                }
                break;
            case KeyCode.DownArrow:
                {
                    targetPos = ((Vector2)transform.position + Vector2.down);
                    StartCoroutine(CoroutineMoveEvent(Vector2.down));
                    spriteRenderer.sprite = sprites[1];
                }
                break;
        }
    }

    private IEnumerator CoroutineMoveEvent(Vector2 _vec)
    {
        if (MoveCheck((int)targetPos.x, (int)targetPos.y, _vec))
        {
            transform.position = Vector2.Lerp(transform.position, targetPos, 1f);
            gameMgr.RemovePath();      
        }

        yield return new WaitForSeconds(0.1f);

        switch (_vec)
        {
            case Vector2 vec when vec.Equals(Vector2.left): spriteRenderer.sprite = sprites[7]; break;
            case Vector2 vec when vec.Equals(Vector2.right): spriteRenderer.sprite = sprites[5]; break;
            case Vector2 vec when vec.Equals(Vector2.up): spriteRenderer.sprite = sprites[2]; break;
            case Vector2 vec when vec.Equals(Vector2.down): spriteRenderer.sprite = sprites[0]; break;
        }
    }

    private bool MoveCheck(int _x, int _y, Vector2 _vec)
    {
        if (gameMgr.CheckToInsideArea(_x, _y))
        {
            for(int i = 0; i < gameMgr.Active_ItemBoxes.Count; i++)
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
}
