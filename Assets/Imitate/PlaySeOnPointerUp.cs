using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaySeOnPointerUp : MonoBehaviour, IPointerUpHandler
{
    public string se;

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.PlaySe(se);
    }
}
