using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmileMapPage : SimplePage
{
    public Text ScoreText;
    public GameObject MapObj;
    public GameObject DetailsObj;
    public Image normal_m;
    public Image new_m;
    public void ToMap()
    {
        MapObj.SetActive (true);
        DetailsObj.SetActive (false);
    }
    public void ToDetails()
    {
        ScoreText.text = SmileProject.Instance.GetSmilePoints ().ToString ();
        MapObj.SetActive (false);
        DetailsObj.SetActive (true);
        normal_m.enabled = true;
        new_m.enabled = false;
    }
    public void ToDetailsNew()
    {
        ScoreText.text = SmileProject.Instance.GetSmilePoints().ToString();
        MapObj.SetActive(false);
        DetailsObj.SetActive(true);
        normal_m.enabled = false;
        new_m.enabled = true;
    }
    public void OnClose()
    {
        SmileProject.Instance.PlaySe("smile_click");
        SimplePageManager.Instance.Show ("SmileMainPage");
    }
    public override void Refresh()
    {
        StatusBarManager.BarStyle (3);
        StatusBarManager.Show (true);
        ToMap ();
    }
}
