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
        //transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
    }


    protected void ParsePosition()
    {
        gameObject.transform.position = targetPos;
        moveX = (int)targetPos.x;
        moveZ = (int)targetPos.z;
    }
}
