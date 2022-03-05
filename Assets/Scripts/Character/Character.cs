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

    [Header("Stat")]
    [SerializeField] protected float speed = 10f;

    // Physic
    protected Rigidbody rigid;

    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
    }

    protected virtual void Move()
    {
    }
}
