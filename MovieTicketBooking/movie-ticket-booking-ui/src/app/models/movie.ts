export interface Movie {
  id: number;
  title: string;
  description: string;
  language: string;
  genre: string;
  durationInMinutes: number;
  releaseDate: string;
  posterUrl: string;
}

export interface CreateMovieDto {
  title: string;
  description: string;
  language: string;
  genre: string;
  durationInMinutes: number;
  releaseDate: string;
  posterUrl?: string;
}

export interface UpdateMovieDto {
  title: string;
  description: string;
  language: string;
  genre: string;
  durationInMinutes: number;
  releaseDate: string;
  posterUrl?: string;
}
