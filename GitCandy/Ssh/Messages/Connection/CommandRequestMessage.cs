using System;
using System.Text;

namespace GitCandy.Ssh.Messages.Connection
{
    public class CommandRequestMessage : ChannelRequestMessage
    {
        public String Command { get; private set; }

        protected override void OnLoad(SshDataWorker reader)
        {
            base.OnLoad(reader);

            Command = reader.ReadString(Encoding.ASCII);
        }
    }
}
