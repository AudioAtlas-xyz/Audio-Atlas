using AudioAtlasDomain.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioAtlasInfrastructure.Database.Configuration.Submission;

public class RejectionReasonConfiguration : IEntityTypeConfiguration<RejectionReason>
{
    public void Configure(EntityTypeBuilder<RejectionReason> builder)
    {
        builder.ToTable("RejectionReasons");

        builder.HasKey(x => x.Code);

        builder.Property(x => x.Code)
            .HasColumnType("varchar(40)")
            .IsRequired();

        builder.Property(x => x.Label)
            .HasColumnType("nvarchar(120)")
            .IsRequired();

        builder.Property(x => x.GuidelineRef)
            .HasColumnType("nvarchar(80)");

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasData(
            new RejectionReason
            {
                Code = "origin_misattribution",
                Label = "Origin incorrectly credited",
                GuidelineRef = "respect-cultural-origins",
                SortOrder = 1,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "terminology_issue",
                Label = "Inappropriate or incorrect naming/terms",
                GuidelineRef = "appropriate-terminology",
                SortOrder = 2,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "unverifiable_speculative",
                Label = "Speculative claim, not supportable",
                GuidelineRef = "do-not-speculate",
                SortOrder = 3,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "insufficient_sources",
                Label = "Missing or inadequate citations",
                GuidelineRef = "cite-sources",
                SortOrder = 4,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "inaccurate_content",
                Label = "Factually incorrect history/context",
                GuidelineRef = "accuracy",
                SortOrder = 5,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "incomplete_required_fields",
                Label = "Missing required data fields",
                GuidelineRef = "genre-data-requirements",
                SortOrder = 6,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "duplicate",
                Label = "Already exists in the atlas",
                GuidelineRef = "data-integrity",
                SortOrder = 7,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "out_of_scope",
                Label = "Not a music genre / not notable",
                GuidelineRef = "scope",
                SortOrder = 8,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "copyright_violation",
                Label = "Copied or unlicensable content",
                GuidelineRef = "licensing-copyright",
                SortOrder = 9,
                IsActive = true
            },
            new RejectionReason
            {
                Code = "spam_or_abuse",
                Label = "Spam, vandalism, or CoC breach",
                GuidelineRef = "code-of-conduct",
                SortOrder = 10,
                IsActive = true
            }
        );
    }
}
