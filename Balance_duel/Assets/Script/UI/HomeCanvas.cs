using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HomeCanvas : UICanvas
{
    // Start is called before the first frame update
    public void PlayBtn()
    {
        SceneManager.LoadScene("GamePlay");
        //StartCoroutine(Play());
        UIManager.Instance.CloseUIDirectly<HomeCanvas>();
        UIManager.Instance.OpenUI<GamePlayCanvas>();
    }
    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.2f);
        
    }
}