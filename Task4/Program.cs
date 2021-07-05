using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task4
{
    class Program
    {
        static string[] testArgs = new string[] { "aaaa", "*****"};
        static void Main(string[] args)
        {
            //args = testArgs;
            var str1 = args[0];
            var str2 = $"^{args[1].Replace("*",".*")}$";
            var matched = Regex.IsMatch(str1, str2);
            if (matched)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("KO");
            }
        }
    }
}
