using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AcademicCalendar
{
    public class HtmlScrapper
    {

        public List<List<string>> GetAcademicCalender()
        {
            string url = "https://www.odu.edu/academics/calendar/spring";
            var response = CallUrl(url).Result;
            var calendarEvents = ParseHtml(response);
            return calendarEvents;
        }
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }
        private List<List<string>> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var calendarDate = htmlDoc.DocumentNode.Descendants("tr");

            List<string> calendarDates = new List<string>();
            foreach (var date in calendarDate)
            {
                if (date.HasChildNodes)
                {
                    String temp = date.InnerText.Replace("\n", " - ").Trim().Trim('-');
                    calendarDates.Add(temp);
                }
            }

            var listOfCalenders = new List<List<string>>();
            if (calendarDates.Count > 30)
            {

                for (int i = 0; i < calendarDates.Count; i += 10)
                {
                    listOfCalenders.Add(calendarDates.GetRange(i, Math.Min(10, calendarDates.Count - i)));
                }
            }

            return listOfCalenders;
        }
    }
}
