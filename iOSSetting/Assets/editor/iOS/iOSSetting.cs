using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class iOSSetting : EditorWindow {

    public static string versionName = "0.0.1";
    private static string mConfigFileName = "iOSSetting.property";

    private iOSConfig mConfig;

    bool showCFBundleURLSchemes = true;
    bool showLSApplicationQueriesSchemes = true;
    bool showFrameworks = true;
    bool showLIBRARY_SEARCH_PATHS = true;
    bool showFRAMEWORK_SEARCH_PATHS = true;
    bool isAuto = true;
    bool isComplierAuto = true;
    string tempURLScheme = "";
    string tempLSApplicationQueriesScheme = "";
    string tempFrameworkName = "";
    string tempLIBRARY_SEARCH_PATH = "";
    string tempFRAMEWORK_SEARCH_PATH = "";

    string[] flags = new string[] { "YES", "NO" };
    int tempBitCodeIndex = 1;
    int tempGCCObjCFLag = 0;

    Texture texture;
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("iOS/Setting")]
    static void AddWIndow()
    {
        Rect rt = new Rect(200, 200, 800, 500);
        iOSSetting window = (iOSSetting)EditorWindow.GetWindowWithRect(typeof(iOSSetting), rt, true, "window");
        window.Show();
    }

    [MenuItem("iOS/UnInstall")]
    static void UnInstallWIndow()
    {
        Rect rt = new Rect(200, 200, 200, 100);
        iOSConfirm window = (iOSConfirm)EditorWindow.GetWindowWithRect(typeof(iOSConfirm), rt, true, "window");
        window.Show();
    }

    [MenuItem("iOS/About")]
    static void AboutWindow()
    {
        Rect rt = new Rect(200, 200, 300, 200);
        iOSAbout window = (iOSAbout)EditorWindow.GetWindowWithRect(typeof(iOSAbout), rt, true, "window");
        window.Show();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Space(20);
        GUILayout.Label(" 第三方Id");
        GUILayout.Space(5);
        mConfig.wxAppId = EditorGUILayout.TextField("微信AppId", mConfig.wxAppId);
        mConfig.wxAppSecret = EditorGUILayout.TextField("微信AppSecret", mConfig.wxAppSecret);

        mConfig.xianLiaoAppId = EditorGUILayout.TextField("闲聊AppId", mConfig.xianLiaoAppId);
        mConfig.yunvaAppId = EditorGUILayout.TextField("云娃AppId", mConfig.yunvaAppId);
        mConfig.baiduAppKey = EditorGUILayout.TextField("百度AppKey", mConfig.baiduAppKey);
        mConfig.buglyAppId = EditorGUILayout.TextField("腾讯buglyAppId", mConfig.buglyAppId);
        mConfig.url_schemes_name = EditorGUILayout.TextField("url_schemes_name", mConfig.url_schemes_name);

        GUILayout.BeginHorizontal();
        if (!string.IsNullOrEmpty(mConfig.iConPath) && texture == null)
        {
            texture = AssetDatabase.LoadAssetAtPath("Assets" +mConfig.iConPath, typeof(Texture)) as Texture;
        }
        texture = EditorGUILayout.ObjectField("提交市场Icon（1024X1024） ", texture, typeof(Texture), true, GUILayout.MaxWidth(300)) as Texture;
        if (texture != null)
        {
            string tempPath = AssetDatabase.GetAssetPath(texture);
            if(tempPath.IndexOf("/") > 0)
            {
                tempPath = tempPath.Substring(tempPath.IndexOf("/"));
            }
            mConfig.iConPath = tempPath;
            GUILayout.Label(mConfig.iConPath);
        }else
        {
            mConfig.iConPath = "";
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label(" 权限相关", GUILayout.MaxWidth(250));
        isAuto = EditorGUILayout.Toggle(isAuto, GUILayout.MaxWidth(15));
        GUILayout.Label("自动填充");
        GUILayout.EndHorizontal();

        string NSCameraUsageDescription = string.IsNullOrEmpty(mConfig.NSCameraUsageDescription) ? "App需要您的同意,才能访问相机" : mConfig.NSCameraUsageDescription;
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("NSCameraUsageDescription", GUILayout.MaxWidth(250));
        mConfig.NSCameraUsageDescription = EditorGUILayout.TextArea(NSCameraUsageDescription, GUILayout.MaxHeight(44));
        GUILayout.EndHorizontal();

        string NSLocationWhenInUseUsageDescription = string.IsNullOrEmpty(mConfig.NSLocationWhenInUseUsageDescription) ? "防作弊房间App需要您的同意,才能在使用期间访问位置" : mConfig.NSLocationWhenInUseUsageDescription;
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("NSLocationWhenInUseUsageDescription", GUILayout.MaxWidth(250));
        mConfig.NSLocationWhenInUseUsageDescription = EditorGUILayout.TextArea(NSLocationWhenInUseUsageDescription, GUILayout.MaxHeight(44));
        GUILayout.EndHorizontal();

        string NSMicrophoneUsageDescription = string.IsNullOrEmpty(mConfig.NSMicrophoneUsageDescription) ? "App需要您的同意,才能访问麦克风" : mConfig.NSMicrophoneUsageDescription;
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("NSMicrophoneUsageDescription", GUILayout.MaxWidth(250));
        mConfig.NSMicrophoneUsageDescription = EditorGUILayout.TextArea(NSMicrophoneUsageDescription, GUILayout.MaxHeight(44));
        GUILayout.EndHorizontal();

        string CFBundleTypeRole = string.IsNullOrEmpty(mConfig.CFBundleTypeRole) ? "Editor" : mConfig.CFBundleTypeRole;
        mConfig.CFBundleTypeRole = EditorGUILayout.TextField("CFBundleTypeRole", CFBundleTypeRole);
        string CFBundleURLName = mConfig.CFBundleURLName;
        if (isAuto)
        {
            CFBundleURLName = mConfig.url_schemes_name;
            mConfig.CFBundleURLName = CFBundleURLName;
            GUILayout.BeginHorizontal();
            GUILayout.Label("CFBundleURLName", GUILayout.MaxWidth(150));
            GUILayout.Label(CFBundleURLName, GUILayout.MaxWidth(250));
            GUILayout.EndHorizontal();
        }
        else
        {
            mConfig.CFBundleURLName = EditorGUILayout.TextField("CFBundleURLName", CFBundleURLName);
        }

        showCFBundleURLSchemes = EditorGUILayout.Foldout(showCFBundleURLSchemes, "CFBundleURLSchemes");
        if (showCFBundleURLSchemes)
        {
            if (mConfig.CFBundleURLSchemes == null)
            {
                mConfig.CFBundleURLSchemes = new List<string>();
            }

            List<string> CFBundleURLSchemes = mConfig.CFBundleURLSchemes;
            if (isAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label(mConfig.wxAppId, GUILayout.MaxWidth(250));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label(mConfig.url_schemes_name, GUILayout.MaxWidth(250));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label("xianliao" + mConfig.xianLiaoAppId, GUILayout.MaxWidth(250));
                GUILayout.EndHorizontal();
            }

            for (int i = 0; i < CFBundleURLSchemes.Count; i++)
            {
                if (isAuto)
                {
                    if (CFBundleURLSchemes[i].Equals(mConfig.wxAppId) ||
                        CFBundleURLSchemes[i].Equals(mConfig.url_schemes_name) ||
                        CFBundleURLSchemes[i].Equals("xianliao" + mConfig.xianLiaoAppId))
                    {
                        continue;
                    }
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                CFBundleURLSchemes[i] = EditorGUILayout.TextArea(CFBundleURLSchemes[i], GUILayout.MaxWidth(250));
                if (GUILayout.Button("X", GUILayout.MaxWidth(44)))
                {
                    mConfig.CFBundleURLSchemes.Remove(CFBundleURLSchemes[i]);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            tempURLScheme = EditorGUILayout.TextArea(tempURLScheme, GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add", GUILayout.MaxWidth(44)))
            {
                if (!string.IsNullOrEmpty(tempURLScheme))
                {
                    if (mConfig.CFBundleURLSchemes.Contains(tempURLScheme))
                    {
                        this.ShowNotification(new GUIContent("已经存在"));
                    }
                    else
                    {
                        mConfig.CFBundleURLSchemes.Add(tempURLScheme);
                    }
                    tempURLScheme = "";
                }
            }
            GUILayout.EndHorizontal();
        }

        showLSApplicationQueriesSchemes = EditorGUILayout.Foldout(showLSApplicationQueriesSchemes, "LSApplicationQueriesSchemes");
        if (showLSApplicationQueriesSchemes)
        {

            if (mConfig.LSApplicationQueriesSchemes == null)
            {
                mConfig.LSApplicationQueriesSchemes = new List<string>();
            }

            List<string> LSApplicationQueriesSchemes = mConfig.LSApplicationQueriesSchemes;

            if (isAuto && LSApplicationQueriesSchemes.Count == 0)
            {
                LSApplicationQueriesSchemes.Add("wechat");
                LSApplicationQueriesSchemes.Add("weixin");
                LSApplicationQueriesSchemes.Add("xianliao");
            }

            for (int i = 0; i < LSApplicationQueriesSchemes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                LSApplicationQueriesSchemes[i] = EditorGUILayout.TextArea(LSApplicationQueriesSchemes[i], GUILayout.MaxWidth(250));
                if (GUILayout.Button("X", GUILayout.MaxWidth(44)))
                {
                    mConfig.LSApplicationQueriesSchemes.Remove(LSApplicationQueriesSchemes[i]);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            tempLSApplicationQueriesScheme = EditorGUILayout.TextArea(tempLSApplicationQueriesScheme, GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add", GUILayout.MaxWidth(44)))
            {
                if (!string.IsNullOrEmpty(tempLSApplicationQueriesScheme))
                {
                    if (mConfig.LSApplicationQueriesSchemes.Contains(tempLSApplicationQueriesScheme))
                    {
                        this.ShowNotification(new GUIContent("已经存在"));
                    }
                    else
                    {
                        mConfig.LSApplicationQueriesSchemes.Add(tempLSApplicationQueriesScheme);
                    }
                    tempLSApplicationQueriesScheme = "";
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label(" 编译设置", GUILayout.MaxWidth(250));
        isComplierAuto = EditorGUILayout.Toggle(isComplierAuto, GUILayout.MaxWidth(15));
        GUILayout.Label("自动填充");
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("CFBundleDevelopmentRegion", GUILayout.MaxWidth(250));
        string CFBundleDevelopmentRegion = mConfig.CFBundleDevelopmentRegion;
        if (isComplierAuto && string.IsNullOrEmpty(CFBundleDevelopmentRegion))
        {
            CFBundleDevelopmentRegion = "zh_CN";
        }
        mConfig.CFBundleDevelopmentRegion = EditorGUILayout.TextArea(CFBundleDevelopmentRegion, GUILayout.MaxWidth(250), GUILayout.MaxHeight(20));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("ENABLE_BITCODE", GUILayout.MaxWidth(250));
        tempBitCodeIndex = EditorGUILayout.Popup(tempBitCodeIndex, flags, GUILayout.MaxWidth(250), GUILayout.MaxHeight(20));
        mConfig.ENABLE_BITCODE = flags[tempBitCodeIndex];
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("GCC_ENABLE_OBJC_EXCEPTIONS", GUILayout.MaxWidth(250));
        tempGCCObjCFLag = EditorGUILayout.Popup(tempGCCObjCFLag, flags, GUILayout.MaxWidth(250), GUILayout.MaxHeight(20));
        mConfig.GCC_ENABLE_OBJC_EXCEPTIONS = flags[tempGCCObjCFLag];
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("DEBUG_INFORMATION_FORMAT", GUILayout.MaxWidth(250));
        string DEBUG_INFORMATION_FORMAT = mConfig.DEBUG_INFORMATION_FORMAT;
        if (isComplierAuto && string.IsNullOrEmpty(DEBUG_INFORMATION_FORMAT))
        {
            DEBUG_INFORMATION_FORMAT = "dwarf";
        }
        mConfig.DEBUG_INFORMATION_FORMAT = EditorGUILayout.TextArea(DEBUG_INFORMATION_FORMAT, GUILayout.MaxWidth(250), GUILayout.MaxHeight(20));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Label("OTHER_LDFLAGS", GUILayout.MaxWidth(250));
        string OTHER_LDFLAGS = mConfig.OTHER_LDFLAGS;
        if (isComplierAuto && string.IsNullOrEmpty(OTHER_LDFLAGS))
        {
            OTHER_LDFLAGS = "-ObjC -force_load \"$(SRCROOT)/Libraries/Plugins/ios/iOSWxSdk/libWeChatSDK.a\"";
        }
        mConfig.OTHER_LDFLAGS = EditorGUILayout.TextArea(OTHER_LDFLAGS, GUILayout.MaxHeight(20));
        GUILayout.EndHorizontal();

        showLIBRARY_SEARCH_PATHS = EditorGUILayout.Foldout(showLIBRARY_SEARCH_PATHS, "LIBRARY_SEARCH_PATHS");
        if (showLIBRARY_SEARCH_PATHS)
        {
            if (mConfig.LIBRARY_SEARCH_PATHS == null)
            {
                mConfig.LIBRARY_SEARCH_PATHS = new List<string>();
            }

            if (mConfig.LIBRARY_SEARCH_PATHS.Count == 0)
            {
                if (isComplierAuto)
                {
                    string[] libPathNames = new string[] { "$(PROJECT_DIR)/Libraries",
                                                            "$(PROJECT_DIR)/Libraries/Plugins/ios/iOSWxSdk",
                                                            "$(PROJECT_DIR)/Libraries/Plugins/ios/iOSYuvaSdk",
                                                            "$(PROJECT_DIR)/Libraries/Plugins/ios/bugly",
                                                            "$(PROJECT_DIR)/Libraries/Plugins/ios/iOSXliaoSdk",
                                                            "$(PROJECT_DIR)/Libraries/Plugins/ios/bdSDK",
                                                            };
                    for (int i = 0; i < libPathNames.Length; i++)
                    {
                        mConfig.LIBRARY_SEARCH_PATHS.Add(libPathNames[i]);
                    }
                }
            }

            List<string> LIBRARY_SEARCH_PATHS = mConfig.LIBRARY_SEARCH_PATHS;
            for (int i = 0; i < LIBRARY_SEARCH_PATHS.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (isComplierAuto)
                {
                    GUILayout.Label(LIBRARY_SEARCH_PATHS[i], GUILayout.MaxWidth(500));
                }
                else
                {
                    EditorGUILayout.TextArea(LIBRARY_SEARCH_PATHS[i], GUILayout.MaxWidth(500));
                }
                if (!isComplierAuto && GUILayout.Button("X", GUILayout.MaxWidth(44)))
                {
                    mConfig.LIBRARY_SEARCH_PATHS.Remove(LIBRARY_SEARCH_PATHS[i]);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            tempLIBRARY_SEARCH_PATH = EditorGUILayout.TextArea(tempLIBRARY_SEARCH_PATH, GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add", GUILayout.MaxWidth(44)))
            {
                if (!string.IsNullOrEmpty(tempLIBRARY_SEARCH_PATH))
                {
                    int index = -1;
                    if (mConfig.frameWorkList != null)
                    {
                        index = mConfig.LIBRARY_SEARCH_PATHS.FindIndex((string f1) => { return tempLIBRARY_SEARCH_PATH.Equals(f1); });
                    }
                    else
                    {
                        mConfig.LIBRARY_SEARCH_PATHS = new List<string>();
                    }
                    if (index > 0)
                    {
                        this.ShowNotification(new GUIContent("已经存在"));
                    }
                    else
                    {
                        mConfig.LIBRARY_SEARCH_PATHS.Add(tempLIBRARY_SEARCH_PATH);
                    }
                    tempLIBRARY_SEARCH_PATH = "";
                }
            }
            GUILayout.EndHorizontal();
        }

        showFRAMEWORK_SEARCH_PATHS = EditorGUILayout.Foldout(showFRAMEWORK_SEARCH_PATHS, "FRAMEWORK_SEARCH_PATHS");
        if (showFRAMEWORK_SEARCH_PATHS)
        {
            if (mConfig.FRAMEWORK_SEARCH_PATHS == null)
            {
                mConfig.FRAMEWORK_SEARCH_PATHS = new List<string>();
            }

            if (mConfig.FRAMEWORK_SEARCH_PATHS.Count == 0)
            {
                if (isComplierAuto)
                {
                    string[] framePathNames = new string[] { "$(PROJECT_DIR)/Libraries/Plugins/ios/bdSDK"
                                                            };
                    for (int i = 0; i < framePathNames.Length; i++)
                    {
                        mConfig.FRAMEWORK_SEARCH_PATHS.Add(framePathNames[i]);
                    }
                }
            }

            List<string> FRAMEWORK_SEARCH_PATHS = mConfig.FRAMEWORK_SEARCH_PATHS;
            for (int i = 0; i < FRAMEWORK_SEARCH_PATHS.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (isComplierAuto)
                {
                    GUILayout.Label(FRAMEWORK_SEARCH_PATHS[i], GUILayout.MaxWidth(500));
                }
                else
                {
                    EditorGUILayout.TextArea(FRAMEWORK_SEARCH_PATHS[i], GUILayout.MaxWidth(500));
                }
                if (!isComplierAuto && GUILayout.Button("X", GUILayout.MaxWidth(44)))
                {
                    mConfig.FRAMEWORK_SEARCH_PATHS.Remove(FRAMEWORK_SEARCH_PATHS[i]);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            tempFRAMEWORK_SEARCH_PATH = EditorGUILayout.TextArea(tempFRAMEWORK_SEARCH_PATH, GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add", GUILayout.MaxWidth(44)))
            {
                if (!string.IsNullOrEmpty(tempFRAMEWORK_SEARCH_PATH))
                {
                    int index = -1;
                    if (mConfig.FRAMEWORK_SEARCH_PATHS != null)
                    {
                        index = mConfig.FRAMEWORK_SEARCH_PATHS.FindIndex((string f1) => { return tempFRAMEWORK_SEARCH_PATH.Equals(f1); });
                    }
                    else
                    {
                        mConfig.FRAMEWORK_SEARCH_PATHS = new List<string>();
                    }
                    if (index > 0)
                    {
                        this.ShowNotification(new GUIContent("已经存在"));
                    }
                    else
                    {
                        mConfig.FRAMEWORK_SEARCH_PATHS.Add(tempFRAMEWORK_SEARCH_PATH);
                    }
                    tempFRAMEWORK_SEARCH_PATH = "";
                }
            }
            GUILayout.EndHorizontal();
        }

        showFrameworks = EditorGUILayout.Foldout(showFrameworks, "frameworks");
        if (showFrameworks)
        {
            if (mConfig.frameWorkList == null)
            {
                mConfig.frameWorkList = new List<iOSFrameWork>();
            }

            if (mConfig.frameWorkList.Count == 0)
            {
                if (isComplierAuto)
                {
                    string[] libNames = new string[] { "libz.tbd",
                                                        "libz.1.tbd",
                                                        "libc++.tbd",
                                                        "libsqlite3.0.tbd",
                                                        "libstdc++.6.0.9.tbd",
                                                        "CoreTelephony.framework",
                                                        "ImageIO.framework",
                                                        "MobileCoreServices.framework",
                                                        "SafariServices.framework",
                                                        "Security.framework",
                                                        "StoreKit.framework",
                                                        "SystemConfiguration.framework",
                                                        //"BaiduMapAPI_Base.framework",
                                                        //"BaiduMapAPI_Location.framework",
                                                        //"Bugly.framework"
                    };
                    for (int i = 0; i < libNames.Length; i++)
                    {
                        iOSFrameWork frame = new iOSFrameWork();
                        frame.name = libNames[i];
                        frame.flag = false;
                        mConfig.frameWorkList.Add(frame);
                    }
                }
            }

            List<iOSFrameWork> frameWorkList = mConfig.frameWorkList;
            for (int i = 0; i < frameWorkList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                if (isComplierAuto)
                {
                    GUILayout.Label(frameWorkList[i].name, GUILayout.MaxWidth(250));
                }
                else
                {
                    EditorGUILayout.TextArea(frameWorkList[i].name, GUILayout.MaxWidth(250));
                }
                if (!isComplierAuto && GUILayout.Button("X", GUILayout.MaxWidth(44)))
                {
                    mConfig.frameWorkList.Remove(frameWorkList[i]);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            tempFrameworkName = EditorGUILayout.TextArea(tempFrameworkName, GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add", GUILayout.MaxWidth(44)))
            {
                if (!string.IsNullOrEmpty(tempFrameworkName))
                {
                    int index = -1;
                    if (mConfig.frameWorkList != null)
                    {
                        index = mConfig.frameWorkList.FindIndex((iOSFrameWork f1) => { return tempFrameworkName.Equals(f1.name); });
                    }
                    else
                    {
                        mConfig.frameWorkList = new List<iOSFrameWork>();
                    }
                    if (index > 0)
                    {
                        this.ShowNotification(new GUIContent("已经存在"));
                    }
                    else
                    {
                        iOSFrameWork frame = new iOSFrameWork();
                        frame.name = tempFrameworkName;
                        frame.flag = false;
                        mConfig.frameWorkList.Add(frame);
                    }
                    tempFrameworkName = "";
                }
            }
            GUILayout.EndHorizontal();
        }


        GUILayout.Space(20);
        if (GUILayout.Button("保存", GUILayout.Width(200)))
        {
            if (isAuto)
            {
                if (mConfig.CFBundleURLSchemes == null)
                {
                    mConfig.CFBundleURLSchemes = new List<string>();
                }
                if (!string.IsNullOrEmpty(mConfig.wxAppId) && !mConfig.CFBundleURLSchemes.Contains(mConfig.wxAppId))
                {
                    mConfig.CFBundleURLSchemes.Add(mConfig.wxAppId);
                }
                if (!string.IsNullOrEmpty(mConfig.url_schemes_name) && !mConfig.CFBundleURLSchemes.Contains(mConfig.url_schemes_name))
                {
                    mConfig.CFBundleURLSchemes.Add(mConfig.url_schemes_name);
                }
                string xlAppId = "xianliao" + mConfig.xianLiaoAppId;
                if (!string.IsNullOrEmpty(xlAppId) && !mConfig.CFBundleURLSchemes.Contains(xlAppId))
                {
                    mConfig.CFBundleURLSchemes.Add(xlAppId);
                }
            }
            SaveSetting();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnFocus()
    {
        Debug.Log("OnFocus");

        if (mConfig != null)
        {
            return;
        }

        string path = ConfigPath();
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            mConfig = JsonUtility.FromJson<iOSConfig>(content);
        } else
        {
            mConfig = new iOSConfig();
        }
    }

    void SaveSetting()
    {
        if (mConfig != null)
        {

            //Debug.Log(mConfig.frameWorkList.Count);

            string content = JsonUtility.ToJson(mConfig);
            string path = ConfigPath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, content);

            this.ShowNotification(new GUIContent("保存成功"));
        }
    }

    static string ConfigPath()
    {
        return Application.dataPath + "/editor/config/" + mConfigFileName;
    }

    // build 完成时
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        Debug.Log("path: " + path);
        if (target == BuildTarget.iOS)
        {
            OnPostprocessBuild_ios(target, path);
        }
    }

    /// <summary>
    /// 在生成xcode工程后，自动将一些ios配置写入xcode工程
    /// </summary>
    /// <param name="buildTarget"></param>
    /// <param name="path"></param>
    public static void OnPostprocessBuild_ios(BuildTarget buildTarget, string path)
    {
        //#if UNITY_IOS
        //Debug.Log ("path: " + path);
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        // 读取配置，一定要保证配置的正确性
        string configPath = ConfigPath();
        string configContent = File.ReadAllText(configPath);
        iOSConfig config = JsonUtility.FromJson<iOSConfig>(configContent);

        #region 修改工程
        string projPath = PBXProject.GetPBXProjectPath(path);
        Debug.Log("projPath: " + projPath);
        PBXProject proj = new PBXProject();
        string fileText = File.ReadAllText(projPath);
        proj.ReadFromString(fileText);
        //Debug.Log ("fileText: " + fileText);

        string targetName = PBXProject.GetUnityTargetName();//Unity-iPhone
        string targetGuid = proj.TargetGuidByName(targetName);
        //Debug.Log ("targetName: " + targetName);
        //Debug.Log ("targetGuid: " + targetGuid);

        // BuildPropertys
        proj.SetBuildProperty(targetGuid, "CFBundleDevelopmentRegion", config.CFBundleDevelopmentRegion);
        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", config.ENABLE_BITCODE);
        proj.SetBuildProperty(targetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", config.GCC_ENABLE_OBJC_EXCEPTIONS);
        proj.SetBuildProperty(targetGuid, "DEBUG_INFORMATION_FORMAT", config.DEBUG_INFORMATION_FORMAT);

        proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", config.OTHER_LDFLAGS);
        for (int i = 0; i < config.LIBRARY_SEARCH_PATHS.Count; i++)
        {
            proj.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", config.LIBRARY_SEARCH_PATHS[i]);
        }
        for (int i = 0; i < config.FRAMEWORK_SEARCH_PATHS.Count; i++)
        {
            proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", config.FRAMEWORK_SEARCH_PATHS[i]);
        }

        // Framework
        //string fileguid = proj.FindFileGuidByProjectPath("libiconv.2.dylib");
        //proj.RemoveFile(fileguid);

        foreach (iOSFrameWork framework in config.frameWorkList)
        {
            proj.AddFrameworkToProject(targetGuid, framework.name, framework.flag);
        }

        // save changed
        File.WriteAllText(projPath, proj.WriteToString());
        #endregion


        #region 修改plist
        string plistPath = Path.Combine(path, "Info.plist");
        Debug.Log("plistPath: " + plistPath);
        PlistDocument plist = new PlistDocument();
        string plistFileText = File.ReadAllText(plistPath);
        plist.ReadFromString(plistFileText);
        PlistElementDict rootDict = plist.root;

        rootDict.SetString("NSCameraUsageDescription", config.NSCameraUsageDescription);
        rootDict.SetString("NSLocationWhenInUseUsageDescription", config.NSLocationWhenInUseUsageDescription);
        rootDict.SetString("NSMicrophoneUsageDescription", config.NSMicrophoneUsageDescription);

        rootDict.SetString("wx_app_id", config.wxAppId);
        rootDict.SetString("wx_app_secret", config.wxAppSecret);
        rootDict.SetString("xianliao_app_id", config.xianLiaoAppId);
        rootDict.SetString("yunwa_app_id", config.yunvaAppId);
        rootDict.SetString("baidu_app_key", config.baiduAppKey);
        rootDict.SetString("bugly_app_id", config.buglyAppId);
        rootDict.SetString("url_schemes_name", config.url_schemes_name);

        //CFBundleURLTypes
        PlistElementArray array = rootDict.CreateArray("CFBundleURLTypes");
        PlistElementDict dict = array.AddDict();
        dict.SetString("CFBundleTypeRole", config.CFBundleTypeRole);
        dict.SetString("CFBundleURLName", config.CFBundleURLName);

        PlistElementArray CFBundleURLSchemesArr = dict.CreateArray("CFBundleURLSchemes");
        int CFBundleURLSchemesCount = config.CFBundleURLSchemes.Count;
        for (int i = 0; i < CFBundleURLSchemesCount; i++)
        {
            CFBundleURLSchemesArr.AddString(config.CFBundleURLSchemes[i]);
        }

        // LSApplicationQueriesSchemes
        PlistElementArray LSApplicationQueriesSchemesArr = rootDict.CreateArray("LSApplicationQueriesSchemes");
        int LSApplicationQueriesSchemesCount = config.LSApplicationQueriesSchemes.Count;
        for (int i = 0; i < LSApplicationQueriesSchemesCount; i++)
        {
            LSApplicationQueriesSchemesArr.AddString(config.LSApplicationQueriesSchemes[i]);
        }

        // 保存修改
        File.WriteAllText(plistPath, plist.WriteToString());

        #endregion

        #region 修改文件
        string applicationPath = Path.Combine(path, "Classes/UnityAppController.mm");
        string applicationContent = File.ReadAllText(applicationPath);

        string oriStr = "_window			= [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];";
        string destStr = "NSString* deviceVersion = [self getDeviceVersion]; if ([deviceVersion isEqualToString: @\"iPhone10,3\"] || [deviceVersion isEqualToString: @\"iPhone10,6\"]){CGRect bounds = CGRectMake(34, 0, 744, 360);_window = [[UIWindow alloc] initWithFrame: bounds];}else{_window = [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];}";
        applicationContent = applicationContent.Replace(oriStr, destStr);

        oriStr = "#include <mach/mach_time.h>";
        destStr = "#include <mach/mach_time.h> \n #import <sys/utsname.h>";
        applicationContent = applicationContent.Replace(oriStr, destStr);

        oriStr = "- (void)applicationDidEnterBackground:(UIApplication*)application";
        destStr = " - (NSString*)getDeviceVersion{struct utsname systemInfo; uname(&systemInfo); NSString* deviceVersion = [NSString stringWithCString: systemInfo.machine encoding: NSUTF8StringEncoding];return deviceVersion;} \n - (void)applicationDidEnterBackground:(UIApplication*)application";
        applicationContent = applicationContent.Replace(oriStr, destStr);

        if (File.Exists(applicationPath))
        {
            File.Delete(applicationPath);
            File.WriteAllText(applicationPath, applicationContent);
        }
        #endregion

        #region 移动文件
        string oriDir = Application.dataPath + "/Plugins/ios/bdSDK";
        string destDir = path + "/Libraries/Plugins/ios/bdSDK";
        Debug.Log("oriDir = " + oriDir + ",destDir =" + destDir);

        CopyFolder(oriDir, destDir);

        #endregion

        #region 修改iCOn
        string iconName = Path.GetFileName(config.iConPath);
        string oriIconPath = Application.dataPath + config.iConPath;
        string destIconPath = path + "/Unity-iPhone/Images.xcassets/AppIcon.appiconset/" + iconName;
        File.Copy(oriIconPath, destIconPath);

        string iConConfigPath = path + "/Unity-iPhone/Images.xcassets/AppIcon.appiconset/Contents.json";
        string content = File.ReadAllText(iConConfigPath);

        content = content.Replace("\"images\" : [", "\"images\" : [{\"size\" : \"1024x1024\",\"idiom\" : \"ios-marketing\",\"scale\" : \"1x\",\"filename\" : \""+iconName+"\"},");
        File.WriteAllText(iConConfigPath, content);
        #endregion

    }
    #region 文件夹操作
    /// <summary>
    /// 复制文件夹中的所有文件夹与文件到另一个文件夹
    /// </summary>
    /// <param name="sourcePath">源文件夹</param>
    /// <param name="destPath">目标文件夹</param>
    public static void CopyFolder(string sourcePath, string destPath)
    {
        if (Directory.Exists(sourcePath))
        {
            if (!Directory.Exists(destPath))
            {
                //目标目录不存在则创建
                try
                {
                    Directory.CreateDirectory(destPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("创建目标目录失败：" + ex.Message);
                }
            }
            //获得源文件下所有文件
            List<string> files = new List<string>(Directory.GetFiles(sourcePath));
            files.ForEach(c =>
            {
                string destFile = Path.Combine(destPath, Path.GetFileName(c));
                File.Copy(c, destFile, true);//覆盖模式
            });
            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
            folders.ForEach(c =>
            {
                string destDir = Path.Combine(destPath, Path.GetFileName(c));
                //采用递归的方法实现
                CopyFolder(c, destDir);
            });
        }
        else
        {
            throw new DirectoryNotFoundException("源目录不存在！");
        }
    }
    #endregion
    //#endif
}

class iOSConfig
{
    // 第三方key
    public string wxAppId;
    public string wxAppSecret;

    public string xianLiaoAppId;
    public string yunvaAppId;
    public string baiduAppKey;
    public string buglyAppId;
    public string url_schemes_name;

    public string iConPath;

    // 权限控制
    public string NSCameraUsageDescription;
    public string NSLocationWhenInUseUsageDescription;
    public string NSMicrophoneUsageDescription;

    public string CFBundleTypeRole;
    public string CFBundleURLName;
    public List<string> CFBundleURLSchemes;
    public List<string> LSApplicationQueriesSchemes;

    // 编译设置
    public string CFBundleDevelopmentRegion;
    public string ENABLE_BITCODE;
    public string GCC_ENABLE_OBJC_EXCEPTIONS;
    public string DEBUG_INFORMATION_FORMAT;

    public string OTHER_LDFLAGS;
    public List<string> LIBRARY_SEARCH_PATHS;
    public List<string> FRAMEWORK_SEARCH_PATHS;

    public List<iOSFrameWork> frameWorkList;
}

[System.Serializable]
class iOSFrameWork
{
    public string name;
    public bool flag;
}

class iOSAbout : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.Space(80);
        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        GUILayout.BeginVertical();
        GUIStyle bb = new GUIStyle();
        bb.normal.textColor = new Color(1, 1, 1);   //设置字体颜色的
        bb.fontSize = 20;       //当然，这是字体颜色
        GUILayout.Label("iOS环境配置", bb);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(35);
        GUILayout.Label("V " + iOSSetting.versionName);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}

class iOSConfirm : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(40);
        GUIStyle bb = new GUIStyle();
        bb.normal.textColor = new Color(1, 1, 1);   //设置字体颜色的
        bb.fontSize = 20;       //当然，这是字体颜色
        GUILayout.Label("确定要卸载么?", bb);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("取消"))
        {
            Close();
        };

        if (GUILayout.Button("确定")){
            string path = Application.dataPath + "/editor/iOS/iOSSetting.cs";
            File.Delete(path);
        };

        GUILayout.EndHorizontal();
    }
}
