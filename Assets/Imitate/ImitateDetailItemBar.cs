using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class ImitateDetailItemBar : MonoBehaviour
{
    public string Title
    {
        get
        {
            return Text_name.text;
        }
        set
        {
            Text_name.text = value;
        }
    }

    public float Percent
    {
        get
        {
            return Scrollbar_progress.Process;
        }
        set
        {
            Scrollbar_progress.Process = value;
            var percentInt = (int)(value * 100);
            Text_percent.text = $"{percentInt}%";
        }
    }

    public void PlayPercentAnimation(float percent)
    {
        Scrollbar_progress.PlayProcessAnimation(percent);
        Text_percent.text = "0%";
        var value = 0f;
        DOTween.To(()=>{
            return value;
        },(v)=>{
            value = v;
            var percentInt = (int)(value * 100);
            Text_percent.text = $"{percentInt}%";
        }, percent, 1f);
    }
}
