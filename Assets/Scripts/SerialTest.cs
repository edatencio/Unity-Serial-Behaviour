using UnityEngine;
using UnityEngine.UI;

public class SerialTest : MonoBehaviour, ISerialReceiver
{
    [SerializeField] private SerialAsset serialAsset;
    [SerializeField] private Text text;
    [SerializeField] private float sendDelay = 1f;
    [SerializeField] private bool verboseMessageReceived;

    private Timer timer;

    private void Start()
    {
        timer = new Timer();
        timer.Start();
        serialAsset.RegisterReceiver(this);
    }

    private void Update()
    {
        if (timer.ElapsedSeconds >= sendDelay)
        {
            timer.Start();
            Send();
        }
    }

    private void Send()
    {
        serialAsset.Send("test indeed");
    }

    public void OnMessageReceived(string msg)
    {
        if (verboseMessageReceived)
            Debug.Log("Received " + msg);

        Text = msg;
    }

    private string Text
    {
        get { return text.text; }

        set
        {
            if (text.text != value)
                text.text = value;
        }
    }
}
