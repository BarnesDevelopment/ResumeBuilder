export interface User {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
}

export interface Cookie {
  key: string;
  userId: string;
  expiration: Date;
  active: boolean;
}
