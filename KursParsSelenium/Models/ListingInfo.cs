using System;

namespace KursParsSelenium
{
    public class ListingInfo
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string Link { get; set; }
        public decimal PriceQualityRatio
        => Convert.ToDecimal(Rating) / Price * 1000;
    }
}