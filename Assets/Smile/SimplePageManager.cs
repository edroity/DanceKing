using UnityEngine;
using System.Collections.Generic;

public class SimplePageManager : MonoBehaviour
{
    public Transform PageRoot;
    Dictionary<string, SimplePage> mPageList = new Dictionary<string, SimplePage> ();
    static SimplePageManager mInstance;
    public static SimplePageManager Instance {
        get {
            if (mInstance == null) {
                mInstance = GameObject.Find ("Main Camera").GetComponent<SimplePageManager> ();
            }
            return mInstance;
        }
    }

    void Start()
    {
        mPageList.Clear();
        for (int i=0;i<PageList.Length;++i)
        {
            GameObject go = GameObject.Instantiate(PageList[i]);
            go.name= PageList[i].name;
            go.transform.parent = PageRoot.transform;
            go.transform.localScale = Vector3.one;
            //go.transform.localPosition = Vector3.zero;

            var rt = go.GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            SimplePage _sp = go.GetComponent<SimplePage>();
            go.SetActive(false);
            mPageList.Add(_sp.name, _sp);
        }
        Show("SmileMainPage");
    }

    public SimplePage Show (string pageName) {
        SimplePage result = null;
        foreach (var kv in mPageList) {
            var show = kv.Key == pageName;
            kv.Value.gameObject.SetActive (show);
            if (show)
                result = kv.Value;
            else
                kv.Value.Leave ();
        }

        if (result)
            result.Refresh ();
        return result;
    }
    public GameObject[] PageList;
}
