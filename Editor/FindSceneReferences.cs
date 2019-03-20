using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindSceneReferences
{

    [MenuItem("Assets/Find References Scene", false, 100)]

    static private void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string dataPath = Application.dataPath + "/Resources/#Scene#scenes";

        if (!string.IsNullOrEmpty(path))
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];

                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(file), typeof(Object)));
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("#Scene#scenes匹配结束");
                    dataPath = Application.dataPath + "/Resources/_levels";

                    if (!string.IsNullOrEmpty(path))
                    {
                        guid = AssetDatabase.AssetPathToGUID(path);

                        files = Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories)
                            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                        startIndex = 0;

                        EditorApplication.update = delegate ()
                        {
                            file = files[startIndex];

                            isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                            if (Regex.IsMatch(File.ReadAllText(file), guid))
                            {
                                Debug.Log(file, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(file), typeof(Object)));
                            }

                            startIndex++;
                            if (isCancel || startIndex >= files.Length)
                            {
                                EditorUtility.ClearProgressBar();
                                EditorApplication.update = null;
                                startIndex = 0;
                                Debug.Log("_levels匹配结束");
                            }

                        };
                    }
                }

            };
        }
    }

    [MenuItem("Assets/Find References Scene", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}