using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
 
public class MyPluginPostProcessBuild
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        #if UNITY_IOS
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
           
            // Get root
            PlistElementDict rootDict = plist.root;
           
            // add plist key
            rootDict.SetString("NSPhotoLibraryAddUsageDescription", "即将保存到相册");
           
            // close bit code
            {
                var path = $"{pathToBuiltProject}/Unity-iPhone.xcodeproj/project.pbxproj";
                var text = File.ReadAllText(path);
                text = text.Replace("ENABLE_BITCODE = YES", "ENABLE_BITCODE = NO");
                File.WriteAllText(path, text);
            }

            // // background modes
            // PlistElementArray bgModes = rootDict.CreateArray("UIBackgroundModes");
            // bgModes.AddString("location");
            // bgModes.AddString("fetch");
           
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
        #endif

        #if UNITY_ANDROID
        if(buildTarget == BuildTarget.Android)
        {
            // build.gradle
            {
                var name = "build.gradle";
                var serchDir = pathToBuiltProject;
                var path = FindFile(serchDir, name);
                if(path != "")
                {
                    var text = File.ReadAllText(path);
                    var start = text.IndexOf("implementation fileTree(dir: 'libs', include: ['*.jar'])");
                    var end = text.IndexOf("}", start);
                    var insertPoint = end - 1;
                    var before = text.Substring(0, insertPoint);
                    var after = text.Substring(insertPoint);
                    var content = @"    
        implementation 'com.fasterxml.jackson.core:jackson-core:+'
        implementation 'com.fasterxml.jackson.core:jackson-databind:+'
        api 'io.reactivex.rxjava2:rxjava:2.2.0'
        api 'io.reactivex.rxjava2:rxandroid:2.1.0'
        ";
                    
                    text = before + content + after;
                    File.WriteAllText(path, text);
                }

            }

            // AndroidManifest.gradle
            {
                var name = "AndroidManifest.xml";
                var serchDir = pathToBuiltProject;
                var path = FindFile(serchDir, name);

                if(path != "")
                {
                    var text = File.ReadAllText(path);
                    var start = text.IndexOf("<application");
                    var insertPoint = start - 1;
                    var before = text.Substring(0, insertPoint);
                    var after = text.Substring(insertPoint);
                    var content = @"    
    <uses-permission android:name='android.permission.INTERNET'/>
    <uses-permission android:name='android.permission.CAMERA'/>
    <uses-permission android:name='android.permission.WRITE_EXTERNAL_STORAGE'/>
    <uses-permission android:name='android.permission.READ_EXTERNAL_STORAGE'/>
    ";
                    
                    text = before + content + after;
                    File.WriteAllText(path, text);
                }

            }

            // UnityPlayerActivity.gradle
            {
                var name = "UnityPlayerActivity.java";
                var serchDir = pathToBuiltProject;
                var path = FindFile(serchDir, name);
                if(path != "")
                {
                    var text = File.ReadAllText(path);
                    var start = text.IndexOf("onCreate");
                    var end = text.IndexOf("}", start);
                    var insertPoint = end - 1;
                    var before = text.Substring(0, insertPoint);
                    var after = text.Substring(insertPoint);
                    var content = @"    
        bridgeClass.PermissionManager.onCreate(this);
        String[] permissions ={
                android.Manifest.permission.CAMERA,
                android.Manifest.permission.READ_EXTERNAL_STORAGE,
                android.Manifest.permission.WRITE_EXTERNAL_STORAGE,
        };
        bridgeClass.PermissionManager.surePermission(2, permissions, null);
        bridgeClass.NativePhoto.gameActivity = this;
       ";
       
                    
                    text = before + content + after;
                    File.WriteAllText(path, text);
                }

            }
             // UnityPlayerActivity.gradle
            {
                var name = "UnityPlayerActivity.java";
                var serchDir = pathToBuiltProject;
                var path = FindFile(serchDir, name);
                if(path != "")
                {
                    var text = File.ReadAllText(path);
                    var start = text.IndexOf("@Override protected void onNewIntent(Intent intent)");
                    var insertPoint = start - 1;
                    var before = text.Substring(0, insertPoint);
                    var after = text.Substring(insertPoint);
                    var content = @"    
        @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        bridgeClass.PermissionManager.onRequestPermissionsResult(requestCode, permissions, grantResults);
    }

        @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        bridgeClass.NativePhoto.onActivityResult(requestCode, resultCode, data);
    }
       ";
       
                    
                    text = before + content + after;
                    File.WriteAllText(path, text);
                }

            }
        }
        #endif
    }

    private static string FindFile(string dir, string filename)
    {
        var list = Directory.GetFiles(dir, filename, SearchOption.AllDirectories);
        if(list.Length != 0)
        {
            return list[0];
        }
        return "";
    }
}