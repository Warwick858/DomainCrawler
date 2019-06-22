using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Whois;
using Whois.Models;

namespace DomainCrawler
{
    public class Program
    {
        //Class Vars:
        public static List<string> _domains = new List<string>();
        public static List<string> _candidates = new List<string>();
        public static int _errorCount = 0;
        public static int _successCount = 0;
        public static int _totalCount = 0;

        static void Main(string[] args)
        {
            //Init locals
            WhoisResponse response = null;
            WhoisLookup whois = null;

            //Setup
            SetupDomainList();

            foreach (var d in _domains)
            {
                if (DomainIsValid(d))
                    try
                    {
                        whois = new WhoisLookup();
                        response = whois.Lookup(d);
                        AnalyzeData(response?.ParsedResponse);

                        Console.WriteLine("\n\n*******************************************************");
                        Console.WriteLine($"************************Results for {d}: ");
                        Console.WriteLine(ScrubContent(response?.Content));
                        Console.WriteLine("\n*******************************************************\n");
                    } // end try
                    catch (Exception)
                    {
                        Console.WriteLine($"Lookup of domain: {d} failed!");
                        Thread.Sleep(1000);
                    } // end catch
            } // end foreach

            PrintReport();

            //Wait for input to exit
            Console.Write("Press enter to exit...");
            Console.ReadKey();
        } // end method Main

        public static void SetupDomainList()
        {
            _domains.AddRange(Domains.DomainList);
        } // end method SetupDomainList

        public static string ScrubContent(string content)
        {
            var marker = content.IndexOf('<'); 
            return content.Remove(marker, (content.Length - marker));
        } // end method ScrubContent

        public static bool DomainIsValid(string domain)
        {
            _totalCount++;

            if (string.IsNullOrWhiteSpace(domain))
            {
                _errorCount++;
                return false;
            }
            if (domain.Split('.').Last() != "com" && domain.Split('.').Last() != "net")
            {
                _errorCount++;
                return false;
            }

            _successCount++;
            return true;
        } // end method DomainIsValid

        public static void PrintReport()
        {
            Console.WriteLine("*******************************************************");
            Console.WriteLine("*******************************************************");
            Console.WriteLine(" _______                             _");
            Console.WriteLine("|  ___  \\                           | |");
            Console.WriteLine("| |___) |____ __ ___   _____   ___ _| |_");
            Console.WriteLine("|  ___  / __ \\  '_  \\ /  _  \\| '____| _|");
            Console.WriteLine("| |   \\ \\ __ /  |_)  |  (_)  |  |   | |_");
            Console.WriteLine("|_|    \\_\\____|  .__/ \\_____/|__|   \\___|");
            Console.WriteLine("              | |");
            Console.WriteLine("              |_|");
            Console.WriteLine($"\nTotal Errors: {_errorCount}");
            Console.WriteLine($"Total Successes: {_successCount}");
            Console.WriteLine($"Total Count: {_totalCount}");
            Console.WriteLine($"\nCandidates: {_candidates.Count.ToString()}");
            _candidates.ForEach(x => Console.WriteLine(x));
            Console.WriteLine("\n\n*******************************************************");
            Console.WriteLine("*******************************************************\n\n");

            File.WriteAllLines(@"D:\Code Projects\DomainCrawler\Candidates\" + DateTime.Now.ToLongDateString() + "_data.txt", _candidates);
        } // end method PrintReport

        public static void AnalyzeData(ParsedWhoisResponse response)
        {
            if (response?.Expiration < DateTime.Now.AddDays(30))
            {
                _candidates.Add($"Domain: {response.DomainName}, " +
                    $"Expiration Date: {response.Expiration}," +
                    $"Registrar: {response.Registrar.Name}," +
                    $"Contact (Admin): {response.AdminContact.Name}," +
                    $"Contact (Tech): {response.TechnicalContact.Name}");
            } // end if
        } // end method AnalyzeData
    } // end class Program
} // end namespace DomainCrawler
