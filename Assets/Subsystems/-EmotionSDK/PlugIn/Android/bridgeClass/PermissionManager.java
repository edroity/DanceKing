package bridgeClass;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.pm.PackageManager;
import android.os.Build;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;


import static android.R.attr.targetSdkVersion;

/**
 * Created by edrotiy_mac1 on 2017/9/11.
 */

public class PermissionManager {

    static Activity gameActivity;

    public static void onCreate(Activity gameActivity)
    {
        PermissionManager.gameActivity = gameActivity;
    }

    static HashMap<Integer, Runnable> callbackDic = new HashMap<>();
    static int code = 0;

    static boolean LAST_SUCCESS = true;

    static void tryPermision(String[] permisionList, Runnable callback)
    {
        //String permision = Manifest.permission.WRITE_EXTERNAL_STORAGE;

        List<String> notGarenteed = new ArrayList<>();

        for (String permmsion: permisionList)
        {
            int ret = selfPermission(permmsion);
            if(ret != PackageManager.PERMISSION_GRANTED)
            {
                notGarenteed.add(permmsion);
            }
        }

        // list to array
        String[] list = new String[notGarenteed.size()];
        for(int i = 0 ; i < notGarenteed.size() ; i ++)
        {
            list[i] = notGarenteed.get(i);
        }

        // request
        if(list.length > 0)
        {
            int code = PermissionManager.code;
            PermissionManager.code++;
            callbackDic.put(code, callback);

            gameActivity.requestPermissions(list, code);
        }
        else
        {
            if(callback != null)
            {
                callback.run();
            }
        }
    }

    static boolean checkPermission(String[] permisionList)
    {
        for (String permmsion: permisionList)
        {
            int ret = selfPermission(permmsion);
            if(ret != PackageManager.PERMISSION_GRANTED)
            {
                return false;
            }
        }
        return true;
    }

    public static void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        boolean success = true;
        for (int i : grantResults)
        {
            if(i != PackageManager.PERMISSION_GRANTED)
            {
                success = false;
            }
        }

        LAST_SUCCESS = success;
        Runnable r = callbackDic.get(requestCode);
        if(r != null)
        {
            r.run();
        }
    }

    // force user allow permission, other wise finish activity.
    public static void surePermission(final int coutner, final String[] list, final Runnable complete)
    {
        if(checkPermission(list))
        {
            if(complete != null)
            {
                complete.run();
            }

            return;
        }

        PermissionManager.tryPermision(list, new Runnable() {
            @Override
            public void run() {
                if(PermissionManager.LAST_SUCCESS)
                {
                    if(complete != null)
                    {
                        complete.run();
                    }
                }
                else
                {
                    if(coutner < 3)
                    {
                        surePermission(coutner + 1, list, complete);
                    }
                    else
                    {
                        gameActivity.finish();
                    }
                }

            }
        });

//        final AlertDialog.Builder normalDialog = new AlertDialog.Builder(gameActivity);
//        normalDialog.setTitle("");
//        normalDialog.setMessage(text);
//        normalDialog.setCancelable(false);
//        normalDialog.setPositiveButton("OK",
//                new DialogInterface.OnClickListener() {
//                    @Override
//                    public void onClick(DialogInterface dialog, int which) {
//
//
//
//                    }
//                });
//        normalDialog.show();
    }

    private static int selfPermission(String permission) {
        // For Android < Android M, self permissions are always granted.
        int result = -1;

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {

            if (targetSdkVersion >= Build.VERSION_CODES.M) {
                // targetSdkVersion >= Android M, we can
                // use Context#checkSelfPermission
                result = gameActivity.checkSelfPermission(permission);
                return result;
            } else {

                return 0;
                // targetSdkVersion < Android M, we have to use PermissionChecker
                //result = PermissionChecker.checkSelfPermission(gameActivity, permission);
            }
        }
        else
        {
            return 0;
        }


    }
}
