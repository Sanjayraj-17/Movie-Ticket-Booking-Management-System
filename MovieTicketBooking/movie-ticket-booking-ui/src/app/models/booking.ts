// Models matching the backend BookingResponseDto and BookingSeatResponseDto
export interface BookingSeat {
  seatId: number;
  row: string;
  seatNumber: string;
  seatType: string;
  price: number;
}

export interface Booking {
  id: number;
  showId: number;
  movieTitle: string;
  screenName: string;
  theatreName: string;
  startTime: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  bookingTime: string;
  totalAmount: number;
  bookingStatus: string;
  seats: BookingSeat[];
}

export interface CreateBookingRequest {
  showId: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  seatIds: number[];
}
