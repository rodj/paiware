export interface BookSummary {
  id: number
  title: string
  author: string
  isbn: string
  isAvailable: boolean
}

export interface CheckoutDetail {
  id: number
  bookId: number
  bookTitle: string
  memberId: number
  memberName: string
  checkedOutAt: string
  dueDate: string
  returnedAt: string | null
}

export interface DashboardStats {
  totalBooks: number
  availableBooks: number
  checkedOutBooks: number
  overdueCheckouts: number
  totalMembers: number
}

export interface Member {
  id: number
  name: string
  email: string
}
