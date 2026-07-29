export interface PendingSubmissionResponse {
  id: string;
  accountId: string;
  accountUsername: string | null;
  newGenreName: string | null;
  submittedAt: string;
  startDate: string | null;
  endDate: string | null;
  description: string | null;
  isSensitive: boolean;
  sensitiveDescription: string | null;
  exampleSongYoutubeId: string | null;
  aliases: string[];
  sourceLinks: string[];
  countryIds: string[];
  instrumentIds: string[];
  similarGenreIds: string[];
  subGenreIds: string[];
  predecessorGenreIds: string[];
  targetGenreId: string | null;
  targetGenreName: string | null;
  isEditSuggestion: boolean;
}
