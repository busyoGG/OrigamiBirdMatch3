using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FileUtils
{
    // Start is called before the first frame update
    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    public static void WriteFile(string path, string content)
    {
        //if (File.Exists(path)) { 
        //}
        string folderPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(path, content, Encoding.Default);
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadFile(string path)
    {
        path = path.Replace("/", "\\");
        return File.ReadAllText(path);
    }

    /// <summary>
    /// 获取文件夹下所有文件的路径
    /// </summary>
    /// <param name="folderUrl"></param>
    /// <returns></returns>
    public static string[] GetFolderFiles(string folderUrl)
    {
#if UNITY_EDITOR
        string url = Application.dataPath + "/" + folderUrl;
#else
        string url = folderUrl;
#endif
        if (folderUrl != null)
        {
            string[] res = Directory.GetFiles(url, "*.*", SearchOption.AllDirectories);
            res = res.Where(file => !file.EndsWith(".meta")).ToArray();
            for(int i = 0 ; i < res.Length; i++)
            {
                res[i] = AbsolutePathToRelative(res[i]);
            }
            return res;
        }
        else
        {
            Debug.LogWarning(folderUrl + "文件夹不存在");
            return null;
        }
    }

    // 将绝对路径转换为相对路径
    private static string AbsolutePathToRelative(string absolutePath)
    {
#if UNITY_EDITOR
        return "Assets" + Path.GetFullPath(absolutePath).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
#else
        return Path.GetFullPath(absolutePath).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/').Substring(1);
#endif
    }
}
