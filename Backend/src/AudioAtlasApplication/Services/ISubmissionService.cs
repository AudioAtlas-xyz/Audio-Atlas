using AudioAtlasApplication.DTOs;

namespace AudioAtlasApplication.Services;

public interface ISubmissionService
{
    public Task<Guid> createSubmissionAsync(Guid accountId, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
    public Task<ICollection<PendingSubmissionResponse>> getPendingAsync(CancellationToken cancellationToken = default);
    public Task approveAsync(Guid submissionId, CancellationToken cancellationToken = default);
    public Task rejectAsync(Guid submissionId, RejectSubmissionRequest request, CancellationToken cancellationToken = default);
}
