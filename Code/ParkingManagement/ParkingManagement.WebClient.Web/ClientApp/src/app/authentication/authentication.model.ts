import { Token } from '../core/token.model';

export interface LoginModel {
  email: string,
  password: string
}

export interface LoginReturn {
  status: LoginStatus,
  accessToken?: Token,
  refreshToken?: Token
}

export enum LoginStatus {
  None = 0,
  Success = 1,
  InvalidEmail = 2,
  AccountDisabled = 3,
  InvalidPassword = 4
}

export enum ForgotPassword {
  None = 0,
  Success = 1,
  InvalidEmail = 2,
  AccountDisabled = 3
}

export interface ForgotPasswordReturn {
  status: ForgotPassword
}

export interface RegisterModel {
  firstName: string,
  lastName: string,
  email: string,
  password: string
}

export interface RegisterReturn {
  status: RegisterStatus,
  accessToken?: Token,
  refreshToken?: Token
}

export enum RegisterStatus {
  None = 0,
  Success = 1,
  InvalidEmail = 2
}
