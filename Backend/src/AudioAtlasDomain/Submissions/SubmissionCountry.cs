using AudioAtlasDomain.Geography;
using System.ComponentModel.DataAnnotations;

namespace AudioAtlasDomain.Submissions
{
    public class SubmissionCountry
    {
        public Guid SubmissionId { get; set; }

        public Guid CountryId { get; set; }

        public Submission Submission { get; set; } = null!;
        public Country Country { get; set; } = null!;
    }
}
