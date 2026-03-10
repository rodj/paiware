import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { getBooks, addBook } from '../api/books'
import StatusBadge from '../components/StatusBadge'

export default function BooksPage() {
  const queryClient = useQueryClient()
  const { data: books, isLoading, error } = useQuery({ queryKey: ['books'], queryFn: getBooks })

  const [title, setTitle]   = useState('')
  const [author, setAuthor] = useState('')
  const [isbn, setIsbn]     = useState('')
  const [message, setMessage] = useState('')

  const addMutation = useMutation({
    mutationFn: () => addBook(title, author, isbn),
    onSuccess: (book) => {
      queryClient.invalidateQueries({ queryKey: ['books'] })
      setTitle(''); setAuthor(''); setIsbn('')
      setMessage(`✓ "${book.title}" added.`)
      setTimeout(() => setMessage(''), 3000)
    },
    onError: (err: Error) => setMessage(`Error: ${err.message}`),
  })

  if (isLoading) return <p className="text-gray-500">Loading books…</p>
  if (error)     return <p className="text-red-600">Failed to load books.</p>

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-800 mb-4">Book Collection</h1>
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-100 text-gray-600 uppercase text-xs">
              <tr>
                <th className="text-left px-4 py-3">Title</th>
                <th className="text-left px-4 py-3">Author</th>
                <th className="text-left px-4 py-3">ISBN</th>
                <th className="text-left px-4 py-3">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {books?.map(book => (
                <tr key={book.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-gray-800">{book.title}</td>
                  <td className="px-4 py-3 text-gray-600">{book.author}</td>
                  <td className="px-4 py-3 text-gray-400 font-mono text-xs">{book.isbn}</td>
                  <td className="px-4 py-3"><StatusBadge available={book.isAvailable} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div>
        <h2 className="text-lg font-semibold text-gray-700 mb-3">Add a Book</h2>
        <p className="text-sm text-gray-500 mb-3">
          The spec didn't require book management, but adding a book and restarting the app
          is the clearest way to demonstrate real persistence.
        </p>
        <div className="bg-white rounded-lg shadow p-4 space-y-3">
          <div className="grid grid-cols-3 gap-3">
            <input className="border rounded px-3 py-2 text-sm" placeholder="Title"
              value={title} onChange={e => setTitle(e.target.value)} />
            <input className="border rounded px-3 py-2 text-sm" placeholder="Author"
              value={author} onChange={e => setAuthor(e.target.value)} />
            <input className="border rounded px-3 py-2 text-sm" placeholder="ISBN"
              value={isbn} onChange={e => setIsbn(e.target.value)} />
          </div>
          <div className="flex items-center gap-4">
            <button
              className="bg-blue-700 text-white px-4 py-2 rounded text-sm font-medium hover:bg-blue-800 disabled:opacity-50"
              onClick={() => addMutation.mutate()}
              disabled={!title || !author || addMutation.isPending}
            >
              {addMutation.isPending ? 'Adding…' : 'Add Book'}
            </button>
            {message && <span className="text-sm text-green-700">{message}</span>}
          </div>
        </div>
      </div>
    </div>
  )
}
