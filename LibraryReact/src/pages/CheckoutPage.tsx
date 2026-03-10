import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { getBooks } from '../api/books'
import { checkOutBook } from '../api/checkouts'

// Members are hardcoded to match the seed data. In a full app there would be a /api/members endpoint.
const MEMBERS = [
  { id: 1, name: 'Alice Hinson'  },
  { id: 2, name: 'Robert Martin' },
  { id: 3, name: "Rodj O'Matic"  },
]

export default function CheckoutPage() {
  const queryClient = useQueryClient()
  const { data: books, isLoading } = useQuery({ queryKey: ['books'], queryFn: getBooks })

  const [bookId,   setBookId]   = useState<number | ''>('')
  const [memberId, setMemberId] = useState<number | ''>('')
  const [message,  setMessage]  = useState('')

  const availableBooks = books?.filter(b => b.isAvailable) ?? []

  const mutation = useMutation({
    mutationFn: () => checkOutBook(Number(bookId), Number(memberId)),
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: ['books'] })
      setBookId(''); setMemberId('')
      setMessage(`✓ "${result.bookTitle}" checked out to ${result.memberName}. Due ${new Date(result.dueDate).toLocaleDateString()}.`)
      setTimeout(() => setMessage(''), 5000)
    },
    onError: (err: Error) => setMessage(`Error: ${err.message}`),
  })

  if (isLoading) return <p className="text-gray-500">Loading…</p>

  return (
    <div className="max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-800">Check Out a Book</h1>
      <div className="bg-white rounded-lg shadow p-6 space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Book</label>
          <select className="w-full border rounded px-3 py-2 text-sm"
            value={bookId} onChange={e => setBookId(Number(e.target.value) || '')}>
            <option value="">— select a book —</option>
            {availableBooks.map(b => (
              <option key={b.id} value={b.id}>{b.title}</option>
            ))}
          </select>
          {availableBooks.length === 0 && (
            <p className="text-xs text-gray-400 mt-1">No books currently available.</p>
          )}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Member</label>
          <select className="w-full border rounded px-3 py-2 text-sm"
            value={memberId} onChange={e => setMemberId(Number(e.target.value) || '')}>
            <option value="">— select a member —</option>
            {MEMBERS.map(m => (
              <option key={m.id} value={m.id}>{m.name}</option>
            ))}
          </select>
        </div>
        <button
          className="w-full bg-blue-700 text-white px-4 py-2 rounded font-medium hover:bg-blue-800 disabled:opacity-50"
          onClick={() => mutation.mutate()}
          disabled={!bookId || !memberId || mutation.isPending}
        >
          {mutation.isPending ? 'Checking out…' : 'Check Out'}
        </button>
        {message && (
          <p className={`text-sm ${message.startsWith('Error') ? 'text-red-600' : 'text-green-700'}`}>
            {message}
          </p>
        )}
      </div>
    </div>
  )
}
