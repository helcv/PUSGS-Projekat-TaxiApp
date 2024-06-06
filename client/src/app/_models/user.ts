export interface User {
  id: number;
  name: string;
  lastname: string;
  email: string;
  username: string;
  photoUrl: string;
  age: number;
  address: string;
  roles: string[];
  verificationStatus: string;
  busy: boolean;
}