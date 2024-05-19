using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIController uIController;
    private void Awake()
    {
        PoolManager.Instance.CreatePool();
    }
    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        uIController.startPanel.SetActive(true);
        uIController.drawPanel.SetActive(true);
        OnScreenDraw.OnNewShapeDraw += OnScreenDraw_OnNewShapeDraw;

    }

    private void OnScreenDraw_OnNewShapeDraw(Vector3[] obj)
    {
        uIController.startPanel.UIPanelDisable("isOff");
        uIController.inGamePanel.SetActive(true);
        OnScreenDraw.OnNewShapeDraw -= OnScreenDraw_OnNewShapeDraw;
        PlayerCluster.OnPlayerDeath += PlayerCluster_OnPlayerDeath;
    }

    private void PlayerCluster_OnPlayerDeath()
    {
        GameEnd(false);
    }

    public void GameEnd(bool isWin)
    {
        if (isWin)
        {
            uIController.gameWinPanel.SetActive(true);
            Debug.Log("WIN");
        }
        else
        {
            uIController.gameLosePanel.SetActive(true);
            Debug.Log("LOSE");
        }
        uIController.drawPanel.UIPanelDisable("isOff");
        uIController.inGamePanel.UIPanelDisable("isOff");
    }

    public void OnRestartBnt_Click()
    {
        throw new NotImplementedException();
    }
}
