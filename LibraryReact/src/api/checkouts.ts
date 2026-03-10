import { api } from './client'
import type { CheckoutDetail, DashboardStats } from '../types'

export const checkOutBook = (bookId: number, memberId: number) =>
  api.post<CheckoutDetail>('/checkouts', { bookId, memberId })

export const returnBook = (checkoutId: number) =>
  api.post<CheckoutDetail>(`/checkouts/${checkoutId}/return`)

export const getOverdue = () => api.get<CheckoutDetail[]>('/checkouts/overdue')

export const getDashboard = () => api.get<DashboardStats>('/checkouts/dashboard')
