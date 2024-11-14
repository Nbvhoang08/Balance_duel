using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerMove player;
    public Enemy enemy;
    public int playerScore;
    public int enemyScore;

    private Vector3 playerStartPosition;
    private Vector3 enemyStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerScore = 0;
        enemyScore = 0;

        // Lưu vị trí ban đầu của Player và Enemy
        playerStartPosition = player.transform.position;
        enemyStartPosition = enemy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameStatus();
    }

    void CheckGameStatus()
    {
        if (player.isDead && !enemy.isDead)
        {
            enemyScore++;
            Debug.Log("Player is dead. Enemy wins! Enemy score: " + enemyScore);
            ResetGame();
        }
        else if (enemy.isDead && !player.isDead)
        {
            playerScore++;
            Debug.Log("Enemy is dead. Player wins! Player score: " + playerScore);
            ResetGame();
        }
    }

    void ResetGame()
    {
        // Reset trạng thái của Player và Enemy cho lần chơi tiếp theo
        player.Reset();
        enemy.Reset();
    }
}

