namespace Core.ConfigModel
{
    public class SiteUIConfigs
    {
        // Default page
        public string SiteUITitleHeader { get; set; }
        public string SiteUITitleFooter { get; set; }
        public bool SiteUIEnableAD { get; set; }

        // -- Login page
        public string SiteUILoginTitleHeader { get; set; }
        public string SiteUILoginBackgroundUrl { get; set; }

        // ScreenAlarm and ScreenProcessEvent
        public string ScreenAlarm { get; set; }
        public string ScreenProcessEvent { get; set; }

        // Key google maps
        public string SiteUILKeyGoogleMap { get; set; }
    }
}
