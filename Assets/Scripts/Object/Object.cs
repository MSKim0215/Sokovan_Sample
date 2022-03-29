using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    protected enum Type
    {
        Box, Finish
    }

    [Header("Type")]
    [SerializeField] protected Type type = Type.Box;

    // Renderer
    protected Renderer render { private set; get; }

    protected virtual void Awake()
    {
        render = GetComponent<Renderer>();
    }

    protected virtual void Start()
    {
        SetItemType();
    }

    #region Parent
    private void SetItemType() => type = (Type)Enum.Parse(typeof(Type), tag, true);
    #endregion

    #region Child
    protected void ChangeToMaterialColor(Color _color) => render.material.color = _color;
    #endregion
}
