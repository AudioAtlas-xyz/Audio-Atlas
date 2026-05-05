export interface CreateSubmissionRequest {
  NewGenreName: string;
  Aliases: string[];
  CountryIds: string[];
  Description: string;
  PlaylistLink: string;
  IsSensitive: boolean;
  SensitiveDescription: string;
  PredecessorGenreIds: string[];
  SubGenreIds: string[];
  SimilarGenreIds: string[];
  StartDate: string;
  EndDate: string;
}
