using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSerialAsset", menuName = "Serial Asset")]
public class SerialAsset : ScriptableObject
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     private List<string> read = new List<string>();
     private List<string> write = new List<string>();

     /*************************************************************************************************
     *** OnEnable
     *************************************************************************************************/
     private void OnEnable()
     {
     }

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     public List<string> Read
     {
          get { return read; }
     }

     public List<string> Write
     {
          get { return write; }
     }

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void ClearRead()
     {
          Read.Clear();
     }

     public void ClearWrite()
     {
          Write.Clear();
     }

     public void Clear()
     {
          ClearRead();
          ClearWrite();
     }
}
