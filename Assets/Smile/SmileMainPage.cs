using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SmileMainPage : SimplePage
{
    public Text ScoreText;


    private void Start()
    {
        SmileProject.Instance.PlayBgm("smile_bgm");
        Refresh ();
    }

    public override void Refresh () {
        StatusBarManager.Show (true);
        StatusBarManager.BarStyle (3);
        ScoreText.text = SmileProject.Instance.GetSmilePoints ().ToString ();
        m_flower_ani.Play("FlowerAni");

        //SmileProject.Instance.FlowerGo.SetActive (true);
        //if (!SmileProject.Instance.FlowerGo.GetComponent<Animation> ().isPlaying)
        //SmileProject.Instance.FlowerGo.GetComponent<Animation> ().CrossFade ("FlowerDance");
    }
    public override void Leave () {
       // SmileProject.Instance.FlowerGo.SetActive (false);
    }

    public void OnTakePhoto () {
        SmileProject.Instance.PlaySe("smile_click");
        SimplePageManager.Instance.Show ("SmileTakePhotoPage");
    }
    public void OnRank () {
        SmileProject.Instance.PlaySe("smile_click");
        SimplePageManager.Instance.Show ("SmileRankPage");
    }
    public void OnMap()
    {
        SmileProject.Instance.PlaySe("smile_click");
        SimplePageManager.Instance.Show ("SmileMapPage");
    }
    public Animation m_flower_ani;
}
