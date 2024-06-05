export interface Driver {
    id: number
    username: string
    name: string
    lastname: string
    verificationStatus: string
    avgRate: number
    isBlocked: boolean
    ratings: Rating[]
  }
  
  export interface Rating {
    userUsername: string,
    stars: number
    message: string
  }