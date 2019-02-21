using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImitateDeco : MonoBehaviour
{
    public float changeImageIn;

    void Start()
    {
        this.RandomTimer();
        this.ChangeImage();
    }

    void RandomTimer()
    {
        changeImageIn = Random.Range(3f, 10f);
    }

    void Update()
    {
        changeImageIn -= Time.deltaTime;
        if(changeImageIn <= 0)
        {
            this.RandomTimer();
            this.ChangeImage();
        }
    }

    void ChangeImage()
    {
        var sprite = ImitateDecoImageManager.RandomSprite();
        var image = this.GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }
}
