﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UFrame;

public class CreateAssetBundles
{
    static List<string> groupBundleLst = new List<string>();
    static void BuildLuaBundle(StreamWriter sw, string subDir, string sourceDir)
    {
        string[] files = Directory.GetFiles(sourceDir + subDir, "*.bytes");
        //如果不把“/”换成"_"会出现打包不成功，原因是不能创建目录
        string bundleName = subDir == null ? "lua" + UFrameConst.Bundle_Extension : "lua" + subDir.Replace('/', '_') + UFrameConst.Bundle_Extension;
        bundleName = bundleName.ToLower();

        string swContent = "";
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Replace("\\", "/");
            AssetImporter importer = AssetImporter.GetAtPath(files[i]);

            if (importer)
            {
                importer.assetBundleName = bundleName;
                importer.assetBundleVariant = null;
            }

            string assetName = fileName.Substring(sourceDir.Length + 1);
            swContent += assetName + "," + bundleName + "\r\n";
        }

        sw.Write(swContent);
    }

    static void GetAllDirs(string dir, List<string> list)
    {
        string[] dirs = Directory.GetDirectories(dir);
        list.AddRange(dirs);

        for (int i = 0; i < dirs.Length; i++)
        {
            GetAllDirs(dirs[i], list);
        }
    }

    public static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(sourceDir))
        {
            return;
        }

        string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
        int len = sourceDir.Length;

        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
        {
            --len;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string str = files[i].Remove(0, len);
            string dest = destDir + "/" + str;
            if (appendext) dest += ".bytes";
            string dir = Path.GetDirectoryName(dest);
            Directory.CreateDirectory(dir);
            File.Copy(files[i], dest, true);
        }
    }

    static void CreateCopyLuaDir()
    {
        string luaCopyDir = Application.dataPath + UFrameConst.Lua_Copy_Dir;

        if (!File.Exists(luaCopyDir))
        {
            Directory.CreateDirectory(luaCopyDir);
        }
    }

    static void CreateDir(string path)
    {
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    static void ClearAllLuaFiles()
    {
        string luaBundlePath = Path.Combine(Application.streamingAssetsPath, UFrameConst.Lua_Bundle_Dir);

        //清空bundle
        if (Directory.Exists(luaBundlePath))
        {
            string[] files = Directory.GetFiles(luaBundlePath, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
                
            }
            Directory.Delete(luaBundlePath, true);
        }

        //清空copy过来的lua.byte
        string luaCopyPath = Path.Combine(Application.dataPath, UFrameConst.Lua_Copy_Dir);
        if (Directory.Exists(luaCopyPath))
        {
            string[] files = Directory.GetFiles(luaCopyPath, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);

            }

            Directory.Delete(luaCopyPath, true);
        }
    }

    public static void BuildPerpare()
    {
        string fileMapFullPath = Path.Combine(Application.dataPath, UFrameConst.GameResources_Dir);
        fileMapFullPath = Path.Combine(fileMapFullPath, UFrameConst.Asset_Bundle_Txt_Name);
        StreamWriter sw = new StreamWriter(fileMapFullPath);

        //清理buildsetting中的场景，只保留入口场景
        OnlyMainScene();

        //LUA打包
        BuildLuaNotJitBundles(sw);

        //其他资源打包
        BuildNormalBundle(sw);

        sw.Dispose();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string assetBundleTxtPathInAssets = "Assets/" + UFrameConst.GameResources_Dir + "/" + UFrameConst.Asset_Bundle_Txt_Name;
        var importerFileMap = AssetImporter.GetAtPath(assetBundleTxtPathInAssets);
        importerFileMap.assetBundleName = Path.GetFileNameWithoutExtension(UFrameConst.Asset_Bundle_Txt_Name) + UFrameConst.Bundle_Extension;

        AssetDatabase.Refresh();


    }

    public static void WriteBundleHash(string assetBundleDirectory)
    {
        string bundleHashPath = Path.Combine(Application.dataPath, UFrameConst.GameResources_Dir);
        bundleHashPath = Path.Combine(bundleHashPath, UFrameConst.Bundle_Hash_Txt_Name);
        StreamWriter sw = new StreamWriter(bundleHashPath);
        string swContent = "";
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in abNames)
        {
            string path = assetBundleDirectory + "/" + name;
            if (!File.Exists(path))
            {
                Logger.LogWarp.Log(name + " " + AssetDatabase.GetAssetPathsFromAssetBundle(name));
                continue;
            }
            swContent += name + "=" + UFrame.Util.MD5Util.FileMD5(path) + "\r\n";
            sw.Write(swContent);
        }
        sw.Flush();
        sw.Dispose();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void BundleHashToBundle(string assetBundleDirectory, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
    {
        string assetName = "Assets/" + UFrameConst.GameResources_Dir + "/" + UFrameConst.Bundle_Hash_Txt_Name;
        string[] assetNames = new string[1];
        assetNames[0] = assetName;

        string assetBundleName = Path.GetFileNameWithoutExtension(UFrameConst.Bundle_Hash_Txt_Name) + UFrameConst.Bundle_Extension;

        AssetBundleBuild [] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = assetBundleName;
        buildMap[0].assetNames = assetNames;

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, assetBundleOptions, targetPlatform);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void InitGroupBundleConfig()
    {
        var abCfg = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AB_Config/group_bundle.txt").text;
        //Debug.Log(abCfg);
        string[] group_bundles = abCfg.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        groupBundleLst.AddRange(group_bundles);
    }

    static bool IsContainsGroupBundle(string path)
    {
        for(int i = 0; i < groupBundleLst.Count; i++)
        {
            if (path.Contains(groupBundleLst[i]))
            {
                return true;
            }
        }

        return false;
    }

    [MenuItem("UFrame框架/资源管理/发布模式/创建Bundle/StandaloneWindows")]
    public static void BuildAll_StandaloneWindows()
    {
        InitGroupBundleConfig();
        BuildPerpare();
        string assetBundleDirectory = "Assets/StreamingAssets/" + UFrameConst.Bundle_Root_Dir;
        //BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
        //    BuildAssetBundleOptions.ChunkBasedCompression |
        //    BuildAssetBundleOptions.DisableWriteTypeTree;
        BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression;

        BuildTarget targetPlatform = BuildTarget.StandaloneWindows;

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, assetBundleOptions, targetPlatform);


        //把Manifest文件加一个后缀，方便下载
        ModifyManifestFileName();

        WriteBundleHash(assetBundleDirectory);

        BundleHashToBundle(assetBundleDirectory, assetBundleOptions, targetPlatform);

        EditorUtility.DisplayDialog("", "Assetbundle打包完毕", "确定");
    }

    [MenuItem("UFrame框架/资源管理/发布模式/创建Bundle/Android")]
    public static void BuildAll_Android()
    {
        InitGroupBundleConfig();
        BuildPerpare();
        string assetBundleDirectory = "Assets/StreamingAssets/" + UFrameConst.Bundle_Root_Dir;
        BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DisableWriteTypeTree;
        BuildTarget targetPlatform = BuildTarget.Android;

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            assetBundleOptions, targetPlatform);


        //把Manifest文件加一个后缀，方便下载
        ModifyManifestFileName();

        WriteBundleHash(assetBundleDirectory);

        BundleHashToBundle(assetBundleDirectory, assetBundleOptions, targetPlatform);

        EditorUtility.DisplayDialog("", "Assetbundle打包完毕", "确定");
    }

    [MenuItem("UFrame框架/资源管理/发布模式/创建Bundle/IOS")]
    public static void BuildAll_IOS()
    {
        InitGroupBundleConfig();
        BuildPerpare();
        string assetBundleDirectory = "Assets/StreamingAssets/" + UFrameConst.Bundle_Root_Dir;
        BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression |
            BuildAssetBundleOptions.DisableWriteTypeTree;

        BuildTarget targetPlatform = BuildTarget.iOS;
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            assetBundleOptions, targetPlatform);


        //把Manifest文件加一个后缀，方便下载
        ModifyManifestFileName();

        WriteBundleHash(assetBundleDirectory);

        BundleHashToBundle(assetBundleDirectory, assetBundleOptions, targetPlatform);

        EditorUtility.DisplayDialog("", "Assetbundle打包完毕", "确定");
    }

    public static void OnlyMainScene()
    {
        EditorBuildSettingsScene[] onlyMain = new EditorBuildSettingsScene[1];
        string fullPath = "Assets/" + UFrameConst.Main_Scene_Path;
        onlyMain[0] = new EditorBuildSettingsScene(fullPath, true);
        EditorBuildSettings.scenes = onlyMain;
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

    }

    public static void BuildLuaNotJitBundles(StreamWriter sw)
    {
        ClearAllLuaFiles();

        string bundleRootPath = Path.Combine(Application.streamingAssetsPath, UFrameConst.Bundle_Root_Dir);
        bundleRootPath = bundleRootPath.Replace("\\", "/");
        CreateDir(bundleRootPath);

        string luaCopyDir = Path.Combine(Application.dataPath, UFrameConst.Lua_Copy_Dir);
        luaCopyDir = luaCopyDir.Replace("\\", "/");
        CreateDir(luaCopyDir);
        
        CopyLuaBytesFiles(UFrameConst.luaDir, luaCopyDir);
        CopyLuaBytesFiles(UFrameConst.toluaDir, luaCopyDir);

        AssetDatabase.Refresh();
        //到这里，完成lua的copy，以.byte结尾
        
        //获得temp下的子目录
        List<string> dirs = new List<string>();
        GetAllDirs(luaCopyDir, dirs);
        //temp下的子目录下的文件打包
        string copyDirWithAssets = "Assets/" + UFrameConst.Lua_Copy_Dir;
        for (int i = 0; i < dirs.Count; i++)
        {
            string str = dirs[i].Remove(0, luaCopyDir.Length);
            BuildLuaBundle(sw, str.Replace('\\', '/'), copyDirWithAssets);
        }

        //temp下的非子目录下文件打包
        BuildLuaBundle(sw, null, copyDirWithAssets);
        sw.Flush();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    static void BuildNormalBundle(StreamWriter sw)
    {
        string GameResourcePath = "Assets/" + UFrameConst.GameResources_Dir;
        string swContent = "";
        string[] filePaths = Directory.GetFiles(GameResourcePath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; ++i)
        {
            string filePath = filePaths[i].Replace("\\", "/");
            //".meta 不打包"
            if (filePath.EndsWith(".meta"))
            {
                continue;
            }

            //Asset_Bundle_Txt_Name, Bundle_Hash_Txt_Name不打
            if (
                filePath.Contains(UFrameConst.Asset_Bundle_Txt_Name) ||
                filePath.Contains(UFrameConst.Bundle_Hash_Txt_Name))
            {
                continue;
            }

            //scenes目录下只打 *.unity文件
            if(filePath.Contains("/scenes/") && Path.GetExtension(filePath) != ".unity")
            {
                continue;
            }

            //if (!IsContainsGroupBundle(filePath))
            {
                string bundleName = filePath.Substring(GameResourcePath.Length + 1);
                if (IsContainsGroupBundle(filePath))
                {
                    bundleName = bundleName.Substring(0, bundleName.LastIndexOf("/"));
                }
                bundleName = bundleName.Replace("/", "_");
                bundleName = Path.GetFileNameWithoutExtension(bundleName);
                bundleName += UFrameConst.Bundle_Extension;

                string bundlePath = filePath.Substring(GameResourcePath.Length + 1);
                int lastSplitIndex = bundlePath.LastIndexOf('.');
                bundlePath = bundlePath.Substring(0, lastSplitIndex);
                swContent += (bundlePath + "," + bundleName + "\r\n");
                var importer = AssetImporter.GetAtPath(filePath);
                importer.assetBundleName = bundleName;
            }
            //else
            //{
            //    string bundleName = filePath.Substring(GameResourcePath.Length + 1);
            //    bundleName = bundleName.Substring(0, bundleName.LastIndexOf("/"));
            //    bundleName = bundleName.Replace("/", "_");
            //    bundleName = Path.GetFileNameWithoutExtension(bundleName);
            //    bundleName += UFrameConst.Bundle_Extension;

            //    string bundlePath = filePath.Substring(GameResourcePath.Length + 1);
            //    int lastSplitIndex = bundlePath.LastIndexOf('.');
            //    bundlePath = bundlePath.Substring(0, lastSplitIndex);
            //    swContent += (bundlePath + "," + bundleName + "\r\n");
            //    var importer = AssetImporter.GetAtPath(filePath);
            //    importer.assetBundleName = bundleName;
            //}

        }

        sw.Write(swContent);
        sw.Flush();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void ModifyManifestFileName()
    {
        string path = "Assets/StreamingAssets/" + UFrameConst.Bundle_Root_Dir + "/" + UFrameConst.Bundle_Root_Dir;
        string destPath = path + UFrameConst.Bundle_Extension;
        if (File.Exists(destPath))
        {
            File.Delete(destPath);
        }
        FileInfo fi = new FileInfo(path);
        fi.MoveTo(destPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //[MenuItem("资源管理/bundle命名")]
    static void SetBundleName()
    {
        string fileMapFullPath = UnityEngine.Application.dataPath + "/GameResources/asset-bundle.txt";
        UnityEngine.Debug.LogError(fileMapFullPath);
        string fileMapPath = fileMapFullPath.Substring(fileMapFullPath.LastIndexOf("Assets/GameResources/"));
        UnityEngine.Debug.LogError(fileMapPath);
        StreamWriter sw = new StreamWriter(fileMapFullPath);
        string swContent = "";
        string[] filePaths = AssetDatabase.GetAllAssetPaths();
        foreach (var filePath in filePaths)
        {
            //不是目录GameResources跳过
            if (!filePath.Contains("GameResources"))
            {
                continue;
            }
            //unix目录格式
            string unixFilePath = filePath.Replace('\\', '/');
            
            //去Assets/GameResources/
            string fileFindPath = unixFilePath.Replace("Assets/GameResources/", "");

            if (fileFindPath.Contains("scene_nav2d/NavMesh"))
            {
                continue;
            }
            
            //没有扩展名的算目录
            int lastSplitIndex = fileFindPath.LastIndexOf('.');
            if (lastSplitIndex <= 0)
            {
                continue;
            }

            ////去扩展名
            //fileFindPath = fileFindPath.Substring(0, lastSplitIndex);
            ////转小写
            //fileFindPath = fileFindPath.ToLower();
            string bundleName;
            //去扩展名
            fileFindPath = fileFindPath.Substring(0, lastSplitIndex);
            if (fileFindPath.Contains("textures/unitylogo"))
            {
                bundleName = GetGroupBundleName(fileFindPath);
            }
            else
            {
                bundleName = GetSingleBundleName(fileFindPath);
            }

            //最后设置bundle名称
            //UnityEngine.Debug.LogError(filePath);
            var importer = AssetImporter.GetAtPath(filePath);
            importer.assetBundleName = bundleName;

            //记录asset和bundle对应关系
            swContent += (fileFindPath + "," + bundleName + "\r\n");
        }
        //对于打包到一起的,单独写对应关系
        //swContent += ("textures/unitylogo" + "," + "textures/unitylogo" + "\r\n");
        
        //不用writeline写配置文件。不同平台换行不一致，runtime解析换行不好判断
        sw.Write(swContent);
        sw.Flush();
        sw.Dispose();
        AssetDatabase.Refresh();
        var importerFileMap = AssetImporter.GetAtPath(fileMapPath);
        importerFileMap.assetBundleName = "asset-bundle";
        AssetDatabase.Refresh();
    }

    static string GetSingleBundleName(string filePath)
    {

        //转小写
        filePath = filePath.ToLower();
        return filePath;
    }

    static string GetGroupBundleName(string filePath)
    {
        int posGroup = filePath.LastIndexOf('/');
        filePath = filePath.Substring(0, posGroup);
        //转小写
        filePath = filePath.ToLower();
        return filePath;
    }

    //[MenuItem("资源管理/创建bundle/StandaloneWindows")]
    //static void BuildAllAssetBundles_Standalone()
    //{
    //    string assetBundleDirectory = "Assets/StreamingAssets/Bundles";
    //    if (!Directory.Exists(assetBundleDirectory))
    //    {
    //        Directory.CreateDirectory(assetBundleDirectory);
    //    }
    //    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //    AssetDatabase.Refresh();
    //}

    //[MenuItem("资源管理/创建bundle/安卓")]
    //static void BuildAllAssetBundles_Android()
    //{
    //    string assetBundleDirectory = "Assets/StreamingAssets/Bundles";
    //    if (!Directory.Exists(assetBundleDirectory))
    //    {
    //        Directory.CreateDirectory(assetBundleDirectory);
    //    }
    //    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
    //    AssetDatabase.Refresh();
    //}

    //[MenuItem("资源管理/创建bundle/IOS")]
    //static void BuildAllAssetBundles_IOS()
    //{
    //    string assetBundleDirectory = "Assets/StreamingAssets/Bundles";
    //    if (!Directory.Exists(assetBundleDirectory))
    //    {
    //        Directory.CreateDirectory(assetBundleDirectory);
    //    }
    //    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
    //    AssetDatabase.Refresh();
    //}
}