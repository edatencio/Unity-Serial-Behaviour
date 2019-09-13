using UnityEngine;
using NaughtyAttributes;
using System;
using System.IO;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New SaveToFile Asset", menuName = "SaveToFile Asset")]
public class SaveToFile : ScriptableObject
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     [SerializeField, BoxGroup("Settings")] private string filePath;
     [SerializeField, BoxGroup("Debug")] private bool verboseWrite;
     [SerializeField, BoxGroup("Debug")] private bool verboseRead;

     private Dictionary<string, string> _objects;

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     private Dictionary<string, string> Objects
     {
          get
          {
               if (_objects == null)
               {
                    _objects = new Dictionary<string, string>();
                    Load();
               }

               return _objects;
          }

          set { _objects = value; }
     }

     private string Path
     {
          get
          {
               string path = filePath;

               if (path.Contains("ProductName"))
                    path = path.Replace("ProductName", Application.productName);
               if (path.Contains("ProductPath"))
                    path = path.Replace("ProductPath", System.IO.Path.GetFullPath("."));

               return path;
          }
     }

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public bool Save()
     {
          bool done = true;

          try
          {
               Directory.CreateDirectory(Path.Substring(0, Path.LastIndexOf('\\')));

               using (StreamWriter writer = new StreamWriter(Path))
               {
                    string line;

                    foreach (string key in Objects.Keys)
                    {
                         Objects.TryGetValue(key, out line);
                         writer.WriteLine(string.Concat(key, '=', line));

                         if (verboseWrite)
                              Log.Message(string.Concat(name, ": Written Key = ", key, "\t| Value = ", line));
                    }
               }
          }
          catch (Exception e)
          {
               done = false;
               Log.Warning(string.Concat(name, ": > Error saving.\n", e.ToString()));
          }

          return done;
     }

     private bool Load()
     {
          bool done = true;

          try
          {
               using (StreamReader reader = new StreamReader(Path))
               {
                    Objects.Clear();

                    string line, key, value;

                    while (true)
                    {
                         line = reader.ReadLine();

                         if (string.IsNullOrEmpty(line))
                              break;

                         key = line.Substring(0, line.IndexOf('='));
                         value = line.Substring(line.IndexOf('=') + 1);
                         Objects.Add(key, value);

                         if (verboseRead)
                              Log.Message(string.Concat(name, ": Loaded Key = ", key, "\t| Value = ", value));
                    }
               }
          }
          catch (Exception e)
          {
               done = false;
               Log.Warning(string.Concat(name, ": > Error loading.\n", e.ToString()));
          }

          return done;
     }

     public void SetValue<T>(string key, T value, bool save = true)
     {
          if (Objects.ContainsKey(key))
               Objects.Remove(key);

          Objects.Add(key, value.ToString());

          if (save)
               Save();
     }

     public string GetValue(string key)
     {
          if (Objects.ContainsKey(key))
          {
               string value;
               Objects.TryGetValue(key, out value);
               return value;
          }
          else
          {
               return null;
          }
     }

     public bool TryGetValue(string key, out string value)
     {
          return Objects.TryGetValue(key, out value);
     }

     [Button("Log Path")]
     private void LogPath()
     {
          Log.Message(string.Concat(name, ": Save path = ", Path));
     }

     [Button("Log Values")]
     private void LogValues()
     {
          if (Objects.Count > 0)
          {
               foreach (var obj in Objects)
                    Log.Message(name, "Key = ", obj.Key, "\t| Value = ", obj.Value);
          }
          else
          {
               Log.Message(name, "There are no values loaded.");
          }
     }
}
