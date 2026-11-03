// Models matching backend ShowSeatResponseDto and ShowSeatAvailabilitySummaryDto
export interface ShowSeat {
  showSeatId: number;
  showId: number;
  seatId: number;
  row: string;
  seatNumber: string;
  seatType: string;
  price: number;
  status: 'Available' | 'Booked' | string;
}

export interface ShowSeatAvailabilitySummary {
  showId: number;
  movieTitle: string;
  screenName: string;
  startTime: string;
  totalSeats: number;
  availableSeatsCount: number;
  bookedSeatsCount: number;
  seats: ShowSeat[];
}

export interface SeatRowGroup {
  rowLabel: string;
  seats: ShowSeat[];
}
