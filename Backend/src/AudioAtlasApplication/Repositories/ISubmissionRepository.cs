using AudioAtlasDomain.Submissions;

namespace AudioAtlasApplication.Repositories;

public interface ISubmissionRepository
{
    public Task addAsync(Submission submission, CancellationToken cancellationToken = default);
}
