using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Tilemaps;

[Serializable]
public class Node
{
    public bool isWall;
    public Node parentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }

    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }
}


public class GameManager : MonoBehaviour
{
    // Transform
    private Vector2Int bottomLeft, topRight, startPos, targetPos;

    [Header("Path List")]
    [SerializeField] private List<Node> pathNodeList = null;

    private bool allowDiagonal, dontCrossCorner;

    private int sizeX, sizeY;
    private Node[,] nodes;
    private Node startNode, targetNode, curNode;
    private List<Node> openNodeList, closeNodeList;

    public void PathFinding()
    {   
        topRight = new Vector2Int(1, 4);
        bottomLeft = new Vector2Int(-9, -5);

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        nodes = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach(Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i,j),0.4f))
                {
                    if(col.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        isWall = true;
                    }
                }
                nodes[i, j] = new Node(isWall, i, j);
            }
        }

        Transform start = GameObject.Find("StartPos").transform;
        Transform target = GameObject.Find("EndPos").transform;

        startPos = new Vector2Int((int)(start.position.x - bottomLeft.x), (int)(start.position.y - bottomLeft.y));
        targetPos = new Vector2Int((int)(target.position.x - bottomLeft.x), (int)(target.position.y - bottomLeft.y));

        startNode = nodes[startPos.x, startPos.y];
        targetNode = nodes[targetPos.x, targetPos.y];

        openNodeList = new List<Node>() { startNode };
        closeNodeList = new List<Node>();
        pathNodeList = new List<Node>();

        while (openNodeList.Count > 0)
        {
            curNode = openNodeList[0];
            for (int i = 1; i < openNodeList.Count; i++)
            {
                if(openNodeList[i].F <= curNode.F && openNodeList[i].H < curNode.H)
                {
                    curNode = openNodeList[i];
                }
            }

            openNodeList.Remove(curNode);
            closeNodeList.Add(curNode);

            if (curNode == targetNode)
            {   // 마지막
                Node targetCurNode = targetNode;
                while (targetCurNode != startNode)
                {
                    pathNodeList.Add(targetCurNode);
                    targetCurNode = targetCurNode.parentNode;
                }

                pathNodeList.Add(startNode);
                pathNodeList.Reverse();

                for(int i = 0; i < pathNodeList.Count; i++)
                {
                    Debug.Log($"{i}번째는 {pathNodeList[i].x} , {pathNodeList[i].y}");
                }

                DrawPath();
                return;
            }

            OpenNodeListAdd(curNode.x, curNode.y + 1);
            OpenNodeListAdd(curNode.x + 1, curNode.y);
            OpenNodeListAdd(curNode.x, curNode.y - 1);
            OpenNodeListAdd(curNode.x - 1, curNode.y);
        }
    }

    private void OpenNodeListAdd(int _x, int _y)
    {
        if (_x >= 0 && _x < sizeX && _y >= 0 && _y < sizeY && !nodes[_x, _y].isWall && !closeNodeList.Contains(nodes[_x, _y]))
        {   // 상하좌우 범위 안벗어나고, 벽이 아니고, 닫힌 리스트에 없다면
            if (allowDiagonal)
            {   // 대각선 허용시, 벽사이 통과 불가
                if (nodes[curNode.x, _y].isWall && nodes[_x, curNode.y].isWall) return;
            }

            if (dontCrossCorner)
            {   // 코너를 가로질러 가지 않을 시, 이동 중에 수직수평 장애물이 있으면 안됨
                if (nodes[curNode.x, _y].isWall || nodes[_x, curNode.y].isWall) return;
            }

            // 이웃 노드에 넣고, 직선은 10, 대각선은 14비용
            Node neighborNode = nodes[_x, _y];
            int moveCost = curNode.G + (curNode.x - _y == 0 || curNode.y - _y == 0 ? 10 : 14);
            if (moveCost < neighborNode.G || !openNodeList.Contains(neighborNode))
            {   // 이동 비용이 이웃노드 G보다 작거나 또는 열린 리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린 리스트에 추가
                neighborNode.G = moveCost;
                neighborNode.H = (Mathf.Abs(neighborNode.x - targetNode.x) + Mathf.Abs(neighborNode.y - targetNode.y)) * 10;
                neighborNode.parentNode = curNode;

                openNodeList.Add(neighborNode);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (pathNodeList.Count != 0)
        {
            for (int i = 0; i < pathNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(pathNodeList[i].x, pathNodeList[i].y),  new Vector2(pathNodeList[i + 1].x, pathNodeList[i + 1].y));
            }
        }
    }

    private void DrawPath()
    {
        if(pathNodeList.Count != 0)
        {
            Tilemap tilemap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
            
            for (int i = 0; i < pathNodeList.Count - 1; i++)
            {
                //Gizmos.DrawLine(new Vector2(pathNodeList[i].x, pathNodeList[i].y), new Vector2(pathNodeList[i + 1].x, pathNodeList[i + 1].y));
                tilemap.SetTileFlags(new Vector3Int(pathNodeList[i].x, pathNodeList[i].y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(pathNodeList[i].x, pathNodeList[i].y, 0), Color.blue);
            }
        }
    }

    [Header("Active Items")]
    [SerializeField] private ItemBox[] active_itemboxes = null;

    [Header("Success Items")]
    [SerializeField] private List<ItemBox> success_itemboxes = null;

    // WIN
    private GameObject winTextObj;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        //SetItems();

        //GameObject uiCanvas = GameObject.Find("UICanvas");
        //winTextObj = uiCanvas.transform.Find("WinText").gameObject;
    }

    private void Update()
    {
        if(isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }
    }

    private void SetItems()
    {
        active_itemboxes = FindObjectsOfType<ItemBox>();
        success_itemboxes = new List<ItemBox>();
    }

    public void IncreaseItemBox(ItemBox _itembox)
    {
        if(success_itemboxes.Count > 0)
        {
            if (success_itemboxes.Find(x => x == _itembox)) return;
        }
        success_itemboxes.Add(_itembox);
        GameChecker();
    }

    public void DecreaseItemBox(ItemBox _itembox)
    {
        if (success_itemboxes.Count <= 0) return;
        if (success_itemboxes.Find(x => x == _itembox))
        {
            success_itemboxes.Remove(_itembox);
        }
    }

    private void GameChecker()
    {
        if (success_itemboxes.Count == active_itemboxes.Length && !isGameOver)
        {
            isGameOver = true;
            winTextObj.SetActive(true);
        }
    }
}
