namespace AudioAtlasDomain.Submissions;

/// <summary>
/// A curator-facing lookup row that classifies why a submission was rejected.
///
/// Code is the stable, machine-readable PK (e.g. "origin_misattribution").
/// Label is the human-readable display string.
/// GuidelineRef is an optional slug pointing to the relevant contribution
/// guideline section so curators can link reviewees to documentation.
/// </summary>
public class RejectionReason
{
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string? GuidelineRef { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
