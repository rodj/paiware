import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { returnBook } from '../api/checkouts'

export default function ReturnPage() {
  const queryClient = useQueryClient()
  const [checkoutId, setCheckoutId] = useState<string>('')
  const [message, setMessage] = useState('')

  const mutation = useMutation({
    mutationFn: () => returnBook(Number(checkoutId)),
    onSuccess: (result) => {
      queryClient.invalidateQueries({ queryKey: ['books'] })
      setCheckoutId('')
      setMessage(`✓ "${result.bookTitle}" returned successfully.`)
      setTimeout(() => setMessage(''), 5000)
    },
    onError: (err: Error) => setMessage(`Error: ${err.message}`),
  })

  return (
    <div className="max-w-lg space-y-6">
      <h1 className="text-2xl font-bold text-gray-800">Return a Book</h1>
      <div className="bg-white rounded-lg shadow p-6 space-y-4">
        <p className="text-sm text-gray-500">
          Enter the checkout ID. You can find this from the overdue list or from the
          response when a book was checked out.
        </p>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Checkout ID</label>
          <input
            type="number"
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="e.g. 1"
            value={checkoutId}
            onChange={e => setCheckoutId(e.target.value)}
          />
        </div>
        <button
          className="w-full bg-blue-700 text-white px-4 py-2 rounded font-medium hover:bg-blue-800 disabled:opacity-50"
          onClick={() => mutation.mutate()}
          disabled={!checkoutId || mutation.isPending}
        >
          {mutation.isPending ? 'Returning…' : 'Return Book'}
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
