import { api } from './client'
import type { BookSummary } from '../types'

export const getBooks = () => api.get<BookSummary[]>('/books')

export const addBook = (title: string, author: string, isbn: string) =>
  api.post<BookSummary>('/books', { title, author, isbn })
