using System;
using System.Text.RegularExpressions;

namespace GitCandy.Base
{
    public static class RegularExpression
    {
        public const String Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const String Username = @"(?i)^[a-z][a-z0-9\-_]+$";
        public const String Teamname = @"(?i)^[a-z][a-z0-9\-_]+$";
        public const String Repositoryname = @"(?i)^[a-z][a-z0-9\-\._]+(?<!\.git)$";

        public static readonly Regex ReplaceNewline = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
    }
}
