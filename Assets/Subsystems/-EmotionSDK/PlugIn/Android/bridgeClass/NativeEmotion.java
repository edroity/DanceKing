package bridgeClass;


import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Point;
import android.nfc.Tag;
import android.util.Base64;
import android.util.Log;
import android.widget.Toast;

import com.edroity.nativebrige.Gate;
import com.fasterxml.jackson.core.JsonGenerationException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationConfig;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.pingan.yzt.monalisasdk.bean.Emotion;
import com.pingan.yzt.monalisasdk.bean.FaceInfo;
import com.pingan.yzt.monalisasdk.bean.HeadPose;
import com.pingan.yzt.monalisasdk.bean.Result;
import com.pingan.yzt.monalisasdk.bean.TypeDef;
import com.pingan.yzt.monalisasdk.data.MoNaService;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.List;

/**
 * Created by zhiyuan.peng on 2018/1/19.
 */

public class NativeEmotion
{
    private static String TAG = "NativeEmotion";

    static void init()
    {
        int result = MoNaService.getInstance().initModel(TypeDef.TYPE_FACE);
        if(result != Result.ERROR_SUCCESS){
            //Toast.makeText(this,"有错误",Toast.LENGTH_SHORT).show();
            Log.e(TAG,"MoNaService init TYPE_FACE error, result: " + result );
        }
        int result2 = MoNaService.getInstance().initModel(TypeDef.TYPE_EMOTION);
        if(result2 != Result.ERROR_SUCCESS){
            //Toast.makeText(this,"有错误",Toast.LENGTH_SHORT).show();
            Log.e(TAG,"MoNaService init TYPE_EMOTION error, result: " + result2 );
        }
        int result3 = MoNaService.getInstance().initModel(TypeDef.TYPE_HEADPOSE);
        if(result3 != Result.ERROR_SUCCESS){
            //Toast.makeText(this,"有错误",Toast.LENGTH_SHORT).show();
            Log.e(TAG,"MoNaService init TYPE_HEADPOSE error, result: " + result3 );
        }
    }

    /**
     * bitmap转为base64
     * @param bitmap
     * @return
     */
    public static String bitmapToBase64(Bitmap bitmap)
    {

        String result = null;
        ByteArrayOutputStream baos = null;
        try {
            if (bitmap != null) {
                baos = new ByteArrayOutputStream();
                bitmap.compress(Bitmap.CompressFormat.JPEG, 100, baos);

                baos.flush();
                baos.close();

                byte[] bitmapBytes = baos.toByteArray();
                result = Base64.encodeToString(bitmapBytes, Base64.DEFAULT);
            }
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            try {
                if (baos != null) {
                    baos.flush();
                    baos.close();
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return result;
    }


    /**
     * base64转为bitmap
     * @param base64Data
     * @return
     */
    public static Bitmap base64ToBitmap(String base64Data)
    {
        byte[] bytes = Base64.decode(base64Data, Base64.DEFAULT);
        return BitmapFactory.decodeByteArray(bytes, 0, bytes.length);
    }

    public static void SetDirectionCheck(final String callId, final String arg)
    {
        Gate.callReturn(callId, "");
    }

    public static void SetEmotionCheck(final String callId, final String arg)
    {
        Gate.callReturn(callId, "");
    }


    static boolean first = true;

    public static void Check(final String callId, final String arg)
    {
        if(first)
        {
            init();
            first = false;
        }
        new Thread(new Runnable() {
            @Override
            public void run() {
                Log.e(TAG, "thread: before sleep");
//                try
//                {
//                    Log.e(TAG, "thread: before sleep");
//                    Thread.sleep(10000);
//                    Log.e(TAG, "thread: after sleep");
//                }
//                catch (Exception e)
//                {
//
//                }
                 /*
                String sign = HardInfo.getSign(arg, GameActivity.instance);
                Gate.callReturn(callId, sign);
                */
                Log.i(TAG, "[Java] NativeEmotion.Check: data: " + arg);

                // get base64 to bitmap
                Bitmap bitmap = base64ToBitmap(arg);
                Log.i(TAG, "bitmap: " + bitmap.getWidth() + "*" + bitmap.getHeight());

                Log.i(TAG, "11111");

                Result<List<FaceInfo>> faceInfoList = MoNaService.getInstance().detectFace(bitmap);

                Log.i(TAG, "22222");

                Result<List<Emotion>> rawResult = MoNaService.getInstance().detectEmotion(bitmap, faceInfoList.result);

                Log.i(TAG, "33333");

                if(rawResult.errorCode != Result.ERROR_SUCCESS)
                {
                    Log.i(TAG, "ret");
                    EmotionCheckResult ret = new EmotionCheckResult();
                    GateReturn(callId, ret);
                    return;
                }

                Log.i(TAG, "44444");

                EmotionCheckResult ret = RawFaceListToFormateType(rawResult.result);

                Log.i(TAG, "55555");

                GateReturn(callId, ret);
                Log.i(TAG, "ret");
                return;
            }
        }).start();

        Log.e(TAG, "pass");
    }

    public static void GateReturn(String callId, EmotionCheckResult result)
    {
        if(result.list.length == 0)
        {
            Gate.callReturn(callId, "{\"list\":[]}");
            return;
        }

        ObjectMapper mapper = new ObjectMapper();
        //mapper.disable(SerializationConfig.Feature.FAIL_ON_EMPTY_BEANS);
        mapper.disable(SerializationFeature.FAIL_ON_EMPTY_BEANS);
        try
        {
            //Convert object to JSON string
            String jsonInString = mapper.writeValueAsString(result.list[0]);
            String json = "{\"list\":[" + jsonInString + "]}";
            System.out.println(json);
            Gate.callReturn(callId, json);
        }
        catch (JsonGenerationException e)
        {
            e.printStackTrace();
        }
        catch (JsonMappingException e)
        {
            e.printStackTrace();
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
    }


    public static EmotionCheckResult RawFaceListToFormateType(List<Emotion> rawEmotionList)
    {
        EmotionCheckResult ret = new EmotionCheckResult();
        ret.list = new Face[rawEmotionList.size()];
        // per face
        //for (Emotion rawFace: rawList)
        for(int i = 0; i < rawEmotionList.size(); i++)
        {
            Emotion rawEmotion = rawEmotionList.get(i);
            FaceInfo rawFace = rawEmotion.faceInfo;
            Face face = new Face();
            face.emotion = rawEmotion.emotion;
            face.leftEyesPos = PointToCGPoint(rawFace.leftEye);
            face.rightEyesPos = PointToCGPoint(rawFace.rightEye);
            face.nosePos = PointToCGPoint(rawFace.nose);
            face.leftMouthCornerPos = PointToCGPoint(rawFace.leftMouth);
            face.rightMouthCornerPos = PointToCGPoint(rawFace.rightMouth);
            ret.list[i] = face;
            //ret.list.add(face);
        }
        return ret;
    }

    public static CGPoint PointToCGPoint(Point point)
    {
        CGPoint ret = new CGPoint();
        ret.x = point.x;
        ret.y = point.y;
        return ret;
    }



}
