// These interfaces mirror the API response shapes returned by the service layer
// (the result records defined in ICheckoutService.cs), NOT the raw C# entity
// models in Library.Core/Models. Names like BookSummary and CheckoutDetail
// reflect that — they are pre-shaped for the consumer, with computed fields
// (e.g. isAvailable) and denormalized display values (e.g. bookTitle, memberName).

export interface BookSummary {
  id: number
  createDate: Date
  title: string
  author: string
  isbn: string
  isAvailable: boolean
}

export interface CheckoutDetail {
  id: number
  createDate: Date
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
  createDate: Date
  name: string
  email: string
}
