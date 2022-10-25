//(c) Vasian Cepa 2005
// Version 2
//
// http://www.codeproject.com/Articles/11016/Numeric-String-Sort-in-C
// with a bit of modification

using System;
using System.Collections;
using System.Collections.Generic;

namespace GitCandy.Base
{
    // emulates StrCmpLogicalW, but not fully
    public class StringLogicalComparer : IComparer, IComparer<String>
    {
        public Int32 Compare(Object s1, Object s2) => Compare(s1 as String, s2 as String);

        public Int32 Compare(String s1, String s2)
        {
            //get rid of special cases
            if (ReferenceEquals(s1, s2)) return 0;
            else if ((s1 == null) && (s2 == null)) return 0;
            else if (s1 == null) return -1;
            else if (s2 == null) return 1;

            if ((s1.Equals(String.Empty) && (s2.Equals(String.Empty)))) return 0;
            else if (s1.Equals(String.Empty)) return -1;
            else if (s2.Equals(String.Empty)) return -1;

            //WE style, special case
            var sp1 = Char.IsLetterOrDigit(s1, 0);
            var sp2 = Char.IsLetterOrDigit(s2, 0);
            if (sp1 && !sp2) return 1;
            if (!sp1 && sp2) return -1;

            Int32 i1 = 0, i2 = 0; //current index
            var r = 0; // temp result
            while (true)
            {
                var c1 = Char.IsDigit(s1, i1);
                var c2 = Char.IsDigit(s2, i2);
                if (!c1 && !c2)
                {
                    var letter1 = Char.IsLetter(s1, i1);
                    var letter2 = Char.IsLetter(s2, i2);
                    if ((letter1 && letter2) || (!letter1 && !letter2))
                    {
                        if (letter1 && letter2)
                        {
                            r = Char.ToLower(s1[i1]).CompareTo(Char.ToLower(s2[i2]));
                        }
                        else
                        {
                            r = s1[i1].CompareTo(s2[i2]);
                        }
                        if (r != 0) return r;
                    }
                    else if (!letter1 && letter2) return -1;
                    else if (letter1 && !letter2) return 1;
                }
                else if (c1 && c2)
                {
                    r = CompareNum(s1, ref i1, s2, ref i2);
                    if (r != 0) return r;
                }
                else if (c1)
                {
                    return -1;
                }
                else if (c2)
                {
                    return 1;
                }
                i1++;
                i2++;
                if ((i1 >= s1.Length) && (i2 >= s2.Length))
                {
                    return 0;
                }
                else if (i1 >= s1.Length)
                {
                    return -1;
                }
                else if (i2 >= s2.Length)
                {
                    return 1;
                }
            }
        }

        private Int32 CompareNum(String s1, ref Int32 i1, String s2, ref Int32 i2)
        {
            Int32 nzStart1 = i1, nzStart2 = i2; // nz = non zero
            Int32 end1 = i1, end2 = i2;

            ScanNumEnd(s1, i1, ref end1, ref nzStart1);
            ScanNumEnd(s2, i2, ref end2, ref nzStart2);
            var start1 = i1; i1 = end1 - 1;
            var start2 = i2; i2 = end2 - 1;

            var nzLength1 = end1 - nzStart1;
            var nzLength2 = end2 - nzStart2;

            if (nzLength1 < nzLength2) return -1;
            else if (nzLength1 > nzLength2) return 1;

            for (Int32 j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
            {
                var r = s1[j1].CompareTo(s2[j2]);
                if (r != 0) return r;
            }
            // the nz parts are equal
            var length1 = end1 - start1;
            var length2 = end2 - start2;
            if (length1 == length2) return 0;
            if (length1 > length2) return -1;
            return 1;
        }

        //lookahead
        private void ScanNumEnd(String s, Int32 start, ref Int32 end, ref Int32 nzStart)
        {
            nzStart = start;
            end = start;
            var countZeros = true;
            while (Char.IsDigit(s, end))
            {
                if (countZeros && s[end].Equals('0'))
                {
                    nzStart++;
                }
                else countZeros = false;
                end++;
                if (end >= s.Length) break;
            }
        }
    }//EOC
}
