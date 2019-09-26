using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public interface ISerialReceiver
{
    string name { get; }

    void OnMessageReceived(string msg);
}

[CreateAssetMenu(fileName = "NewSerialAsset", menuName = "Serial Asset")]
public class SerialAsset : ScriptableObject
{
    /*************************************************************************************************
    *** Variables
    *************************************************************************************************/
    [SerializeField, BoxGroup] private bool verboseRegistration;
    [SerializeField, BoxGroup] private string test;

    private List<ISerialReceiver> receivers = new List<ISerialReceiver>();
    private List<string> sendBuffer = new List<string>();

    /*************************************************************************************************
    *** OnEnable
    *************************************************************************************************/
    private void OnEnable()
    {
        receivers.Clear();
        sendBuffer.Clear();
    }

    /*************************************************************************************************
    *** Properties
    *************************************************************************************************/
    public int SendBufferCount { get { return sendBuffer.Count; } }

    /*************************************************************************************************
    *** Methods
    *************************************************************************************************/
    public void RegisterReceiver(ISerialReceiver serialReceiver)
    {
        if (verboseRegistration)
        {
            if (!receivers.Contains(serialReceiver))
                Debug.Log(string.Concat(name, "Registered = ", serialReceiver.name));
            else
                Debug.LogWarning(string.Concat(name, serialReceiver.name, " is already registered"));
        }

        if (!receivers.Contains(serialReceiver))
            receivers.Add(serialReceiver);
    }

    public void MessageReceived(string message)
    {
        for (int i = 0; i < receivers.Count; i++)
            receivers[i].OnMessageReceived(message);
    }

    public void Send(string message)
    {
        sendBuffer.Add(message);
    }

    public string[] GetSendBuffer(bool andClear = false)
    {
        string[] messages = sendBuffer.ToArray();

        if (andClear)
            sendBuffer.Clear();

        return messages;
    }

    public void ClearSendBuffer()
    {
        sendBuffer.Clear();
    }

    [Button("Invoke: Test")]
    private void InvokeTest()
    {
        MessageReceived(test);
        test = "";
    }

    [Button("Log Registered Actions")]
    private void LogRegisteredActions()
    {
        for (int i = 0; i < receivers.Count; i++)
            Log.Message(name, "Listener [", i++, "] = ", receivers[i].name);
    }
}
