import { useQuery } from '@tanstack/react-query'
import { getOverdue } from '../api/checkouts'

export default function OverduePage() {
  const { data, isLoading, error } = useQuery({ queryKey: ['overdue'], queryFn: getOverdue })

  if (isLoading) return <p className="text-gray-500">Loading…</p>
  if (error)     return <p className="text-red-600">Failed to load overdue checkouts.</p>

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-4">Overdue Checkouts</h1>
      {data?.length === 0 ? (
        <p className="text-green-700 bg-green-50 rounded p-4">No overdue checkouts. 🎉</p>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-red-50 text-red-700 uppercase text-xs">
              <tr>
                <th className="text-left px-4 py-3">ID</th>
                <th className="text-left px-4 py-3">Book</th>
                <th className="text-left px-4 py-3">Member</th>
                <th className="text-left px-4 py-3">Checked Out</th>
                <th className="text-left px-4 py-3">Due Date</th>
                <th className="text-left px-4 py-3">Days Overdue</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {data?.map(c => {
                const daysOverdue = Math.floor(
                  (Date.now() - new Date(c.dueDate).getTime()) / 86400000
                )
                return (
                  <tr key={c.id} className="hover:bg-red-50">
                    <td className="px-4 py-3 text-gray-400">{c.id}</td>
                    <td className="px-4 py-3 font-medium text-gray-800">{c.bookTitle}</td>
                    <td className="px-4 py-3 text-gray-600">{c.memberName}</td>
                    <td className="px-4 py-3 text-gray-500">{new Date(c.checkedOutAt).toLocaleDateString()}</td>
                    <td className="px-4 py-3 text-red-600">{new Date(c.dueDate).toLocaleDateString()}</td>
                    <td className="px-4 py-3">
                      <span className="bg-red-100 text-red-700 px-2 py-0.5 rounded-full text-xs font-semibold">
                        {daysOverdue}d
                      </span>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
