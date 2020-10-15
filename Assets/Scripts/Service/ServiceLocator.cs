using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Service
{
    public class ServiceLocator<T> where T : class
    {
        public static T Instance { get; private set; }

        public static void Bind(T instance)
        {
            Instance = instance;
        }

        public static void UnBind(T instance)
        {
            if (Instance == instance)
            {
                Instance = null;
            }
        }
    }
}
