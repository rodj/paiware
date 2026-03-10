import { useQuery } from '@tanstack/react-query'
import { getDashboard } from '../api/checkouts'

interface StatCardProps { label: string; value: number; color: string }

function StatCard({ label, value, color }: StatCardProps) {
  return (
    <div className={`bg-white rounded-lg shadow p-6 border-l-4 ${color}`}>
      <p className="text-sm text-gray-500 uppercase tracking-wide">{label}</p>
      <p className="text-4xl font-bold text-gray-800 mt-1">{value}</p>
    </div>
  )
}

export default function DashboardPage() {
  const { data, isLoading, error } = useQuery({ queryKey: ['dashboard'], queryFn: getDashboard })

  if (isLoading) return <p className="text-gray-500">Loading…</p>
  if (error)     return <p className="text-red-600">Failed to load dashboard.</p>

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-800 mb-6">Dashboard</h1>
      <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
        <StatCard label="Total Books"       value={data!.totalBooks}       color="border-blue-500"  />
        <StatCard label="Available"         value={data!.availableBooks}   color="border-green-500" />
        <StatCard label="Checked Out"       value={data!.checkedOutBooks}  color="border-yellow-500"/>
        <StatCard label="Overdue"           value={data!.overdueCheckouts} color="border-red-500"   />
        <StatCard label="Total Members"     value={data!.totalMembers}     color="border-purple-500"/>
      </div>
    </div>
  )
}
