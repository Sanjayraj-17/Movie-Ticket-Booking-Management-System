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

// Physical Seat Models for Screen Management
export interface PhysicalSeat {
  id: number;
  screenId: number;
  rowNumber: string;
  seatNumber: string;
  seatType: string;
}

export interface CreateSeatDto {
  screenId: number;
  rowNumber: string;
  seatNumber: string;
  seatType: string;
}

export interface BulkCreateSeatsDto {
  screenId: number;
  rowNumber: string;
  totalSeatsInRow: number;
  seatType: string;
}
