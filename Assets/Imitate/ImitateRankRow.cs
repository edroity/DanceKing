using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ImitateRankRow : MonoBehaviour
{
    public int Rank
    {
        set
        {
            if(value <= 3)
            {
                // show sort image
                var spriteName = "";
                if(value == 1)
                {
                    spriteName = "1st";
                }
                else if(value == 2)
                {
                    spriteName = "2nd";
                }
                else if(value == 3)
                {
                    spriteName = "3rd";
                }
                var sprite = ImitateUtilResManager.Load<Sprite>(spriteName);
                Image_huangguan.sprite = sprite;
                // hide sort text
                Image_huangguan.gameObject.SetActive(true);
                Text_sort.gameObject.SetActive(false);
            }
            else
            {
                var text = $"{value}th";
                Text_sort.text = text;
                Text_sort.gameObject.SetActive(true);
                Image_huangguan.gameObject.SetActive(false);
            }

            // set frame
            {
                var spriteName = "";
                if(value == 1)
                {
                    spriteName = "t1";
                }
                else
                {
                    spriteName = "t2";
                }
                var sprite = ImitateUtilResManager.Load<Sprite>(spriteName);
                Image_frame.sprite = sprite;
            }


        }
    }

    public Sprite Icon
    {
        set
        {
            Image_icon.sprite = value;
        }
    }

    public string PlayerName
    {
        set
        {
            Text_name.text = value;
        }
    }

    public string Desc
    {
        set
        {
            Text_dec.text = value;
        }
    }

    public int Score
    {
        set
        {
            Text_score.text = $"{value}分";
        }
    }
}
