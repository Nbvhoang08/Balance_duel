using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public PlayerMove player;
    public Enemy enemy;
    public int playerScore;
    public int enemyScore;
    private bool hasUpdatedScore = false;
    private bool isResetting = false;
    private bool isGameOver = false; 
    // Start is called before the first frame update
    void Start()
    {
        playerScore = 0;
        enemyScore = 0;
    }


void Update()
{
    CheckGameStatus();
    CheckWinLoseCondition();
}
    // Update is called once per frame
 void CheckWinLoseCondition()
{
    if (!isGameOver)
    {
        if(enemyScore > 3)
        {
            UIManager.Instance.OpenUI<LoseCanvas>();
            Time.timeScale = 0f;
            isGameOver = true;  // Đánh dấu game đã kết thúc
        }
        else if(playerScore > 3)
        {   
            UIManager.Instance.OpenUI<WinCanvas>();
            Time.timeScale = 0f;
            isGameOver = true;  // Đánh dấu game đã kết thúc
        }
    }
}

void CheckGameStatus()
{
    // Chỉ kiểm tra và cập nhật điểm nếu chưa cập nhật và không đang trong quá trình reset
    if (!hasUpdatedScore && !isResetting)
    {
        if (player.isDead && !enemy.isDead)
        {
            enemyScore++;
            enemy.score += 2;
            hasUpdatedScore = true;
            StartCoroutine(Reset());
        }
        else if (enemy.isDead && !player.isDead)
        {
            playerScore++;
            player.score += 2;
            hasUpdatedScore = true;
            StartCoroutine(Reset());
        }
    }
}

IEnumerator Reset()
{
    // Đánh dấu đang trong quá trình reset
    isResetting = true;
    yield return new WaitForSeconds(1f);
    ResetGame();
}

void ResetGame()
{
    // Reset trạng thái của Player và Enemy
    player.Reset();
    enemy.Reset();
    
    // Reset các biến điều khiển
    hasUpdatedScore = false;
    isResetting = false;
}
}

