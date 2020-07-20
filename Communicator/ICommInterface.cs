namespace Communicator
{
    public enum CommStatusEn
    {
        Connected,
        Disconnected
    }

    public delegate void ReceiveDlg(byte[] pBuffer);

    public delegate void CommStatusDlg(CommStatusEn pCommStatus, string pCommId);

    public interface ICommInterface
    {
        void Send(byte[] pBuffer);
        void Close();
        event ReceiveDlg ReceiveEvent;
        event CommStatusDlg CommStatusEvent;
    }
}
