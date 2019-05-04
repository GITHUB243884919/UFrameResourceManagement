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
            LoadScene();
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
    }

    public class Main : AMain
    {
        protected override IGameApp CreateGameApp()
        {
            return new GameApp();
        }
    }
}

