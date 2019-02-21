package bridgeClass;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Base64;

import com.edroity.nativebrige.Gate;

import java.io.BufferedOutputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;

public class NativePhoto
{
    public static Activity gameActivity;
    /** 首先默认个文件保存路径 */
    private static final String SAVE_PIC_PATH=Environment.getExternalStorageState().equalsIgnoreCase(Environment.MEDIA_MOUNTED) ? Environment.getExternalStorageDirectory().getAbsolutePath() : "/mnt/sdcard";//保存到SD卡
    private static final String SAVE_REAL_PATH = SAVE_PIC_PATH + "/good/savePic";//保存的确切位置

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


    public static void Save(String callId, String arg) throws IOException
    {
        Bitmap bitmap = base64ToBitmap(arg);

        SimpleDateFormat sdf=new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        java.util.Date date=new java.util.Date();
        String datestring=sdf.format(date);

        saveFile(gameActivity, bitmap, datestring, "");
        Gate.callReturn(callId, "true");
    }

    public static void saveFile(Activity activity, Bitmap bm, String fileName, String path) throws IOException
    {
        String subForder = SAVE_REAL_PATH + path;
        File foder = new File(subForder);
        if (!foder.exists()) {
            foder.mkdirs();
        }
        File myCaptureFile = new File(subForder, fileName);
        if (!myCaptureFile.exists()) {
            myCaptureFile.createNewFile();
        }
        BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(myCaptureFile));
        bm.compress(Bitmap.CompressFormat.JPEG, 80, bos);
        bos.flush();
        bos.close();

        Intent intent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
        Uri uri = Uri.fromFile(myCaptureFile);
        intent.setData(uri);
        activity.sendBroadcast(intent);//这个广播的目的就是更新图库，发了这个广播进入相册就可以找到你保存的图片了！，记得要传你更新的file哦
    }

    public static String select_call_id;
    public static int IMAGE_REQUEST_CODE = 9999;
    public static void Select(String callId, String arg)
    {
        if(select_call_id == null)
        {
            select_call_id = callId;
            Intent intent = new Intent(
                    Intent.ACTION_PICK,
                    android.provider.MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            gameActivity.startActivityForResult(intent, IMAGE_REQUEST_CODE);
        }
    }

    public static void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        if(select_call_id != null)
        {
            Bitmap bitmap = GetBitmap(requestCode, resultCode, data);
            String base64 = bitmapToBase64(bitmap);
            Gate.callReturn(select_call_id, base64);
            select_call_id = null;
        }
    }


    private static Bitmap GetBitmap(int requestCode, int resultCode, Intent data)
    {
        //在相册里面选择好相片之后调回到现在的这个activity中
        switch (requestCode) {
            case 9999://这里的requestCode是我自己设置的，就是确定返回到那个Activity的标志
                if (resultCode == Activity.RESULT_OK) {//resultcode是setResult里面设置的code值
                    try {
                        Uri selectedImage = data.getData(); //获取系统返回的照片的Uri
                        String[] filePathColumn = {MediaStore.Images.Media.DATA};
                        Cursor cursor = gameActivity.getContentResolver().query(selectedImage,
                                filePathColumn, null, null, null);//从系统表中查询指定Uri对应的照片
                        cursor.moveToFirst();
                        int columnIndex = cursor.getColumnIndex(filePathColumn[0]);
                        String path = cursor.getString(columnIndex);  //获取照片路径
                        cursor.close();
                        Bitmap bitmap = BitmapFactory.decodeFile(path);
                        return bitmap;
                    } catch (Exception e) {
                        // TODO Auto-generatedcatch block
                        e.printStackTrace();
                    }
                }
                break;
        }
        return null;
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



}
