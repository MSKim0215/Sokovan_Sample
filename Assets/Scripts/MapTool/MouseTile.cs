using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseTile : MonoBehaviour
{
    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponentInParent<Tilemap>();
    }

    private void OnMouseOver()
    {
        try
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector3.zero);
            if(this.tilemap = hit.transform.GetComponent<Tilemap>())
            {
                this.tilemap.RefreshAllTiles();
                int x, y;
                x = this.tilemap.WorldToCell(ray.origin).x;
                y = this.tilemap.WorldToCell(ray.origin).y;
                Vector3Int v3Int = new Vector3Int(x, y, 0);

                this.tilemap.SetTileFlags(v3Int, TileFlags.None);
                this.tilemap.SetColor(v3Int, (Color.red));
            }
        }
        catch(NullReferenceException)
        {
        }
    }

    private void OnMouseExit()
    {
        this.tilemap.RefreshAllTiles();
    }
}
