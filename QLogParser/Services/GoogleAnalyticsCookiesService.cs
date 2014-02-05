using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using QLogParser.Services.Messages;

namespace QLogParser.Services
{
    public class GoogleAnalyticsCookiesService
    {
        public GoogleAnalyticsCookiesService()
        {
        }

        public GoogleAnalyticsCookies ParseGoogleAnalyticsCookies(string cookies)
        {
            GoogleAnalyticsCookies googleAnalyticsCookies = new GoogleAnalyticsCookies();

            if (string.IsNullOrEmpty(cookies))
                return googleAnalyticsCookies;

            try
            {
                string[] cookieList = cookies.Split(new char[] { ';' });

                string utma = "";
                string utmb = "";
                string utmz = "";

                foreach (string s in cookieList)
                {
                    if (s.StartsWith("+__utma="))
                        utma = s.Replace("+__utma=", "");

                    if (s.StartsWith("+__utmb="))
                        utmb = s.Replace("+__utmb=", "");

                    if (s.StartsWith("+__utmz="))
                        utmz = s.Replace("+__utmz=", "");
                }

                if (utma != "")
                {
                    // utmaList = [domain_hash, random_id, time_initial_visit, time_beginning_previous_visit, time_beginning_current_visit, session_counter]
                    string[] utmaList = utma.Split(new char[] { '.' });

                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                    try { googleAnalyticsCookies.FirstVisit = origin.AddSeconds(Double.Parse(utmaList[2])); }
                    catch { }

                    try { googleAnalyticsCookies.PreviousVisit = origin.AddSeconds(Double.Parse(utmaList[3])); }
                    catch { }

                    try { googleAnalyticsCookies.CurrentVisitStarted = origin.AddSeconds(Double.Parse(utmaList[4])); }
                    catch { }

                    try { googleAnalyticsCookies.TimesVisited = Int32.Parse(utmaList[5]); }
                    catch { }
                }

                if (utmb != "")
                {
                    // utmbList = [domain_hash, pages_viewed, garbage, time_beginning_current_session]
                    string[] utmbList = utmb.Split(new char[] { '.' });

                    try { googleAnalyticsCookies.PagesViewed = Int32.Parse(utmbList[1]); }
                    catch { }
                }

                if (utmz != "")
                {
                    // utmzList = [domain_hash, timestamp, session_number, campaign_number, campaign_data]
                    string[] utmzList = { "" };
                    try { utmzList = utmz.Split(new char[] { '.' }, 5); }
                    catch { }

                    if (utmzList.Count() >= 5)
                    {
                        // Campaign Data
                        string[] campaignData = utmzList[4].Split(new char[] { '|' });

                        foreach (string s in campaignData)
                        {
                            if (s.StartsWith("utmcsr="))
                                googleAnalyticsCookies.CampaignSource = HttpUtility.UrlDecode(s.Replace("utmcsr=", "").Replace("(", "").Replace(")", "")).Replace("+", " ");

                            if (s.StartsWith("utmccn="))
                                googleAnalyticsCookies.CampaignName = HttpUtility.UrlDecode(s.Replace("utmccn=", "").Replace("(", "").Replace(")", "")).Replace("+", " ");

                            if (s.StartsWith("utmcmd="))
                                googleAnalyticsCookies.CampaignMedium = HttpUtility.UrlDecode(s.Replace("utmcmd=", "").Replace("(", "").Replace(")", "")).Replace("+", " ");

                            if (s.StartsWith("utmctr="))
                                googleAnalyticsCookies.CampaignTerm = HttpUtility.UrlDecode(s.Replace("utmctr=", "")).Replace("+", " ");

                            if (s.StartsWith("utmcct="))
                                googleAnalyticsCookies.CampaignContent = HttpUtility.UrlDecode(s.Replace("utmcct=", "")).Replace("+", " ");
                        }
                    }
                }
            }
            catch { }

            return googleAnalyticsCookies;
        }
    }
}
