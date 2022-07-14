using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

public class UIManager : MonoBehaviour
{
    public TMP_Text highscoreText;
    public static UIManager instance;
    public List<UIPanel> uiPanels;

    public UIPanel activeUIPanel;
    public GameObject musicBtn;
    public GameObject soundBtn;

    public Canvas effectCanvas;

    public GameObject ComboObjPrefab;

    private GameObject comboSpwaned;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SwitchCanvas(uiPanels[0].uiPanelType);
        OnMusicVolumeChanged();
        OnSoundVolumeChanged();
        if (SoundManager.instance != null)
        {
            if (SoundManager.instance.OriginalbackGroundAudioVolume == 0)
            {
                EnableDisableMusicBtns(musicBtn, 0.5f);
            }
            else
            {
                EnableDisableMusicBtns(musicBtn, 1f);
            }
            if (SoundManager.instance.OriginalsoundeffectVolume == 0)
            {
                EnableDisableMusicBtns(soundBtn, 0.5f);
            }
            else
            {
                EnableDisableMusicBtns(soundBtn, 1f);
            }
        }
        GetHighScore();
    }
    private void Update()
    {
        if (activeUIPanel.uiPanelType == UIPanelType.howToplay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SwitchCanvas(UIPanelType.mainmenu);
            }
        }
    }
    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                highscoreText.text = d.high_score.ToString();
            }
        });
    }
    private void EnableDisableMusicBtns(GameObject MainGameBTN, float alphaVAlue)
    {
        if (MainGameBTN == null)
            return;
        MainGameBTN.GetComponent<Image>().color = new Color(
                MainGameBTN.GetComponent<Image>().color.r,
                MainGameBTN.GetComponent<Image>().color.g,
                MainGameBTN.GetComponent<Image>().color.b,
                alphaVAlue);

    }
    public void OnMusicBTNClickded()
    {
        if (musicBtn.GetComponent<Image>().color.a == 1)
        {
            EnableDisableMusicBtns(musicBtn, 0.5f);


            SoundManager.instance.MusicVolumeChanged(0);
        }
        else
        {
            EnableDisableMusicBtns(musicBtn, 1f);

            SoundManager.instance.MusicVolumeChanged(SoundManager.instance.OriginalbackGroundAudioVolume);
        }
    }
    public void OnSoundBTNClickded()
    {
        if (soundBtn.GetComponent<Image>().color.a == 1)
        {
            EnableDisableMusicBtns(soundBtn, 0.5f);


            SoundManager.instance.SoundVolumeChanged(0);
        }
        else
        {
            EnableDisableMusicBtns(soundBtn, 1f);

            SoundManager.instance.SoundVolumeChanged(SoundManager.instance.OriginalbackGroundAudioVolume);
        }
    }

    public void SwitchCanvas(UIPanelType targetPanel)
    {

        foreach (UIPanel panel in uiPanels)
        {

            if (panel.uiPanelType == targetPanel)
            {
                
                activeUIPanel = panel;
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }
        
        activeUIPanel.gameObject.SetActive(true);
    }

    public void OnMusicVolumeChanged()
    {
        // soundManager.instance.MusicVolumeChanged(musicSlider.value);
        // soundManager.instance.SaveMusicVoulme(musicSlider.value);
    }

    public void OnSoundVolumeChanged()
    {
        // soundManager.instance.SoundVolumeChanged(soundSlider.value);
        // soundManager.instance.SaveSoundVoulme(soundSlider.value);
    }

    public void ShowCombo(Vector3 pos, string combotext)
    {
        StopCoroutine("AutoDisableCombo");
        if (comboSpwaned == null)
            comboSpwaned = Instantiate(ComboObjPrefab, effectCanvas.transform);
        EnableCombo();
        comboSpwaned.transform.position = pos;
        comboSpwaned.GetComponent<TMP_Text>().text = combotext.ToString();
        StartCoroutine("AutoDisableCombo");
    }

    public void DisableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(false);
    }
    public void EnableCombo()
    {
        if (comboSpwaned != null)
            comboSpwaned.SetActive(true);
    }

    IEnumerator AutoDisableCombo()
    {
        yield return new WaitForSeconds(1);
        DisableCombo();
        // Code to execute after the delay
    }
}
