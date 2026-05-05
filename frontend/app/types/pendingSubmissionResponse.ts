export interface PendingSubmissionResponse {
  id: Guid;
  accountId: string;
  accountUsername?: string;
  newGenreName?: string;
  startDate?: DateOnly;
  endDate?: DateOnly;
  description?: string;
  isSensitive?: boolean;
  sensitiveDescription?: string;
  playlistLink?: string;
  aliases: string[];
  sourceLinks?: string;
  countryIds: Guid[];
  instrumentsIds: Guid[];
  similarGenreIds: Guid[];
  subGenreIds: Guid[];
  predecessorGenreIds: Guid[];
}
