﻿namespace UFrame.Common
{
    public interface ISingleton
    {
        void Init();
    }

    public class Singleton<T> where T : ISingleton, new()
    {
        static public T GetInstance()
        {
            if (null == s_Instance)
            {
                s_Instance = new T();
                s_Instance.Init();
            }
            return s_Instance;

        }

        static public void DestroyInstance()
        {
            s_Instance = default(T);
        }

        protected Singleton() { }

        private static T s_Instance = default(T);
    }


    public class SingletonMono<T> : UnityEngine.MonoBehaviour
        where T : UnityEngine.MonoBehaviour
    {
        static public T GetInstance()
        {
            return s_Instance;

        }

        static public void DestroyInstance()
        {
            s_Instance = default(T);
        }

        public virtual void Awake()
        {
            s_Instance = this as T;
        }

        public virtual void OnDestroy()
        {
            DestroyInstance();
        }

        protected SingletonMono() { }

        private static T s_Instance = default(T);
    }
}

