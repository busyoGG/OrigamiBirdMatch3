using UnityEngine;

namespace ReflectionUI
{
    public class UGUIUtils
    {
        public static T GetUI<T>(UGUIData comp, string path) where T : class
        {
            string[] paths = path.Split('/');
            UGUIData res = null;
            UGUIData parent = comp;
            foreach (string s in paths)
            {
                if (s == "") continue;
                int output;
                bool isNumeric = int.TryParse(s, out output);
                if (isNumeric)
                {
                    res = parent.GetChildAt(output);
                }
                else
                {
                    res = parent.GetChild(s);
                }

                if (res == null)
                {
                    Debug.LogError("ui路径错误" + path);
                    return null;
                }
                
                parent = res;
            }

            return res as T;
        }
    }
}