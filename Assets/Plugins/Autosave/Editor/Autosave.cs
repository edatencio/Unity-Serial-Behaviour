//Source: https://forum.unity3d.com/threads/we-need-auto-save-feature.483853/

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class Autosave
{
     static Autosave()
     {
          EditorApplication.playModeStateChanged += (PlayModeStateChange) =>
          {
               if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
               {
                    Debug.Log("Auto-saving all open scenes...");
                    EditorSceneManager.SaveOpenScenes();
                    AssetDatabase.SaveAssets();

               }

          };

     }

}
