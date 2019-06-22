using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCrawler
{
    public static class Domains
    {
        private static List<string> _domainList = new List<string>
        {
            "www.potato.com",
            "redroses.com"
        };

        public static List<string> DomainList
        {
            get
            {
                PopulateDomainList();
                return _domainList;
            }
        }

        private static void PopulateDomainList()
        {
            var line = string.Empty;
            StreamReader stream = new StreamReader(@"D:\Code Projects\DomainCrawler\data1.txt");

            while ((line = stream.ReadLine()) != null)
            {
                _domainList.Add(line);
            } // end while
        } // end method PopulateDomainList
    } // end class Domains
} // end namespace DomainCrawler
