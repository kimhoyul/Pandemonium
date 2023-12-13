using BestHTTP.JSON.LitJson;
using System;

namespace TOONIPLAY
{
    public static class JsonDataExtension
    {
        public static string GetStringValue(this JsonData jsonData, string key, string defaultValue = "") => jsonData.ContainsKey(key) ? (jsonData[key] != null ? (jsonData[key].IsString ? (string)jsonData[key] : defaultValue) : defaultValue) : defaultValue;

        public static long GetLongValue(this JsonData jsonData, string key, long defaultValue = 0) => jsonData.ContainsKey(key) ? (jsonData[key] != null ? (jsonData[key].IsLong ? (int)jsonData[key] : defaultValue) : defaultValue) : defaultValue;

        public static int GetIntValue(this JsonData jsonData, string key, int defaultValue = 0) => jsonData.ContainsKey(key) ? (jsonData[key] != null ? (jsonData[key].IsInt ? (int)jsonData[key] : defaultValue) : defaultValue) : defaultValue;

        public static bool GetBoolValue(this JsonData jsonData, string key, bool defaultValue = false) => jsonData.ContainsKey(key) ? (jsonData[key] != null ? (jsonData[key].IsBoolean ? (bool)jsonData[key] : defaultValue) : defaultValue) : defaultValue;

        public static UID GetUIDValue(this JsonData jsonData, string key, UID defaultValue = default) => jsonData.ContainsKey(key) ? (jsonData[key] != null ? (jsonData[key].IsString ? new UID((string)jsonData[key]) : defaultValue) : defaultValue) : defaultValue;

        public static DateTime GetDateTimeValue(this JsonData jsonData, string key, DateTime defaultValue = default) => jsonData.ContainsKey(key) && jsonData[key] != null && jsonData[key].IsString ? DateTime.Parse((string)jsonData[key]) : defaultValue;
    }
}
