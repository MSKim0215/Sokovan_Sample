using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
        SetItems();

        GameObject uiCanvas = GameObject.Find("UICanvas");
        winTextObj = uiCanvas.transform.Find("WinText").gameObject;
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
