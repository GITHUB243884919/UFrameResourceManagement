using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UFrame;
//using UFrame.FSM;
//using UFrame.MessageCenter
using UFrame.ResourceManagement;
using UnityEngine.U2D;

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
            TestCase_Scene_01.LoadScene();
            //TestCase_01.Test_Vector2_Bit();
            //TestCase_01.Test_Async();
            //TestCase_01.Test_Sync();
            //TestCase_01.Test_Shader();
            //TestCase_01.Test_Sprite();
        }




        public void Update(float s)
        {
                        
        }



        public void OnDrawGizmos()
        {
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

