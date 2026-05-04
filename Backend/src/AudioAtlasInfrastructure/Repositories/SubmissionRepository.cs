using AudioAtlasApplication.Repositories;
using AudioAtlasDomain.Submissions;
using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Submission?> getByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions
            .Include(submission => submission.RejectedSubmission)
            .SingleOrDefaultAsync(submission => submission.Id == submissionId, cancellationToken);
    }

    public async Task<ICollection<Submission>> getPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions
            .Where(submission => !submission.IsRejected && !submission.IsApproved)
            .Include(submission => submission.Account)
            .Include(submission => submission.Aliases)
            .Include(submission => submission.Sources)
            .Include(submission => submission.Countries)
            .Include(submission => submission.SimilarGenres)
            .Include(submission => submission.SubGenres)
            .Include(submission => submission.PredecessorGenres)
            .ToListAsync(cancellationToken);
    }

    public async Task saveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
