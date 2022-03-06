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

    private void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        if (gameMgr.IsGameOver) return;

        base.Move();

    }
}
