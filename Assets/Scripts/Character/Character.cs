using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected enum State
    {
        IDLE, MOVE
    }

    [Header("State")]
    [SerializeField] protected State state = State.IDLE;

    // Transform
    protected Vector3 targetPos;
    protected int moveX;
    protected int moveZ;

    // Physic
    protected Rigidbody rigid;

    // Renderer
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Move()
    {
    }
}
