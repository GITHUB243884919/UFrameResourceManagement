using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UFrame;
//using UFrame.FSM;
//using UFrame.MessageCenter

namespace Game
{
    /// <summary>
    /// 游戏总入口
    /// 如果成员变量希望跨场景访问，定义成static, 或者是一个单件
    /// </summary>
    public class GameApp : IGameApp 
    {
        //FSMMachine gameFSMMachine = new FSMMachine();
        
        public void Awake()
        {
            //Logger.LogWarp.Log("GameApp Awake");
            //FSMState stateUpdate = new StateUpdate("Update", gameFSMMachine);
            //FSMState stateLogin = new StateLogin("Login", gameFSMMachine);
            //FSMState stateHome = new StateHome("Home", gameFSMMachine);

            //gameFSMMachine.AddState(stateUpdate);
            //gameFSMMachine.AddState(stateLogin);
            //gameFSMMachine.AddState(stateHome);
            //gameFSMMachine.SetDefaultState("Update");

            //ResHelper.LoadScene("scenes/scene_2");
            //var getter = ResHelper.LoadGameObject("prefabs/cube");
            //GameObject go = getter.Get();
            //Logger.LogWarp.Log(go.name);




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
            LoadScene_1();
        }

        void LoadScene_1()
        {
            //string scenePath = "GameResources/scenes/scene_2";
            //UnityEngine.SceneManagement.SceneManager.LoadScene(scenePath);
            //UnityEngine.SceneManagement.SceneManager.sceneLoaded +=
            //    (a, b) =>
            //    {
            //        Debug.Log(a.name);
            //        string loadPath = "Assets/GameResources/prefabs/cube.prefab";
            //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(loadPath);
            //        GameObject go = GameObject.Instantiate<GameObject>(prefab);
            //        Debug.Log(go.name);
            //    };

            //ResHelper.LoadScene("scenes/scene_2");
            //UnityEngine.SceneManagement.SceneManager.sceneLoaded +=
            //    (a, b) =>
            //    {
            //        var getter = ResHelper.LoadGameObject("prefabs/cube");
            //        GameObject go = getter.Get();
            //        Logger.LogWarp.Log(go.name);
            //    };

            UFrame.ResourceManagement.SceneManagement.GetInstance().LoadScene("scenes/scene_2", () =>
            {
                var getter = ResHelper.LoadGameObject("prefabs/cube");
                GameObject go = getter.Get();
                Logger.LogWarp.Log(go.name);
            });

        }


        public void Update(float s)
        {
            //MessageManager.GetInstance().Tick();
            //gameFSMMachine.Tick((int)(s * 1000));
            
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

