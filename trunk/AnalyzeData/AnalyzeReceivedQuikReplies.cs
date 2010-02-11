using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;

namespace AnalyzeData
{
    class AnalyzeReceivedQuikReplies
    {
        [STAThread]
        static void Main(string[] args)
        {
            AnalyzeReplies();
        }

        static List<string> patterns = new List<string>()
        {
            @"Recieve <- \(0; 0; 3; \d+; \d+; ""\[FORTS] Заявка N \d+ успешно зарегистрирована""\)",
            @"Recieve <- \(0; 0; 3; \d+; 0; ""\[FORTS] Операция выполнена успешно. Снятое количество: 1""\)",

            @"Recieve <- \(0; 0; 4; \d+; 0; ""\[FORTS] Не разрешена встречная заявка на один счет и/или ИНН.""\)",
            @"Recieve <- \(0; 0; 4; \d+; 0; ""\[FORTS] Не найдена заявка для удаления""\)",
            @"Recieve <- \(0; 0; 4; \d+; 0; ""\[FORTS] Идет пром. клиринг, нельзя ставить заявки.""\)",
            @"Recieve <- \(0; 0; 4; \d+; 0; ""\[FORTS] Сейчас эта сессия не идет.""\)",
            @"Recieve <- \(0; 0; 5; \d+; 0; ""Вы не можете снять данную заявку""\)",
            @"Recieve <- \(0; 0; 6; \d+; 0; ""Превышен лимит по инструменту""\)",
            @"Recieve <- \(0; 0; 6; \d+; 0; ""Неопределено ГО по инструменту""\)",

            @"Recieve <- \(5; 0; 0; 0; 0; "" Указанный класс не найден: ""SPBFUT""""\)",
            @"Recieve <- \(5; 0; 0; \d+; 0; "" Указанная транзакция по указанному классу не найдена: ""SPBFUT"".""\)",
            @"Recieve <- \(5; 0; 0; 0; 0; "" Неправильно указана цена: ",

            @"Recieve <- \(0; 0; 2; \d+; 0; ""Gate for SPBFUT589000 not active""\)",
            @"Recieve <- \(0; 0; 0; \d+; 0; ""Gate for SPBFUT589000 not active""\)",
            @"Recieve <- \(0; 0; 2; \d+; 0; ""No gate for SPBFUT589000""\)",
            @"Recieve <- \(0; 0; 0; \d+; 0; ""No gate for SPBFUT589000""\)",
            @"Recieve <- \(0; 0; 2; \d+; 0; ""Communication gate is down""\)"
        };

        public static void AnalyzeReplies()
        {
            var parsers = patterns.Select(pattern => new Regex(pattern)).ToList();

            SlyBotDataContext dataContext = new SlyBotDataContext();
            
            var replies = (from logMsg in dataContext.Logs 
                          where logMsg.Logger == "QuikApi" && logMsg.Message.Contains("Recieve <- ")
                          select logMsg.Message).ToList();

            var notMatchingReply = replies.FirstOrDefault(reply => !parsers.Any(pars => pars.IsMatch(reply)));

            if (string.IsNullOrEmpty(notMatchingReply))
            {
                Console.WriteLine("All replies have matching patterns!");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(notMatchingReply);
                Debugger.Break();
            }

            var groups = replies.GroupBy(reply => parsers.Single(pars => pars.IsMatch(reply))).ToList();

            groups.Sort((a, b) => b.Count() - a.Count());

            groups.ForEach(group => Console.WriteLine(group.Key.ToString().Substring(10) + ": " + group.Count()));

            Console.ReadLine();
        }

    }
}
