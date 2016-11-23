using System;
using System.Diagnostics.Contracts;

namespace GitCandy.Ssh.Messages
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public MessageAttribute(String name, byte number)
        {
            Contract.Requires(name != null);

            Name = name;
            Number = number;
        }

        public String Name { get; private set; }
        public byte Number { get; private set; }
    }
}
