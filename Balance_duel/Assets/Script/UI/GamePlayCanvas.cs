using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayCanvas : UICanvas
{
    // Start is called before the first frame update
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private GameManager gameManager;
    public List<Image> PlayerPoint;
    public List<Image> EnemyPoint;
    public Sprite full;
    public Sprite empty;
    void OnEnable()
    {
        if(playerMove == null)
        {
            playerMove = FindObjectOfType<PlayerMove>();
        }
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
    void Update()
    {
        if(playerMove == null)
        {
            playerMove = FindObjectOfType<PlayerMove>();
        }
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }else
        {
            UpdateScoreUI(gameManager.playerScore,PlayerPoint);
            UpdateScoreUI(gameManager.enemyScore,EnemyPoint);
        }
       

    }
    public void UpdateScoreUI(int score , List<Image> ScoreImg ) 
    {
        if (ScoreImg == null || ScoreImg.Count == 0 )
        {
            return;
        }
        //Debug.Log("" + score);
        for (int i = 0; i < ScoreImg.Count; i++)
        {
            if (i < score)
            {
                ScoreImg[i].sprite = full;
            }
            else
            {
                ScoreImg[i].sprite = empty;
            }
        }
    }


    public void pauseBtn()
    {
        UIManager.Instance.OpenUI<PauseCanvas>();
        Time.timeScale = 0;
        SoundManager.Instance.PlayClickSound();
    }
    public void SetMove(int hor)
    {
        playerMove.horizontalInput = hor;
    }
}
