using UnityEngine;
using NaughtyAttributes;
using System.IO.Ports;
using System;

public class SerialBehaviour : MonoBehaviour
{
    /*************************************************************************************************
    *** Variables
    *************************************************************************************************/
    [SerializeField, BoxGroup("Settings")] private string port;
    [SerializeField, BoxGroup("Settings")] private int baudRate = 115200;
    [SerializeField, BoxGroup("Settings")] private bool singleton;
    [SerializeField, BoxGroup("Settings")] private SerialAsset serialAsset;
    [SerializeField, BoxGroup("Settings")] private SaveToFile serialSave;
    [SerializeField, BoxGroup("Settings")] private SerialMonitor serialMonitor;

    [SerializeField, BoxGroup("Debug")] private bool verboseRead;
    [SerializeField, BoxGroup("Debug")] private bool verboseUnicodeRead;
    [SerializeField, BoxGroup("Debug")] private bool verboseWrite;

    [SerializeField, BoxGroup("Send Message")] private string debugMessage;

    private SerialPort serialPort;
    private Timer timer;
    private string message = "";

    private const string portKey = "Port";
    private const string baudRateKey = "Baud Rate";
    private const string parityKey = "Parity";
    private const string dataBitsKey = "Data Bits";
    private const string stopBitsKey = "Stop Bits";
    private const float delayBetweenMessages = 0.05f;

    /*************************************************************************************************
    *** OnEnable
    *************************************************************************************************/
    private void OnEnable()
    {
        if (singleton)
        {
            if (FindObjectOfType<SerialBehaviour>() != this)
            {
                Log.Error(name, "There's more than one SerialBehaviour loaded");
                gameObject.SetActive(false);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        if (serialPort == null)
        {
            try
            {
                if (Application.isEditor)
                    serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
                else if (!LoadSerialConf())
                    SaveSerialConf();

                serialPort.Open();
                serialPort.ReadTimeout = 1;
            }
            catch (Exception e)
            {
                Log.Error(name, "> Error creating serial port:\n", e.ToString());
            }
        }

        timer = new Timer();
        timer.Start();
        serialAsset.ClearSendBuffer();
    }

    /*************************************************************************************************
    *** Update
    *************************************************************************************************/
    private void Update()
    {
        Read();
        Write();
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
    private void Write()
    {
        if (serialAsset.SendBufferCount <= 0 || timer.ElapsedSeconds < delayBetweenMessages)
            return;

        timer.Start();

        string msg = "";
        string[] serialBuffer = serialAsset.GetSendBuffer(true);
        for (int i = 0; i < serialBuffer.Length; i++)
            msg += serialBuffer[i] + ';';

        try
        {
            serialPort.Write(msg);
            serialMonitor.Add("<- ", msg);

            if (verboseWrite)
                Log.Message(name, ": <- Sended Message = ", msg);
        }
        catch (Exception e)
        {
            Log.Error(name, "> Error writing to serial port:\n", e);
        }
    }

    private void Read()
    {
        try
        {
            // This is not an infinite loop because when the serial port times out, it throws a
            // TimeoutException exception
            char chr;
            while (true)
            {
                chr = (char)serialPort.ReadByte();
                if (chr == 255) break;
                message += chr;
            }
        }
        catch (TimeoutException) { }

        if (!string.IsNullOrEmpty(message) && message.IndexOf(';') >= 0)
        {
            try
            {
                int messageCount = message.Length - message.Replace(";", "").Length;
                int messagesLength = 0;
                string msg;
                for (int index = 0, prevIndex = 0, i = 0; i < messageCount; i++)
                {
                    index = message.IndexOf(';', prevIndex);
                    msg = message.Substring(prevIndex, index - prevIndex);
                    prevIndex = index + 1;

                    messagesLength += msg.Length + 1; // Including the ';' character

                    serialAsset.MessageReceived(msg);
                    serialMonitor.Add("-> ", msg);

                    // Debug
                    if (verboseRead)
                        Log.Message(name, "-> Recieved Message = ", msg);

                    if (verboseUnicodeRead)
                    {
                        int unicode = 0;
                        foreach (char c in msg)
                            unicode += c;

                        Log.Message(name, "-> Received Total Unicode = ", msg);
                    }
                }

                message = message.Substring(messagesLength);
            }
            catch (Exception e)
            {
                Log.Error(name, "> Error reading from serial port:\n", e);
            }
        }
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
        serialAsset.Send(debugMessage);
    }
}
