using System;

namespace QLogParser.Services.Messages
{
    public class GoogleAnalyticsCookies
    {
        public string CampaignSource { get; set; }
        public string CampaignName { get; set; }
        public string CampaignMedium { get; set; }
        public string CampaignContent { get; set; }
        public string CampaignTerm { get; set; }

        /// <summary>
        /// Local Time
        /// </summary>
        public DateTime FirstVisit { get; set; }

        /// <summary>
        /// Local Time
        /// </summary>
        public DateTime PreviousVisit { get; set; }

        /// <summary>
        /// Local Time
        /// </summary>
        public DateTime CurrentVisitStarted { get; set; }

        public int TimesVisited { get; set; }
        public int PagesViewed { get; set; }

        public GoogleAnalyticsCookies()
        {
            CampaignSource = CampaignName = CampaignMedium = CampaignContent = CampaignTerm = "";
            FirstVisit = PreviousVisit = CurrentVisitStarted = DateTime.Parse("01/01/1900");
            TimesVisited = PagesViewed = 0;
        }
    }
}
