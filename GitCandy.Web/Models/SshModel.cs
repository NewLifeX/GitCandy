
using System;

namespace GitCandy.Models
{
    public class SshModel
    {
        public String Username { get; set; }
        public SshKey[] SshKeys { get; set; }

        public class SshKey
        {
            public String Name { get; set; }
        }
    }
}
