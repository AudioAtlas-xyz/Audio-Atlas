export interface CreateSubmissionRequest {
  NewGenreName: string;
  Aliases: string[];
  CountryIds: string[];
  Description: string;
  ExampleSongYoutubeId: string;
  IsSensitive: boolean;
  SensitiveDescription: string;
  PredecessorGenreIds: string[];
  SubGenreIds: string[];
  SimilarGenreIds: string[];
  InstrumentIds: string[];
  SourceLinks?: string[];
  StartDate: string | null;
  EndDate: string | null;
}
