using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : Item
{
    private void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(60 * Time.deltaTime, 60 * Time.deltaTime, 60 * Time.deltaTime);
    }
}
