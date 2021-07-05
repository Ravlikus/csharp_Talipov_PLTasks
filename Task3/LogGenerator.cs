using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Task3
{
    public class LogGenerator
    {
        static string _dateTimeFormat = "yyyy-MM-ddТHH:mm:ss.fffZ";

        // 1мб примерно 17 000 строк
        public static void GenerateLog(int actionCount)
        {
            var maxValue = 200;
            var currentValue = 37;
            var rnd = new Random();
            var logFileName = $"log.log";
            File.Create($"{Directory.GetCurrentDirectory()}{logFileName}");
            var fs = new FileStream(logFileName, FileMode.Append);
            var sw = new StreamWriter(fs);
            sw.WriteLine("META DATA");
            sw.WriteLine($"{maxValue} (объём бочки)");
            sw.WriteLine($"{currentValue} (текущий объем воды в бочке)");

            for(int i = 0; i < actionCount; i++)
            {
                var change = rnd.Next(-100, 100);
                var name = Usernames[Math.Abs(change % 4)];
                if (change > 0)
                {
                    if (currentValue + change <= maxValue)
                    {
                        sw.WriteLine($"{DateTime.Now.ToString(_dateTimeFormat)} – [{name}] - wanna top up {change}l(успех)");
                        currentValue += change;
                    }
                    else
                    {
                        sw.WriteLine($"{DateTime.Now.ToString(_dateTimeFormat)} – [{name}] - wanna top up {change}l(фейл)");
                    }
                }
                else if (change < 0)
                {
                    if (currentValue + change >= 0)
                    {
                        sw.WriteLine($"{DateTime.Now.ToString(_dateTimeFormat)} – [{name}] - wanna scoop {-change}l(успех)");
                        currentValue += change;
                    }
                    else
                    {
                        sw.WriteLine($"{DateTime.Now.ToString(_dateTimeFormat)} – [{name}] - wanna scoop {-change}l(фейл)");
                    }
                }
                else
                {
                    i--;
                }
            }

            sw.Flush();
        }

        static string[] Usernames = new string[]
        {
            "John",
            "Carl",
            "Josh",
            "Kenny"
        };
    }
}
