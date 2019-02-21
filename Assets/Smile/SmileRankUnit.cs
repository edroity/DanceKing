using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SmileRankUnit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetInfo(Sprite _img, string _name, int _num)
    {
        m_head_img.sprite = _img;
        m_name_label.text = _name;
        m_num_label.text = _num.ToString();
    }
    public Image m_head_img;
    public Text m_name_label;
    public Text m_num_label;
}
