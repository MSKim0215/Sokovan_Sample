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

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        float gravity = rigid.velocity.y;

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        velocity *= speed;
        velocity.y = gravity;
        rigid.velocity = velocity;
    }
}
