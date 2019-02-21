package bridgeClass;

import android.app.Activity;
import android.util.Log;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;

/**
 * Created by zhiyuan.peng on 2017/5/24.
 */

public class NativeGameManifestManager {

    static Activity gameActivity;

    // c# read this
    static String manifestString = null;

    static JSONObject manifest = null;

    public static void onGameActivitCreate(Activity activity)
    {
        gameActivity = activity;
        load();
        printLog();
    }

    // call
    public static String GetManifest(String arg)
    {
        return manifestString;
    }

    public static void load()
    {
        try
        {
            InputStream stream = gameActivity.getAssets().open("game-manifest.json");
            String jsonString = readToEnd(stream);
            manifestString = jsonString;
            manifest = new JSONObject(jsonString);
        }
        catch (Exception e)
        {
            e.printStackTrace();
            throw new RuntimeException(e);
        }
    }

    public static String tryGet(String key, String defaultValue)
    {
        if(manifest == null)
        {
            throw new RuntimeException("game-manifest not intied while reading");
        }
        boolean has = manifest.has(key);
        if(!has) return defaultValue;
        return JSONObjectUtil.getString(manifest, key);
    }

    public static void printLog()
    {
        Log.d("A-GameManifestManager", "game-manifest: " + manifestString);
    }

    /**
     * 按行读取txt
     *
     * @param is
     * @return
     * @throws Exception
     */
    private static String readToEnd(InputStream is) throws Exception {
        InputStreamReader reader = new InputStreamReader(is);
        BufferedReader bufferedReader = new BufferedReader(reader);
        StringBuffer buffer = new StringBuffer("");
        String str;
        while ((str = bufferedReader.readLine()) != null) {
            buffer.append(str);
            buffer.append("\n");
        }
        return buffer.toString();
    }

}
