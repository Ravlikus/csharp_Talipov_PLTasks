using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task3
{
    class Program
    {
        static string _usage = "g - генерирует пример лога (log.log) на 1 мб\n" +
            "g <название/путь генерируемого лог файла> - генерирует пример лога на 1 мб по заданному пути/названию\n" +
            "<путь до лог файла> <начало периода> <конец периода> - генерирует отчет по периоду в report.log";
        static Regex _dateTimeTemplate = new Regex(@"(\d{4}-\d{2}-\d{2}[TТ]\d{2}:\d{2}:\d{2}\.\d{3}Z)");
        static Regex _usernameTemplate = new Regex(@"(\[.*\])");
        static Regex _valueTemplate = new Regex(@" (\d*)l");
        static Regex _scoopTemplate = new Regex(@"scoop");

        static string[] testArgs = new string[] { "log.log", "2021-07-05T16:18:16.104Z", "2021-07-05T16:18:16.153Z" };

        static void Main(string[] args)
        {
            //args = testArgs;
            if (args.Length == 1 && args[0] == "g")
            {
                try
                {
                    LogGenerator.GenerateLog(17000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (args.Length == 2 && args[0] == "g")
            {
                try
                {
                    LogGenerator.GenerateLog(17000, args[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (args.Length == 3)
            {
                DateTime from;
                DateTime to; 
                if (DateTime.TryParse(args[1], out from) && DateTime.TryParse(args[2], out to))
                {
                    var processResult = ProcessLog(args[0], from, to);
                    File.WriteAllText("report.csv", processResult.GetCsvString());
                }
            }
            else
            {
                Console.WriteLine(_usage);
            }
        }

        static LogProcessResult ProcessLog(string fileName, DateTime from, DateTime to)
        {
            var fs = new FileStream($"{fileName}", FileMode.Open);
            var sr = new StreamReader(fs);
            var result = new LogProcessResult();
            var enteredPeriod = false;
            var exitPeriod = false;

            sr.ReadLine();
            var regex = new Regex(@"(\d+)");
            result.maxWaterValue = int.Parse(regex.Match(sr.ReadLine()).Groups[1].Value);
            result.currentWaterValue = int.Parse(regex.Match(sr.ReadLine()).Groups[1].Value);

            while (!sr.EndOfStream)
            {
                var currentLine = sr.ReadLine();
                if (IsValidLogString(currentLine))
                {
                    ProcessLogString(from, to, result, ref enteredPeriod, ref exitPeriod, currentLine);
                }
                if(exitPeriod == true)
                {
                    return result;
                }
            }
            return result;

        }

        private static void ProcessLogString(DateTime from, 
            DateTime to, 
            LogProcessResult result, 
            ref bool enteredPeriod, 
            ref bool exitPeriod, 
            string currentLine)
        {
            var value = GetValueFromLogString(currentLine);
            var date = DateTime.Parse(_dateTimeTemplate.Match(currentLine).Groups[1].Value);
            var isScooped = value < 0;
            var isSuccessfull = result.currentWaterValue + value >= 0
                && result.currentWaterValue + value <= result.maxWaterValue;

            if (isSuccessfull) result.currentWaterValue += value;

            if (date >= from && date <= to)
            {
                enteredPeriod = true;
                if (result.fillAttemtsCount + result.scoopedAttemtsCount == 0)
                    result.startPeriodWaterValue = result.currentWaterValue;
                if (enteredPeriod && !exitPeriod) result.endPeriodWaterValue = result.currentWaterValue;
                result.totalAttemtsCount++;
                if (!isSuccessfull)
                {
                    if (isScooped)
                    {
                        result.failedScoopWaterValue -= value;
                        result.scoopedAttemtsCount++;
                    }
                    else
                    {
                        result.failedFillWaterValue += value;
                        result.fillAttemtsCount++;
                    }
                    result.failedAttemptsCount++;
                }
                else
                {
                    if (isScooped)
                    {
                        result.scoopedWaterValue -= value;
                        result.scoopedAttemtsCount++;
                    }
                    else
                    {
                        result.filledWaterValue += value;
                        result.fillAttemtsCount++;
                    }
                }
            }
            if (date > to && enteredPeriod) exitPeriod = false;
        }

        static bool IsValidLogString(string LogStr)
        {
            return _dateTimeTemplate.IsMatch(LogStr)
                && _usernameTemplate.IsMatch(LogStr)
                && _valueTemplate.IsMatch(LogStr);
        }

        static int GetValueFromLogString(string LogStr)
        {
            return int.Parse(_valueTemplate.Match(LogStr).Groups[1].Value) *
                (_scoopTemplate.IsMatch(LogStr) ? -1 : 1);
        }

        class LogProcessResult
        {
            public int maxWaterValue = 0;
            public int fillAttemtsCount = 0;
            public int totalAttemtsCount = 0;
            public int failedAttemptsCount = 0;
            public int filledWaterValue = 0;
            public int failedFillWaterValue = 0;
            public int scoopedAttemtsCount = 0;
            public int scoopedWaterValue = 0;
            public int failedScoopWaterValue = 0;
            public int startPeriodWaterValue = 0;
            public int endPeriodWaterValue = 0;
            public int currentWaterValue = 0;

            public string GetCsvString()
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var result = new StringBuilder();
                result.AppendLine($"Top up attempts,{fillAttemtsCount}");
                result.AppendLine($"Error percentage,{failedAttemptsCount/(double)totalAttemtsCount*100}%");
                result.AppendLine($"Topped up water value,{filledWaterValue}l");
                result.AppendLine($"Not topped up water value,{failedFillWaterValue}l");
                result.AppendLine($"Scooped water value,{scoopedWaterValue}l");
                result.AppendLine($"Not scooped water value,{failedScoopWaterValue}l");
                result.AppendLine($"Period start water value,{startPeriodWaterValue}l");
                result.AppendLine($"Period end water value,{endPeriodWaterValue}l");

                return result.ToString();
            }
        }
    }
}
