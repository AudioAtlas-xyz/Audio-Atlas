export interface PendingSubmissionResponse {
  id: string;
  accountId: string;
  accountUsername?: string;
  newGenreName?: string;
  startDate?: string;
  endDate?: string;
  description?: string;
  isSensitive?: boolean;
  sensitiveDescription?: string;
  playlistLink?: string;
  aliases: string[];
  sourceLinks?: string;
  countryIds: string[];
  instrumentsIds: string[];
  similarGenreIds: string[];
  subGenreIds: string[];
  predecessorGenreIds: string[];
}
