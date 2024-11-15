using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCanvas : UICanvas
{
    // Start is called before the first frame update
    public void homeBtn()
    {
        Time.timeScale = 1;
        StartCoroutine(returnHome());
        SoundManager.Instance.PlayClickSound();

    }
    IEnumerator returnHome()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Home");
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.CloseUIDirectly<GamePlayCanvas>();
        UIManager.Instance.CloseUIDirectly<LoseCanvas>();
        UIManager.Instance.OpenUI<HomeCanvas>();
    }
}
