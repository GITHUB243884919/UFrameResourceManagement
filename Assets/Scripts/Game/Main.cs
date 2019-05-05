using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UFrame;
//using UFrame.FSM;
//using UFrame.MessageCenter
using UFrame.ResourceManagement;

namespace Game
{
    /// <summary>
    /// 游戏总入口
    /// 如果成员变量希望跨场景访问，定义成static, 或者是一个单件
    /// </summary>
    public class GameApp : IGameApp 
    {
        public void Awake()
        {

        }

        public void LateUpdate()
        {
            
        }

        public void OnApplicationFocus(bool force)
        {

        }

        public void OnApplicationPause(bool pause)
        {

        }

        public void OnApplicationQuit()
        {

        }

        public void OnMemoryWarnning()
        {

        }

        public void Shutdown()
        {

        }

        public void Start()
        {

            //LoadScene();
            //Test_Vector2_Bit();
            Test_Async();
            //Test_Sync();
        }

        void Test_Async()
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

        void Test_Sync()
        {
            var getter = ResHelper.LoadGameObject("prefabs/cube");
            GameObject go = getter.Get();
            ResHelper.DestroyGameObject(go);

            getter = ResHelper.LoadGameObject("prefabs/cube");
            GameObject go2 = getter.Get();
            //ResHelper.DestroyGameObject(go);

        }


        void Test_Vector2_Bit()
        {
            UFrame.Data.Vector2_Bit v2bit1 = new UFrame.Data.Vector2_Bit();
            v2bit1.x = 4;
            v2bit1.y = 9;
            Logger.LogWarp.Log("v2bit1" + v2bit1);
            UFrame.Data.Vector2_Bit v2bit2 = new UFrame.Data.Vector2_Bit(v2bit1.BitData);
            Logger.LogWarp.Log("v2bit2" + v2bit2);
        }
        void LoadScene()
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


        public void Update(float s)
        {
                        
        }

        void LoadSceneCallback()
        {
            Logger.LogWarp.Log("loadsceneCallback");
            LoadCube();
            LoadTerrain();
        }

        void LoadCube()
        {
            var getter = ResHelper.LoadGameObject("prefabs/cube");
            GameObject go = getter.Get();
            Logger.LogWarp.Log(go.name);
        }

        void LoadTerrain()
        {
            //string path = "";
            //for (int i = 0; i < 2; i++)
            //{
            //    for (int j = 0; j < 2; j++)
            //    {
            //        path = string.Format("terrainslicing/t_{0}_{1}", i, j);
            //        var getter = ResHelper.LoadGameObject(path);
            //        GameObject go = getter.Get();
            //        go.transform.position = new Vector3(100 * i, 0, 100 * j);
            //    }
            //}

            //string path = "terrainslicing/terrain";
            //var getter = ResHelper.LoadGameObject(path);
            //GameObject go = getter.Get();

            //TerrainManager.GetInstance().LoadSlicingTerrain("terrainslicing/terrain/terrain_slicingdata");
            //TerrainManager.GetInstance().LoadSlicingTerrain("terrain");



            //TerrainManager.GetInstance().LoadSlicingMapTile("terrain", pos);
            //Vector3 pos = new Vector3(51, 0, 51);
            //TerrainManager.GetInstance().LoadSlicingMapTileAsync("terrain", pos);

            var getter = ResHelper.LoadGameObject("prefabs/nine_trunk_test");
            var tester = getter.Get();
            Logger.LogWarp.Log(tester.transform.rotation);
            tester.transform.position = new Vector3(26, 0, 26);
            tester.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public void OnDrawGizmos()
        {
            //int Num = 200 / 8;
            int Num = 8;
            int Num2 = 25;
            for (int i = 0; i <= Num; i++)
            {
                Gizmos.color = Color.blue;
                Vector3 begin = new Vector3(0, 0, 25 * i);
                Vector3 end = new Vector3(200, 0, 25 * i);
                Gizmos.DrawLine(begin, end);
            }

            for (int i = 0; i <= Num; i++)
            {
                Gizmos.color = Color.blue;
                Vector3 begin = new Vector3(25 * i, 0, 0);
                Vector3 end = new Vector3(25 * i, 0, 200);
                Gizmos.DrawLine(begin, end);
            }
        }
    }

    public class Main : AMain
    {
        protected override IGameApp CreateGameApp()
        {
            return new GameApp();
        }
    }
}

