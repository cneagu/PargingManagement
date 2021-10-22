export interface Token {
  expires: string,
  value: string
}

export interface RefreshReturn {
  accessToken?: Token;
  refreshToken?: Token;
}

export interface ValidationError {
  property: string;
  message: string;
}
