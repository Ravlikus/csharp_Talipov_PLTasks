using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        const string _usage = "Usage:\n<Original number> <Result base> - Convert number from decimal, to any base\n" +
            "<Original number> <Original base> <result base> - convert number from original base to result base";

        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine(_usage);
            }
            else if(args.Length == 2)
            {
                uint originalNumber = 0;
                if (!uint.TryParse(args[0], out originalNumber))
                {
                    Console.WriteLine(_usage);
                }
                else
                {
                    Console.WriteLine(IToBase(uint.Parse(args[0]), args[1]));
                }
            }
            else
            {
                Console.WriteLine(IToBase(args[0], args[1], args[2]));
            }
        }

        static string IToBase(uint originalNumber, string resultBase)
        {
            var baseValue = (uint)resultBase.Length;
            var result = new StringBuilder();
            var currentPower = 0;
            do
            {
                result.Insert(0, resultBase[(int)(originalNumber % baseValue )]);
                originalNumber /= baseValue;
                currentPower++;
            } while (originalNumber>0);
            return result.ToString();
        }

        static string IToBase(string originalNumber, string baseSrc, string baseDst)
        {
            var baseSrcValue = baseSrc.Length;
            var numberInDec = 0;
            for(int i = 0; i < originalNumber.Length; i++)
            {
                var digitNumber = baseSrc.IndexOf(originalNumber[i]);
                if (digitNumber == -1) return _usage;
                numberInDec += digitNumber * (int)(Math.Pow(baseSrcValue, originalNumber.Length - 1 - i));
            }

            return IToBase((uint)numberInDec, baseDst);
        }
    }
}
