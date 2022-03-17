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
    public Vector2Int BottomLeft => bottomLeft;
    public Vector2Int TopRight => topRight;

    [Header("Path List")]
    [SerializeField] private List<Node> pathNodeList = null;

    private bool allowDiagonal, dontCrossCorner;

    private int sizeX, sizeY;
    private Node[,] nodes;
    public Node[,] Nodes => nodes;
    private Node startNode, targetNode, curNode;
    private List<Node> openNodeList, closeNodeList;

    private void Start()
    {
        CheckNodes();
    }

    private void CheckNodes()
    {
        GameObject bottomLeftObj = GameObject.Find("NodeArea/BottomLeft");
        GameObject topRightObj = GameObject.Find("NodeArea/TopRight");
        bottomLeft = new Vector2Int(((int)bottomLeftObj.transform.position.x), ((int)bottomLeftObj.transform.position.y));
        topRight = new Vector2Int(((int)topRightObj.transform.position.x), ((int)topRightObj.transform.position.y));

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        nodes = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                nodes[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }
    }

    public void PathFinding()
    {
        CheckNodes();

        GameObject player = GameObject.Find("Player");
        GameObject end = GameObject.Find("EndPos");

        startPos = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y);
        targetPos = new Vector2Int((int)end.transform.position.x, (int)end.transform.position.y);


        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        startNode = nodes[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        targetNode = nodes[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        openNodeList = new List<Node>() { startNode };
        closeNodeList = new List<Node>();
        pathNodeList = new List<Node>();


        while (openNodeList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            curNode = openNodeList[0];
            for (int i = 1; i < openNodeList.Count; i++)
                if (openNodeList[i].F <= curNode.F && openNodeList[i].H < curNode.H) curNode = openNodeList[i];

            openNodeList.Remove(curNode);
            closeNodeList.Add(curNode);


            // 마지막
            if (curNode == targetNode)
            {
                Node TargetcurNode = targetNode;
                while (TargetcurNode != startNode)
                {
                    pathNodeList.Add(TargetcurNode);
                    TargetcurNode = TargetcurNode.parentNode;
                }
                pathNodeList.Add(startNode);
                pathNodeList.Reverse();

                //for (int i = 0; i < pathNodeList.Count; i++) print(i + "번째는 " + pathNodeList[i].x + ", " + pathNodeList[i].y);
                //if(!isPath)
                //{
                //    isPath = true;
                //    StartCoroutine(CoroutineDrawPath());
                //}
                //else
                //{
                //    isPath = false;
                //    RemovePath();
                //}
                StartCoroutine(CoroutineDrawPath());
                return;
            }

            OpenNodeListAdd(curNode.x, curNode.y + 1);
            OpenNodeListAdd(curNode.x + 1, curNode.y);
            OpenNodeListAdd(curNode.x, curNode.y - 1);
            OpenNodeListAdd(curNode.x - 1, curNode.y);
        }
    }

    void OpenNodeListAdd(int _x, int _y)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (_x >= bottomLeft.x && _x < topRight.x + 1 && _y >= bottomLeft.y && _y < topRight.y + 1 && !nodes[_x - bottomLeft.x, _y - bottomLeft.y].isWall && !closeNodeList.Contains(nodes[_x - bottomLeft.x, _y - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (nodes[curNode.x - bottomLeft.x, _y - bottomLeft.y].isWall && nodes[_x - bottomLeft.x, curNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (nodes[curNode.x - bottomLeft.x, _y - bottomLeft.y].isWall || nodes[_x - bottomLeft.x, curNode.y - bottomLeft.y].isWall) return;


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = nodes[_x - bottomLeft.x, _y - bottomLeft.y];
            int MoveCost = curNode.G + (curNode.x - _x == 0 || curNode.y - _y == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !openNodeList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - targetNode.x) + Mathf.Abs(NeighborNode.y - targetNode.y)) * 10;
                NeighborNode.parentNode = curNode;

                openNodeList.Add(NeighborNode);
            }
        }
    }

    private IEnumerator CoroutineDrawPath()
    {
        if(pathNodeList.Count != 0)
        {
            Tilemap tilemap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();
            
            for (int i = 0; i < pathNodeList.Count; i++)
            {
                tilemap.SetTileFlags(new Vector3Int(pathNodeList[i].x, pathNodeList[i].y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(pathNodeList[i].x, pathNodeList[i].y, 0), Color.blue);

                //yield return new WaitForSeconds(0.1f);
                yield return null;
            }
        }
    }

    public void RemovePath()
    {
        Tilemap tilemap = GameObject.Find("Grid/Tilemap").GetComponent<Tilemap>();

        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                tilemap.SetTileFlags(new Vector3Int(nodes[i, j].x, nodes[i, j].y, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(nodes[i, j].x, nodes[i, j].y, 0), Color.white);
            }
        }
    }

    [Header("Active Items")]
    [SerializeField] private List<ItemBox> active_itemboxes = null;
    public List<ItemBox> Active_ItemBoxes => active_itemboxes;

    [Header("Success Items")]
    [SerializeField] private List<ItemBox> success_itemboxes = null;

    [Header("Active Finish")]
    [SerializeField] private List<ItemBox> active_finishes = null;
    public List<ItemBox> Active_Finishes => active_finishes;

    // WIN
    private GameObject winTextObj;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    private bool isPath = false;

    private void Awake()
    {
        SetItems();

        //GameObject uiCanvas = GameObject.Find("UICanvas");
        //winTextObj = uiCanvas.transform.Find("WinText").gameObject;
    }

    private void Update()
    {
        if(isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }
        else if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("Main");

        PathFinding();
    }

    private void SetItems()
    {
        GameObject[] itemboxes = GameObject.FindGameObjectsWithTag("ItemBox");
        active_itemboxes = new List<ItemBox>();
        success_itemboxes = new List<ItemBox>();

        GameObject[] finishes = GameObject.FindGameObjectsWithTag("Finish");
        active_finishes = new List<ItemBox>();

        for (int i = 0; i < itemboxes.Length; i++)
        {
            active_itemboxes.Add(itemboxes[i].GetComponent<ItemBox>());
        }

        for (int i = 0; i < finishes.Length; i++)
        {
            active_finishes.Add(finishes[i].GetComponent<ItemBox>());
        }
    }

    public void IncreaseItemBox(ItemBox _itembox)
    {
        if(success_itemboxes.Count > 0)
        {
            if (success_itemboxes.Find(x => x == _itembox)) return;
        }
        success_itemboxes.Add(_itembox);
        GameChecker();

        
        if(success_itemboxes.Count >= 0)
        {
            GameObject end = GameObject.Find("EndPos");
            end.transform.position = active_finishes[success_itemboxes.Count].transform.position;
            targetPos = new Vector2Int((int)end.transform.position.x, (int)end.transform.position.y);
        }
        
    }

    public void DecreaseItemBox(ItemBox _itembox)
    {
        if (success_itemboxes.Count <= 0) return;
        if (success_itemboxes.Find(x => x == _itembox))
        {
            success_itemboxes.Remove(_itembox);
        }

        if (active_finishes.Count >= 0)
        {
            GameObject end = GameObject.Find("EndPos");
            end.transform.position = active_finishes[success_itemboxes.Count].transform.position;
            targetPos = new Vector2Int((int)end.transform.position.x, (int)end.transform.position.y);
        }
    }

    private void GameChecker()
    {
        if (success_itemboxes.Count == active_itemboxes.Count && !isGameOver)
        {
            isGameOver = true;
            winTextObj.SetActive(true);
        }
    }


}
