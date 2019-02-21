using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

public static class ImitateManager
{
    public static Texture2D origin;
    public static Texture2D input;

    public static async Task<ImitateResult> RateAsync()
    {
        EmotionSDK.SetDirectionCheck(false);
        EmotionSDK.SetEmotionCheck(true);
        var originBase64 = ImitateUtil.TextureToBase64(origin);
        var inputBase64 = ImitateUtil.TextureToBase64(input);
        // 检查原图片
        List<float> emotionOrigin;
        {
            List<Face> result = null;
            // var t = new Thread(async()=>{
            //     result = await EmotionSDK.CheckAsync(originBase64);
            // });
            // t.Start();
            // while(result == null)
            // {
            //     await Task.Delay(1);
            // }
            result = await EmotionSDK.CheckAsync(originBase64);
            if(result.Count == 0)
            {
                throw new Exception("face not found in origin image");
            }
            emotionOrigin = result[0].emotion;
        }
        // 检查输入图片
        List<float> emotionInput;
        {
            List<Face> result = null;
            // var t = new Thread(async()=>{
            //     result = await EmotionSDK.CheckAsync(inputBase64);
            // });
            // t.Start();
            // while(result == null)
            // {
            //     await Task.Delay(1);
            // }
            result = await EmotionSDK.CheckAsync(inputBase64);
            if(result.Count == 0)
            {
                throw new Exception("face not found in input image");
            }
            emotionInput = result[0].emotion;
        }
        // 选取前5项数值
        var indexList = FindBigestIndex(emotionOrigin, 2);
        var pickedListA = PickByIndex(emotionOrigin, indexList);
        var pickedListB = PickByIndex(emotionInput, indexList);

        // log
        LogList(pickedListA);
        LogList(pickedListB);

        // make score
        var score = CalcuScoreV3(pickedListA, pickedListB);

        // return
        var ret = new ImitateResult();
        ret.score = score;
        ret.itemList = new List<ImitateItem>();
        for(int i = 0; i < indexList.Count; i++)
        {
            var index = indexList[i];
            var target = pickedListA[i];
            var input = pickedListB[i];
            var item = new ImitateItem();
            item.index = index;
            item.target = target;
            item.input = input;
            ret.itemList.Add(item);
        }

        return ret;
    }

    public static int CalcuScoreV3(List<float> listA, List<float> listB)
    {
        // 单独评价每一项
        var likeList = new List<float>();
        for(int i = 0; i < listA.Count; i++)
        {
            var a = listA[i];
            var b = listB[i];
            var likely = RateSignleItem01(a, b);
            likeList.Add(likely);
        }
        // 综合评分
        var aveLike = Ave(likeList);

        // 正选映射
        double score01 = aveLike;
        var fixTime = int.Parse(GameManifestManager.Get("sin-fix-time"));
        for(int i = 0; i < fixTime; i++)
        {
            score01 = SinFix(score01);
        }
 
        // 映射到 0 ～ 100
        var score100 = score01 * 100;
        return (int)score100;
    }

    public static double SinFix(double input)
    {
        var d = input * Math.PI/2;
        var o = Math.Sin(d);
        return o;
    }

    public static float RateSignleItem01(float a, float b)
    {
        var min = Math.Min(a, b);
        var max = Math.Max(a, b);
        var likely = min / max;
        return likely;
    }

    public static void LogList(List<float> list)
    {
        Debug.Log("start print list");
        foreach(var i in list)
        {
            Debug.Log(i);
        }
    }

    public static List<float> PickByIndex(List<float> valueList, List<int> indexList)
    {
        var ret = new List<float>();
        foreach(var i in indexList)
        {
            var value = valueList[i];
            ret.Add(value);
        }
        return ret;
    }

    public static List<int> FindBigestIndex(List<float> list, int count)
    {
        var elementList = new List<ValueAndIndex>();
        for(int i = 0; i < list.Count; i++)
        {
            var e = new ValueAndIndex();
            e.index = i;
            e.value = list[i];
            elementList.Add(e);
        }
        elementList.Sort((a, b)=>{
            if(a.value > b.value)
            {
                // a 在前面
                return -1;
            }
            else if(a.value < b.value)
            {
                // a 在后面
                return 1;
            }
            else
            {
                return 0;
            }
        });
        // 取前5个的index
        var indexList = new List<int>();
        for(int i = 0; i < count; i++)
        {
            var index = elementList[i].index;
            indexList.Add(index);
        }
        return indexList;
    }

    public struct ValueAndIndex
    {
        public float value;
        public int index;
    }

    public static int CalcuScore(List<float> listA, List<float> listB)
    {
        listA = Bilihua(listA);
        listB = Bilihua(listB);

        var a_ave = Ave(listA);
        var b_ave = Ave(listB);

        var a_cha_list = AveChaList(listA, a_ave);
        var b_cha_list = AveChaList(listB, b_ave);

        var ji_list = JiList(a_cha_list, b_cha_list);

        var fenzi = Sum(ji_list);

        var a_fangcha_list = PingfangList(a_cha_list);
        var b_fangcha_list = PingfangList(b_cha_list);

        var a_fangcha_he = Sum(a_fangcha_list);
        var b_fangcha_he = Sum(b_fangcha_list);

        var a_fangcha_he_kaifang = Math.Sqrt(a_fangcha_he);
        var b_fangcha_he_kaifang = Math.Sqrt(b_fangcha_he);

        var fenmu = a_fangcha_he_kaifang * b_fangcha_he_kaifang;

        var valueFu1to1 = fenzi / fenmu;
        Debug.Log($"fenzi: {fenzi}, fenmu: {fenmu}, value: {valueFu1to1}");

        // 映射到 0 ～ 100
        var input = valueFu1to1;
        // -100 ~ 100
        input = input * 100;
        // // -80 ~ 120
        // input += 20;
        // // -320 ~ 480
        // input *=4;
        // // clamp to 0 ~ 100
        if(input <= 0)
        {
            input = 0;
        }
        else if(input >= 100)
        {
            input = 100;
        }
        return (int)input;
    }

    public static List<float> Bilihua(List<float> list)
    {
        var ret = new List<float>();
        var sum = Sum(list);
        foreach(var e in list)
        {
            var bili = e / sum;
            ret.Add(bili);
        }
        return ret;
    }

    public static List<float> PingfangList(List<float> list)
    {
        var ret = new List<float>();
        foreach(var e in list)
        {
            var pingfang = e * e;
            ret.Add(pingfang);
        }
        return ret;
    }

    public static float Sum(List<float> list)
    {
        float sum = 0;
        foreach(var e in list)
        {
            sum += e;
        }
        return sum;
    }

    public static List<float> JiList(List<float> listA, List<float> listB)
    {
        var ret = new List<float>();
        for(int i = 0; i < listA.Count; i++)
        {
            var a = listA[i];
            var b = listB[i];
            var ji = a * b;
            ret.Add(ji);
        }
        return ret;
    }

    public static List<float> AveChaList(List<float> list, float ave)
    {
        var ret = new List<float>();
        for(int i = 0; i < list.Count; i++)
        {
            var e = list[i];
            var cha = e - ave;
            ret.Add(cha);
        }
        return ret;
    }

    public static float Ave(List<float> list)
    {
        float ave;
        float sum = 0;
        foreach(var e in list)
        {
            sum += e;
        }
        ave = sum / list.Count;
        return ave;
    }

    // 输出 0 ~ 1
    public static float RateForSignleEmotion(float target, float input)
    {
        var max = Math.Max(target, input);
        var min = Math.Min(target, input);
        var rate = min / max;
        return rate;
    }

    public static EmotionIndexAndValue FindObviousEmotionIndexAndValue(List<float> emotionList)
    {
        var maxIndex = 0;
        var max = 0f;
        for(int i = 0; i < emotionList.Count; i++)
        {
            var value = emotionList[i];
            if(value > max)
            {
                max = value;
                maxIndex = i;
            }
        }
        var ret = new EmotionIndexAndValue()
        {
            index = maxIndex,
            value = max
        };
        return ret;
    }
}

public struct EmotionIndexAndValue
{
    public int index;
    public float value;
}