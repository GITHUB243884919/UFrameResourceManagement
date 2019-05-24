using System.Collections;
using System.Collections.Generic;
using UFrame.ResourceManagement;
using UnityEngine;
using UnityEngine.UI;
public class TestCase_01
{

    public static void Test_Sprite()
    {
        Sprite sp = ResHelper.LoadSprite("ui_sprite/unitylogo/unitylogo", PublicAssetHolderGameObject.GetInstance().Go);
        Debug.LogError("sp != null " + (sp != null));
        GameObject.Find("Image").GetComponent<Image>().sprite = sp;
    }

    public static void Test_Shader()
    {
        Material m = new Material(Shader.Find("UFrame/Test_1"));
        GameObject cube = GameObject.Find("Cube");
        if (cube == null)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
        }
        cube.GetComponent<MeshRenderer>().material = m;
    }

    public static void Test_Async()
    {
        ResHelper.LoadGameObjectAsync("prefabs/cube", (getter) =>
        {
            GameObject go = getter.Get();
            //ResHelper.DestroyGameObject(go);
            Debug.Log("aaaa");
            //Debug.Log("bbbb");
            //ResHelper.LoadGameObjectAsync("prefabs/cube", (getter2) =>
            //{
            //    GameObject go2 = getter2.Get();
            //    //ResHelper.DestroyGameObject(go);
            //});
        });

        Debug.Log("bbbb");
        ResHelper.LoadGameObjectAsync("prefabs/cube", (getter) =>
        {
            GameObject go = getter.Get();
            //ResHelper.DestroyGameObject(go);
        });
    }

    public static void Test_Sync()
    {
        var getter = ResHelper.LoadGameObject("prefabs/cube");
        GameObject go = getter.Get();
        ResHelper.DestroyGameObject(go);

        getter = ResHelper.LoadGameObject("prefabs/cube");
        GameObject go2 = getter.Get();
        //ResHelper.DestroyGameObject(go);

    }


    public static void Test_Vector2_Bit()
    {
        UFrame.Data.Vector2_Bit v2bit1 = new UFrame.Data.Vector2_Bit();
        v2bit1.x = 4;
        v2bit1.y = 9;
        Logger.LogWarp.Log("v2bit1" + v2bit1);
        UFrame.Data.Vector2_Bit v2bit2 = new UFrame.Data.Vector2_Bit(v2bit1.BitData);
        Logger.LogWarp.Log("v2bit2" + v2bit2);
    }

}
