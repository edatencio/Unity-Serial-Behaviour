public interface ISerialReceiver
{
    string name { get; }

    void OnSerialMessageReceived(string msg);
}
