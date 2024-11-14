using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCanvas : UICanvas
{
    // Start is called before the first frame update
     public void homeBtn()
    {
        Time.timeScale = 1;
        StartCoroutine(returnHome());

    }
    IEnumerator returnHome()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Home");
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.CloseUIDirectly<HomeCanvas>();
        UIManager.Instance.OpenUI<GamePlayCanvas>();
    }
}
