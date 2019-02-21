using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ImitateReadyPage : Page
{
    private float COUNTDOWN_START = 5;
    private float countdown;
    
    public override void OnNavigatedTo()
    {
        countdown = COUNTDOWN_START;
        var texture = ImitateLevelManager.GetSprite();
        RawImage_origin.texture = texture;
    }


    void Update()
    {
        if(countdown != -1)
        {
            this.countdown -= Time.deltaTime;
            // write countdown to label
            this.Text_countdown.text = ((int)this.countdown).ToString();
            // write to process bar
            this.Scrollbar.Process = countdown/COUNTDOWN_START;
            //this.Scrollbar
            if(this.countdown <= 0)
            {
                StartShot();
                countdown = -1;
            }
        }

    }

    public string Title
    {
        set
        {
            Label_title.text = value;
        }
    }

    public void OnButton(string msg)
    {
        if(msg == "start")
        {
            this.StartShot();
        }
        else if(msg == "back")
        {
            UIEngine.BackTo<ImitateMainPage>();
        }
    }

    private void StartShot()
    {
        var origin = RawImage_origin.texture as Texture2D;
        ImitateManager.origin = origin;
        UIEngine.Forward<ImitateShotPage>();
    }
}
