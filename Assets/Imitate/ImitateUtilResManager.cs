using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImitateUtilResManager 
{
    public static T Load<T>(string name) where T : UnityEngine.Object
    {
        var res = Resources.Load<T>($"Util/{name}");
        return res;
    }
}
