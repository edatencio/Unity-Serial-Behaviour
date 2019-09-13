//To use this class you need to set the "Api Compatibility Level" to ".NET 2.0"
//in Edit -> Project Settings -> Player
using UnityEngine;
using NaughtyAttributes;
using System.IO.Ports;

public class SerialBehaviour : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     [SerializeField, BoxGroup("Settings")] private string port;
     [SerializeField, BoxGroup("Settings")] private int baudRate = 115200;
     [SerializeField, BoxGroup("Settings")] private bool dontDestroyOnLoad;
     [SerializeField, BoxGroup("Settings")] private SerialAsset serialAsset;
     [SerializeField, BoxGroup("Settings")] private SaveToFile serialSave;
     [SerializeField, BoxGroup("Settings")] private SerialMonitor serialMonitor;

     [SerializeField, BoxGroup("Debug")] private bool overrideSaveToFile;
     [SerializeField, BoxGroup("Debug")] private bool verboseRead;
     [SerializeField, BoxGroup("Debug")] private bool verboseUnicodeRead;
     [SerializeField, BoxGroup("Debug")] private bool verboseWrite;

     [SerializeField, BoxGroup("Send Message")] private string debugMessage;

     private static SerialPort serialPort;
     private static readonly float delayBetweenMessages = 0.05f;
     private Timer timer;
     private string message = "";

     private const string portKey = "Port";
     private const string baudRateKey = "Baud Rate";
     private const string parityKey = "Parity";
     private const string dataBitsKey = "Data Bits";
     private const string stopBitsKey = "Stop Bits";

     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     private void Start()
     {
          if (dontDestroyOnLoad)
               DontDestroyOnLoad(gameObject);

          if (FindObjectOfType<SerialBehaviour>() != this)
               Log.Error(name, "There's more than one SerialBehaviour in scene.");

          if (serialPort == null)
          {
               try
               {
#if !UNITY_EDITOR
                    overrideSaveToFile = false;
#endif
                    if (!overrideSaveToFile)
                    {
                         if (!LoadSerialConf())
                              SaveSerialConf();
                    }
                    else
                    {
                         Log.Warning(name, "SaveToFile overwritten, using port: ", port);
                         serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
                    }

                    serialPort.Open();
                    serialPort.ReadTimeout = 1;
               }
               catch (System.Exception e)
               {
                    Log.Error(name, "IOException = ", e.ToString());
               }
          }

          timer = new Timer();
          timer.Start();

          serialAsset.Clear();
     }

     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     private void Update()
     {
          Read();

          // Write
          if (serialAsset.Write.Count > 0 && timer.ElapsedSeconds >= delayBetweenMessages)
          {
               timer.Start();

               string msg = "";
               foreach (string message in serialAsset.Write)
                    msg += message + ';';

               Write(msg);
               serialAsset.ClearWrite();

               if (verboseWrite)
                    Log.Message(name, ": <<< Sended Message = ", msg);
          }
     }

     /*************************************************************************************************
     *** OnDisable
     *************************************************************************************************/
     private void OnDisable()
     {
          serialPort.Close();
     }

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     private void Write(string message)
     {
          try
          {
               serialPort.Write(message);
               serialMonitor.Add("<- ", message);
          }
          catch (System.Exception e)
          {
               Log.Error(name, "> Error writing to serial port:\n", e);
          }
     }

     private void Read()
     {
          try
          {
               byte chr;
               while (true)
               {
                    chr = (byte)serialPort.ReadByte();

                    if (chr == 255)
                         break;

                    message += (char)chr;
               }
          }
          catch { }

          try
          {
               if (!string.IsNullOrEmpty(message) && message.IndexOf(';') >= 0)
               {
                    foreach (string msg in ParseMessages(ref message))
                    {
                         serialAsset.Read.Add(msg);
                         serialMonitor.Add("-> ", msg);

                         // Debug
                         if (verboseRead)
                              Log.Message(name, ">>> Recieved Message = ", msg);

                         if (verboseUnicodeRead)
                         {
                              int unicode = 0;
                              foreach (char c in msg)
                                   unicode += c;

                              Log.Message(name, ">>> Received Total Unicode = ", msg);
                         }
                    }
               }
          }
          catch (System.Exception e)
          {
               Log.Error(name, "> Error reading from serial port:\n", e);
          }
     }

     private static string[] ParseMessages(ref string message)
     {
          int count = 0;

          foreach (char chr in message)
          {
               if (chr == ';')
                    count++;
          }

          string[] msgs = new string[count];

          for (int index, i = 0; i < count; i++)
          {
               index = message.IndexOf(';');
               msgs[i] = message.Substring(0, index);
               message = message.Remove(0, index + 1);
          }

          return msgs;
     }

     private void SaveSerialConf()
     {
          serialSave.SetValue(portKey, port);
          serialSave.SetValue(baudRateKey, baudRate);
          serialSave.SetValue(parityKey, (int)Parity.None);
          serialSave.SetValue(dataBitsKey, "8");
          serialSave.SetValue(stopBitsKey, (int)StopBits.One);
     }

     private bool LoadSerialConf()
     {
          bool done = true;

          serialPort = new SerialPort();
          string value;

          // Port
          if (serialSave.TryGetValue(portKey, out value))
               serialPort.PortName = value;
          else
               done = false;

          // Baud rate
          if (serialSave.TryGetValue(baudRateKey, out value))
               serialPort.BaudRate = int.Parse(value);
          else
               done = false;

          // Parity
          if (serialSave.TryGetValue(parityKey, out value))
               serialPort.Parity = (Parity)int.Parse(value);
          else
               done = false;

          // Data bits
          if (serialSave.TryGetValue(dataBitsKey, out value))
               serialPort.DataBits = int.Parse(value);
          else
               done = false;

          // Stop bits
          if (serialSave.TryGetValue(stopBitsKey, out value))
               serialPort.StopBits = (StopBits)int.Parse(value);
          else
               done = false;

          return done;
     }

     [Button("Send Debug Message")]
     private void SendDebugMessage()
     {
          serialAsset.Write.Add(debugMessage);
     }
}
