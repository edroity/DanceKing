using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImitateTranslateService
{
    public static string GetText(string id)
    {
        return StaticData.GetCell<string>("translate", id, "value");
    }

    private static IList<string> _keyList;
    public static IList<string> KeyList
    {
        get
        {
            if(_keyList == null)
            {
                var sheet = StaticData.GetSheet("translate");
                _keyList = ((IDictionary)sheet).Keys as IList<string>; 
            }
            return _keyList;
        }
    }

    public static string GetId(int index)
    {
        var id = KeyList[index];
        return id;
    }

    public static string GetTextByIndex(int index)
    {
        var id = GetId(index);
        var text = GetText(id);
        return text;
    }
}
