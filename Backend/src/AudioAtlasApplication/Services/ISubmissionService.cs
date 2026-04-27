using AudioAtlasApplication.DTOs;

namespace AudioAtlasApplication.Services;

public interface ISubmissionService
{
    public Task<Guid> createSubmissionAsync(Guid accountId, CreateSubmissionRequest request, CancellationToken cancellationToken = default);
}
