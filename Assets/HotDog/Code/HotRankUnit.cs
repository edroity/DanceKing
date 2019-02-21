using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HotRankUnit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetInfo(int _order,Sprite _img,string _name,int _num)
    {
        m_head_img.sprite = _img;
        switch(_order)
        {
            case 1:
                m_order_label.text = "1st";
                break;
            case 2:
                m_order_label.text = "2nd";
                break;
            case 3:
                m_order_label.text = "3th";
                break;
            default:
                m_order_label.text = _order.ToString();
                break;
        }
        m_name_label.text = _name;
        m_num_label.text = _num.ToString();
    }
    public Image m_head_img;
    public Text m_order_label;
    public Text m_name_label;
    public Text m_num_label;
}
