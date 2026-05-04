using AudioAtlasDomain.Submissions;

namespace AudioAtlasApplication.Repositories;

public interface ISubmissionRepository
{
    public Task addAsync(Submission submission, CancellationToken cancellationToken = default);
    public Task<Submission?> getByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    public Task<ICollection<Submission>> getPendingAsync(CancellationToken cancellationToken = default);
    public Task saveChangesAsync(CancellationToken cancellationToken = default);
}
