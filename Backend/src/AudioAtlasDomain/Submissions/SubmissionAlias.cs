namespace AudioAtlasDomain.Submissions;

/// <summary>
/// Represents a proposed alternative name within a submission.
/// 
/// Unlike GenreAlias, which reflects accepted terminology,
/// SubmissionAlias captures user-suggested naming that has not yet
/// been validated or integrated into the canonical data model.
/// 
/// This allows multiple naming possibilities to be evaluated
/// before becoming part of the established genre representation.
/// </summary>
public class SubmissionAlias
{
    /// <summary>
    /// Identifier of the associated submission.
    /// </summary>
    public Guid SubmissionId { get; set; }

    /// <summary>
    /// The proposed alternative name.
    /// 
    /// May represent slang, regional terminology, translations,
    /// or stylistic variations suggested by the user.
    /// 
    /// Not guaranteed to be accurate, unique, or accepted.
    /// </summary>
    public string Alias { get; set; } = null!;

    /// <summary>
    /// Navigation property to the submission this alias belongs to.
    /// 
    /// Multiple aliases can be proposed within a single submission.
    /// </summary>
    public Submission Submission { get; set; } = null!;
}