using AudioAtlasApplication.DTOs;

namespace AudioAtlasApplication.Services;

public interface ISubmissionService
{
    public Task<Guid> createSubmissionAsync(Guid accountId, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
    public Task<ICollection<PendingSubmissionResponse>> getPendingAsync(CancellationToken cancellationToken = default);
    public Task approveAsync(Guid submissionId, Guid reviewerId, CancellationToken cancellationToken = default);
    public Task rejectAsync(Guid submissionId, Guid reviewerId, RejectSubmissionRequest request, CancellationToken cancellationToken = default);
    public Task holdForSensitivityAsync(Guid submissionId, Guid reviewerId, CancellationToken cancellationToken = default);
    public Task<ICollection<RejectionReasonResponse>> getActiveRejectionReasonsAsync(CancellationToken cancellationToken = default);
}
