using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayCanvas : UICanvas
{
    // Start is called before the first frame update
    [SerializeField] private PlayerMove playerMove;
    public List<Image> PlayerPoint;
    public List<Image> EnemyPoint;
    void OnEnable()
    {
        if(playerMove == null)
        {
            playerMove = FindObjectOfType<PlayerMove>();
        }
    }
    void Update()
    {
        if(playerMove == null)
        {
            playerMove = FindObjectOfType<PlayerMove>();
        }
    }
    public void pauseBtn()
    {
        UIManager.Instance.OpenUI<PauseCanvas>();
        Time.timeScale = 0;
    }
    public void SetMove(int hor)
    {
        playerMove.horizontalInput = hor;
    }
}
