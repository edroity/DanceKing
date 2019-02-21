using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImitateLevelManager 
{
    public static int levelIndex;

    public static Texture GetSprite()
    {
        var index = levelIndex % 3 + 1;
        var name = $"pic{index}";
        var sprite = ImitateUtilResManager.Load<Texture>(name);
        return sprite;
    }
}
