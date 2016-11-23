using System;
using System.Diagnostics.Contracts;

namespace GitCandy.Ssh.Services
{
    public class UserauthArgs
    {
        public UserauthArgs(String keyAlgorithm, String fingerprint, byte[] key)
        {
            Contract.Requires(keyAlgorithm != null);
            Contract.Requires(fingerprint != null);
            Contract.Requires(key != null);

            KeyAlgorithm = keyAlgorithm;
            Fingerprint = fingerprint;
            Key = key;
        }

        public String KeyAlgorithm { get; private set; }
        public String Fingerprint { get; private set; }
        public byte[] Key { get; private set; }
        public bool Result { get; set; }
    }
}
