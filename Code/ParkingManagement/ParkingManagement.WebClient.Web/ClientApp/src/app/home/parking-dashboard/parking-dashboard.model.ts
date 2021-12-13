export interface DashboardData {
  allocationResults: AllocationResult[],
  departments: Department[]
}
export interface AllocationResult {
  id: string,
  employeeName: string,
  email: string,
  parkingSpotNumber: number,
  date: Date
}

export interface Department {
  id: string,
  name: string
}
