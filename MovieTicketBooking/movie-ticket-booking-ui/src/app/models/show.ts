// Model matching the backend ShowResponseDto
export interface Show {
  id: number;
  movieId: number;
  movieTitle: string;
  screenId: number;
  screenName: string;
  theatreId: number;
  theatreName: string;
  startTime: string;
  endTime: string;
  price: number;
  totalSeats: number;
}
