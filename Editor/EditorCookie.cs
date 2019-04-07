using System.Collections.Generic;
using System.Text;
using SmallJson;
using System.IO;
using System;

namespace AssetBundleBrowser
{
    static class Cookie
    {
        public static string GetValue(string key)
        {
            if(mCookie.ContainsKey(key))
            {
                return mCookie[key];
            }

            return string.Empty;
        }

        public static void SetValue(string key,string value)
        {
            if(null == value)
            {
                mCookie.Remove(key);
            }
            else
            {
                mCookie[key] = value;
            }

            SaveCookie();
        }

        static Dictionary<string, string> LoadCookie()
        {
            string content = string.Empty;

            if(File.Exists(CookieName))
            {
                content = File.ReadAllText(CookieName, Encoding.UTF8);
            }

            if(!string.IsNullOrEmpty(content))
            {
                Dictionary<string, string> temp = JSerializer.Deserialize<Dictionary<string, string>>(content);

                if (null != temp)
                {
                    return temp;
                }
            }
            
            return new Dictionary<string, string>();
        }

        static void SaveCookie()
        {
            File.WriteAllText(CookieName, JSerializer.SerializeToString(mCookie),Encoding.UTF8);
        }

        public static Dictionary<string, string> GetDictionaryValue(string prefix)
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in mCookie)
            {
                string sValue;
                if (kv.Key.Contains(prefix) && !tmp.TryGetValue(kv.Key, out sValue))
                {
                    tmp.Add(kv.Key.Replace(prefix, ""), kv.Value);
                }
            }

            return tmp;
        }

        const string CookieName = "ResDependencies.txt";

        static Dictionary<string, string> mCookie = LoadCookie();
    }
}
