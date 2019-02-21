using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImitateDecoImageManager
{
    private static Sprite[] spriteList;
    public static void Init()
    {
        spriteList = Resources.LoadAll<Sprite>("deco");
    }

    public static Sprite RandomSprite()
    {
        var index = Random.Range(0, spriteList.Length);
        var sprite = spriteList[index];
        return sprite;
    }
}
