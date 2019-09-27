using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSerialAsset", menuName = "Serial Asset")]
public class SerialAsset : ScriptableObject
{
    /*************************************************************************************************
    *** Variables
    *************************************************************************************************/
    [SerializeField, BoxGroup] private bool verboseRegistration;

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
            receivers[i].OnSerialMessageReceived(message);
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

    [Button("Log Registered Listeners")]
    private void LogRegisteredListeners()
    {
        for (int i = 0; i < receivers.Count; i++)
            Log.Message(name, "Listener [", i++, "] = ", receivers[i].name);
    }
}
