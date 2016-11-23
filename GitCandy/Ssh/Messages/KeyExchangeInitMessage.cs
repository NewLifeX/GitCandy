using System;
using System.Security.Cryptography;
using System.Text;

namespace GitCandy.Ssh.Messages
{
    [Message("SSH_MSG_KEXINIT", MessageNumber)]
    public class KeyExchangeInitMessage : Message
    {
        private const byte MessageNumber = 20;

        private static readonly RandomNumberGenerator _rng = new RNGCryptoServiceProvider();

        public KeyExchangeInitMessage()
        {
            Cookie = new byte[16];
            _rng.GetBytes(Cookie);
        }

        public byte[] Cookie { get; private set; }

        public String[] KeyExchangeAlgorithms { get; set; }

        public String[] ServerHostKeyAlgorithms { get; set; }

        public String[] EncryptionAlgorithmsClientToServer { get; set; }

        public String[] EncryptionAlgorithmsServerToClient { get; set; }

        public String[] MacAlgorithmsClientToServer { get; set; }

        public String[] MacAlgorithmsServerToClient { get; set; }

        public String[] CompressionAlgorithmsClientToServer { get; set; }

        public String[] CompressionAlgorithmsServerToClient { get; set; }

        public String[] LanguagesClientToServer { get; set; }

        public String[] LanguagesServerToClient { get; set; }

        public bool FirstKexPacketFollows { get; set; }

        public uint Reserved { get; set; }

        public override byte MessageType { get { return MessageNumber; } }

        protected override void OnLoad(SshDataWorker reader)
        {
            Cookie = reader.ReadBinary(16);
            KeyExchangeAlgorithms = reader.ReadString(Encoding.ASCII).Split(',');
            ServerHostKeyAlgorithms = reader.ReadString(Encoding.ASCII).Split(',');
            EncryptionAlgorithmsClientToServer = reader.ReadString(Encoding.ASCII).Split(',');
            EncryptionAlgorithmsServerToClient = reader.ReadString(Encoding.ASCII).Split(',');
            MacAlgorithmsClientToServer = reader.ReadString(Encoding.ASCII).Split(',');
            MacAlgorithmsServerToClient = reader.ReadString(Encoding.ASCII).Split(',');
            CompressionAlgorithmsClientToServer = reader.ReadString(Encoding.ASCII).Split(',');
            CompressionAlgorithmsServerToClient = reader.ReadString(Encoding.ASCII).Split(',');
            LanguagesClientToServer = reader.ReadString(Encoding.ASCII).Split(',');
            LanguagesServerToClient = reader.ReadString(Encoding.ASCII).Split(',');
            FirstKexPacketFollows = reader.ReadBoolean();
            Reserved = reader.ReadUInt32();
        }

        protected override void OnGetPacket(SshDataWorker writer)
        {
            writer.Write(Cookie);
            writer.Write(String.Join(",", KeyExchangeAlgorithms), Encoding.ASCII);
            writer.Write(String.Join(",", ServerHostKeyAlgorithms), Encoding.ASCII);
            writer.Write(String.Join(",", EncryptionAlgorithmsClientToServer), Encoding.ASCII);
            writer.Write(String.Join(",", EncryptionAlgorithmsServerToClient), Encoding.ASCII);
            writer.Write(String.Join(",", MacAlgorithmsClientToServer), Encoding.ASCII);
            writer.Write(String.Join(",", MacAlgorithmsServerToClient), Encoding.ASCII);
            writer.Write(String.Join(",", CompressionAlgorithmsClientToServer), Encoding.ASCII);
            writer.Write(String.Join(",", CompressionAlgorithmsServerToClient), Encoding.ASCII);
            writer.Write(String.Join(",", LanguagesClientToServer), Encoding.ASCII);
            writer.Write(String.Join(",", LanguagesServerToClient), Encoding.ASCII);
            writer.Write(FirstKexPacketFollows);
            writer.Write(Reserved);
        }
    }
}
