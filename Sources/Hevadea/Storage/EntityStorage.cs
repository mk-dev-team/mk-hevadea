﻿using System;
using System.Collections.Generic;
using Hevadea.Framework.Networking;

namespace Hevadea.Storage
{
    public class EntityStorage
    {
        public string Type { get; set; }
        public int Ueid { get; set; } = -1;
        
        public Dictionary<string, object> Data { get; set; }

        public EntityStorage(string type)
        {
            Type = type;
            Data = new Dictionary<string, object>();
        }
        
        public EntityStorage(string type, Dictionary<string, object> data)
        {
            Type = type;
            Data = data;
        }

        public void SaveToDataBuffer(DataBuffer data)
        {
            
        }
        
        public float GetFloat(string name, float defaultValue = 0f)
        {
            return Convert.ToSingle(Get(name, defaultValue));
        }
        
        public int GetInt(string name, int defaultValue = 0)
        {
            return Convert.ToInt32(Get(name, defaultValue));
        }
        
        public bool GetBool(string name, bool defaultValue = false)
        {
            return Convert.ToBoolean(Get(name, defaultValue));
        }
        
        public string GetString(string name, string defaultValue = "")
        {
            return Convert.ToString(Get(name, defaultValue));
        }
        
        internal object Get<T>(string dataName, T defaultValue)
        {
            return Data.ContainsKey(dataName) ? Data[dataName] : defaultValue;
        }

        public void Set<T>(string dataName, T value)
        {
            Data[dataName] = value;
        }
    }
}