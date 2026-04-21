using System;
using System.Collections.Generic;
using System.Text;

namespace AudioAtlas.Domain.Geography
{
    public class ContributorSummaryDTO
    {
        public string id { get; set; } = null!;
        public string username { get; set; } = null!;
        public int genreCount { get; set; } = 0;


    }
}
