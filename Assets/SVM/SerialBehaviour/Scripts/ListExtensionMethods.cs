using System.Collections.Generic;

public static class ListExtensionMethods
{
     public static void Add(this List<string> list, params string[] item)
     {
          string str = null;
          for (int i = 0; i < item.Length; i++)
               str += item[i];

          list.Add(str);
     }

     public static void Add(this List<string> list, params object[] item)
     {
          string str = null;
          for (int i = 0; i < item.Length; i++)
               str += item[i].ToString();

          list.Add(str);
     }

     public static string Get(this List<string> list, char firstCharacter)
     {
          string value = null;

          if (list.Count > 0)
          {
               for (int i = 0; i < list.Count; i++)
               {
                    if (!string.IsNullOrEmpty(list[i]) && list[i][0] == firstCharacter)
                    {
                         value = list[i];
                         list.RemoveAt(i);
                    }
               }
          }

          return value;
     }

     public static string Get(this List<string> list, string contains)
     {
          string value = null;

          if (list.Count > 0)
          {
               for (int i = 0; i < list.Count; i++)
               {
                    if (list[i].Contains(contains))
                    {
                         value = list[i];
                         list.RemoveAt(i);
                    }
               }
          }

          return value;
     }

     public static string Get(this List<string> list, int index)
     {
          string value = null;

          if (index > 0 && index < list.Count)
          {
               value = list[index];
               list.RemoveAt(index);
          }

          return value;
     }

     public static string[] Get(this List<string> list)
     {
          string[] value = null;

          if (list.Count > 0)
          {
               value = list.ToArray();
               list.Clear();
          }

          return value;
     }

     public static bool ContainsAndRemove(this List<string> list, string contains)
     {
          if (list.Count > 0 && list.Contains(contains))
          {
               list.Remove(contains);
               return true;
          }

          return false;
     }
}
