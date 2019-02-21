//ybzuo-edroity
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HotDogMan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //m_speed = (m_max_mouth_deg - m_min_mouth_deg) * 3;
    }

    // Update is called once per frame
    void Update()
    {

        if(m_current_offset<m_target_offset)
        {
            m_current_offset += Time.deltaTime*m_speed;
            if(m_current_offset>m_target_offset)
            {
                m_current_offset = m_target_offset;
            }
        }
        else if(m_current_offset>m_target_offset)
        {
            m_current_offset -= Time.deltaTime*m_speed;
            if (m_current_offset < m_target_offset)
            {
                m_current_offset = m_target_offset;
            }
        }
        m_head.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(m_min_mouth_deg, m_max_mouth_deg, m_current_offset));
    }

    float DataFilter(float _raw_data)
    {
        //实时去噪
        //保留大趋
        float _out = _raw_data;
        if(m_dir_up)
        {
            if(_raw_data<m_target_offset)
            {
                if(m_target_offset-_raw_data<0.1f)
                {
                    //噪音,保持原趋势继续
                    _out=m_target_offset + 0.01f;
                }
            }
        }
        else
        {
            if (_raw_data > m_target_offset)
            {
                if (_raw_data-m_target_offset< 0.1f)
                {
                    //噪音,保持原趋势继续
                    _out = m_target_offset - 0.01f;
                }
            }
        }

        if(_out>1.0f)
        {
            _out = 1.0f;
        }
        else if(_out<0.0f)
        {
            _out = 0.0f;
        }
        return _out;
    }
    public bool UpdateMouth(float _offset)
    {
        //去噪
        _offset = DataFilter(_offset);
        bool _eat = false;
        if(_offset>m_target_offset)
        {
            if (!m_dir_up)
            {
                m_dir_up = true;
            }
        }
        else
        {
            if(m_dir_up)
            {
                m_dir_up = false;
            }
        }
        if (m_ready_eat)
        {
            if(m_target_offset<m_check_bottom)
            {
                m_ready_eat = false;
                _eat = true;
               // m_eat_ani.Play();
            }
        }
        else
        {
            if (m_target_offset > m_check_top)
            {
                m_ready_eat = true;
            }
        }
        m_target_offset = _offset;//Mathf.Clamp(_offset, 0.0f, 1.0f);
        return _eat;
    }
    public void PlayEatAni()
    {
        //m_eat_ani.Play();
    }
    public Image m_head;
    public Image m_hot_dog;
    public float m_max_mouth_deg;
    public float m_min_mouth_deg;
    float m_target_offset = 0.0f;
    float m_current_offset = 0.0f;
    public float m_speed = 20.0f;
    bool m_dir_up = true;
    public Animation m_eat_ani;
    bool m_ready_eat = false;
    public float m_check_top = 0.6f;
    public float m_check_bottom = 0.3f;
}
