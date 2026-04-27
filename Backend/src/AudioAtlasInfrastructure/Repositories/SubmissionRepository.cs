using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Submissions;
using AudioAtlasInfrastructure.Database;

namespace AudioAtlasInfrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _dbContext;

    public SubmissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task addAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        await _dbContext.Submissions.AddAsync(submission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
