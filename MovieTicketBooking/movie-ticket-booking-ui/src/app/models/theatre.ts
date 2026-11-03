// Models matching backend TheatreResponseDto and ScreenResponseDto
export interface Theatre {
  id: number;
  name: string;
  city: string;
  address: string;
  totalScreens: number;
}

export interface Screen {
  id: number;
  theatreId: number;
  theatreName: string;
  screenName: string;
  totalSeats: number;
}

