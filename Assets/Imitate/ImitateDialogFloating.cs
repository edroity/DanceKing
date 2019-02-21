using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public partial class ImitateDialogFloating : Floating
{
    public void PlayFadeOut(string text)
    {
        Test_content.text = text;
        var animation = this.GetComponent<Animation>();
        animation.Play();
    }

    public void OnAnimationPlayComplete()
    {
        UIEngine.HideFlaoting<ImitateDialogFloating>();
    }
}
