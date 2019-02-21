using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class Scrollbar : MonoBehaviour
{
    public float Process
    {
        set
        {
            this.Image_fg.fillAmount = value;
        }
        get
        {
            return this.Image_fg.fillAmount;
        }
    }

    public void PlayProcessAnimation(float process)
    {
        this.Process = 0;
        this.Image_fg.DOFillAmount(process, 1f);
    }
}
