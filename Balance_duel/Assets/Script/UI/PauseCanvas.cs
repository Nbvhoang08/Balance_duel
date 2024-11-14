using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseCanvas : UICanvas
{
    public Sprite OnVolume;
    public Sprite OffVolume;

    [SerializeField] private Image buttonImage;

    void Update()
    {
        UpdateButtonImage();
    }
    public void resumeBtn()
    {
        Time.timeScale = 1;
        UIManager.Instance.CloseUI<PauseCanvas>(0.2f);
    }
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
        UIManager.Instance.CloseUIDirectly<GamePlayCanvas>();
        UIManager.Instance.CloseUIDirectly<PauseCanvas>();
        UIManager.Instance.OpenUI<HomeCanvas>();
    }
    public void SoundButton()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        UpdateButtonImage();
        SoundManager.Instance.PlayVFXSound(2);
    }
    private void UpdateButtonImage()
    {
        if (SoundManager.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }

    
}
