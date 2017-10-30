
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TimeLine
{
    public class CameraEffectTypes : ScriptableObject
    {
        private static List<Type> effectTypeList;

        private static List<string> typeNameList;

        public static bool InitData = false;

        public static List<Type> EffectTypeList
        {
            get
            {
                if (effectTypeList == null)
                    effectTypeList = new List<Type>();
                return effectTypeList;
            }

            set
            {
                effectTypeList = value;
            }
        }

        public static List<string> TypeNameList
        {
            get
            {
                if (typeNameList == null)
                    typeNameList = new List<string>();
                return typeNameList;
            }

            set
            {
                typeNameList = value;
            }
        }

        public static void Clean()
        {
            EffectTypeList.Clear();
            TypeNameList.Clear();
        }

        public static string[] getEnums()
        {
            return TypeNameList.ToArray();
        }

        public static int getIndex(Type t)
        {
            if (t == null)
                return -1;

            return EffectTypeList.IndexOf(t);
        }
        public static int getIndex(string t)
        {
            if (t == null)
                return -1;

            return TypeNameList.IndexOf(t);
        }

        public static Type getType(int index)
        {
            if (index < 0 || index >= EffectTypeList.Count)
                return null;
            return EffectTypeList[index];
        }
        public static string getName(int index)
        {
            if (index < 0 || index >= TypeNameList.Count)
                return null;
            return TypeNameList[index];
        }
        public static void AddType(Type t)
        {
            if (!EffectTypeList.Contains(t))
            {
                EffectTypeList.Add(t);
                string filePath = t.ToString().Replace(@"_", @"/");
                TypeNameList.Add(filePath);
            }
        }
    }
}