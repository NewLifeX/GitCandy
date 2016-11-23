using System;

namespace GitCandy.Ssh
{
    public class SshConnectionException : Exception
    {
        public SshConnectionException()
        {
        }

        public SshConnectionException(String message, DisconnectReason disconnectReason = DisconnectReason.None)
            : base(message)
        {
            DisconnectReason = disconnectReason;
        }

        public DisconnectReason DisconnectReason { get; private set; }

        public override String ToString()
        {
            return String.Format("SSH connection disconnected bacause {0}: {1}");
        }
    }
}
