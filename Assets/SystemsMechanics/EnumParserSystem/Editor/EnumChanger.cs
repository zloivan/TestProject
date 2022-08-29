using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
#if UNITY_EDITOR
namespace EnumParser
{
    public class EnumChanger : OdinMenuEditorWindow
    {

        [MenuItem("Tools/UpdateEmum")]
        private static void OpenWindow()
        {
            GetWindow<EnumChanger>().Show();
        }
        
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Add("AddNewSound", new AddNewSound());

            return tree;
        }
    }

    class AddNewSound
    {
        [Button]
        public void SomeMethod()
        {
            StreamReader reader = new StreamReader("Assets/EnumParser/TestEnum.cs");

            // //mylogs Probably remove this later
            // if (Debug.isDebugBuild) Debug.Log($"<color=purple>reader {reader.ReadToEnd()}</color>");

            var file = reader.ReadToEnd();
            var indexesOfOpenBraces = file.LastIndexOf('{');
            
            
            var splited = file.Split('\n');

            foreach (var s in splited)
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"<color=red>{s}</color>");

            }
            
            
            var dd = file.Remove(0,indexesOfOpenBraces + 1);
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=green>{dd}</color>");
            
            var indexesOfCloseBraces = dd.IndexOf('}');
            dd = dd.Remove(indexesOfCloseBraces);
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=green>{dd}</color>");
            
            var newStrings = dd.Split(',');


            for (int i = 0; i < newStrings.Length; i++)
            {
                newStrings[i] =  newStrings[i].Trim();
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"<color=purple>{newStrings[i]}</color>");

            }
        }
    }
}
#endif 