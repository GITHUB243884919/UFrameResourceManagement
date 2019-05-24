using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCase_Scene_01
{
    public static void LoadScene()
    {
        //UFrame.ResourceManagement.SceneManagement.GetInstance().LoadScene("scenes/scene_2", () =>
        //{
        //    var getter = ResHelper.LoadGameObject("prefabs/cube");
        //    GameObject go = getter.Get();
        //    Logger.LogWarp.Log(go.name);
        //});

        UFrame.ResourceManagement.SceneManagement.GetInstance().LoadScene(
            "scenes/scene_2", LoadSceneCallback);
    }

    static void LoadSceneCallback()
    {
        Logger.LogWarp.Log("loadsceneCallback");
        LoadCube();
        LoadTerrain();
    }

    static void LoadCube()
    {
        var getter = ResHelper.LoadGameObject("prefabs/cube");
        GameObject go = getter.Get();
        Logger.LogWarp.Log(go.name);
    }

    static void LoadTerrain()
    {
        var getter = ResHelper.LoadGameObject("prefabs/nine_trunk_test");
        var tester = getter.Get();
        tester.transform.position = new Vector3(26, 0, 26);
    }
}
