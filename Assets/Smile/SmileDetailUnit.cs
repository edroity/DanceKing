using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SmileDetailUnit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetInfo(string _name,int _score)
    {
        name_label.text = _name;
        if(_score<1)
        {
            _score = 1;
        }
        score_label.text = _score + "%";
    }
    public Text name_label;
    public Text score_label;

}
