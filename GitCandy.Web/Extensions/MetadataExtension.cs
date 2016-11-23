using GitCandy.App_GlobalResources;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace GitCandy.Extensions
{
    public static class MetadataExtension
    {
        const int ShaBytesLength = 20;
        const String HexValuesInUppercase = "0123456789ABCDEF";
        const String HexValuesInLowercase = "0123456789abcdef";

        public static byte[] AggregateSha(this byte[] one, params byte[][] twos)
        {
            Contract.Requires(one == null);
            Contract.Requires(one.Length == ShaBytesLength);

            var val = one.ToArray();
            foreach (var two in twos)
            {
                Contract.Assert(two != null && two.Length == ShaBytesLength);
                for (int i = 0; i < ShaBytesLength; i++)
                {
                    val[i] ^= two[i];
                }
            }

            return val;
        }

        public static String BytesToString(this byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var length = bytes.Length;
            var chars = new char[length * 2];
            for (int i = 0, index = 0; i < length; i++)
            {
                var b = bytes[i];
                chars[index++] = HexValuesInLowercase[b & 0xf];
                chars[index++] = HexValuesInLowercase[b >> 4];
            }

            return new String(chars);
        }

        public static String ToFlagString(this bool flag, String trueStr, String falseStr)
        {
            return flag ? trueStr : falseStr;
        }

        public static Dictionary<String, object> CastToDictionary(this object values)
        {
            if (values == null)
                return null;

            var dictionary = new Dictionary<String, object>(StringComparer.OrdinalIgnoreCase);
            var properties = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                var value = propertyDescriptor.GetValue(values);
                dictionary.Add(propertyDescriptor.Name, value);
            }
            return dictionary;
        }

        public static String ToShortSha(this String sha)
        {
            if (sha == null)
                return null;

            return sha.Length > 7 && sha.All(c => '0' <= c && c <= '9' || 'a' <= c && c <= 'f' || 'A' <= c && c <= 'F')
                ? sha.Substring(0, 7)
                : sha;
        }

        public static IEnumerable<SelectListItem> ToSelectListItem(this IEnumerable<String> items, String selected)
        {
            return items.Select(s => new SelectListItem
            {
                Text = s,
                Selected = s == selected,
            });
        }

        public static String CalcSha(this String str)
        {
            var sha = new SHA1CryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(str);
            data = sha.ComputeHash(data);
            return data.BytesToString();
        }

        public static String RepetitionIfEmpty(this String str, String repetition)
        {
            return String.IsNullOrWhiteSpace(str)
                ? repetition
                : str;
        }

        public static String ShortString(this String str, int length)
        {
            var wide = 0;
            var len = 0;
            foreach (var ch in str)
            {
                // simple place a wide character
                wide += ch < 0x1000 ? 1 : 2;
                len++;
                if (wide > length)
                    return str.Substring(0, len - 4) + " ...";
            }
            return str;
        }

        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null)
                return null;

            if (stream is MemoryStream)
            {
                var ms = (MemoryStream)stream;
                return ms.ToArray();
            }

            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int len;
                while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, len);
                }
                return ms.ToArray();
            }
        }

        public static String ReadLines(this StringReader reader, int lineCount)
        {
            var sb = new StringBuilder();
            while (lineCount-- > 0)
            {
                sb.Append(reader.ReadLine());
                if (lineCount > 0)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        public static String ToLocateString(this ChangeKind changeKind)
        {
            switch (changeKind)
            {
                case ChangeKind.Added:
                    return SR.Repository_FileAdded;
                case ChangeKind.Copied:
                    return SR.Repository_FileCopied;
                case ChangeKind.Deleted:
                    return SR.Repository_FileDeleted;
                case ChangeKind.Ignored:
                    return SR.Repository_FileIgnored;
                case ChangeKind.Modified:
                    return SR.Repository_FileModified;
                case ChangeKind.Renamed:
                    return SR.Repository_FileRenamed;
                case ChangeKind.TypeChanged:
                    return SR.Repository_FileTypeChanged;
                case ChangeKind.Unmodified:
                    return SR.Repository_FileUnmodified;
                case ChangeKind.Untracked:
                    return SR.Repository_FileUntracked;
                default:
                    return String.Empty;
            }
        }

        [Pure]
        public static String SafyToString(this object obj)
        {
            if (obj == null)
                return null;
            return obj.ToString();
        }
    }
}