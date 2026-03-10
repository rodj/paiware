import { NavLink, Outlet } from 'react-router-dom'

const navLinks = [
  { to: '/',          label: 'Books'     },
  { to: '/checkout',  label: 'Check Out' },
  { to: '/return',    label: 'Return'    },
  { to: '/overdue',   label: 'Overdue'   },
  { to: '/dashboard', label: 'Dashboard' },
]

export default function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-blue-800 text-white shadow-md">
        <div className="max-w-5xl mx-auto px-4 py-3 flex items-center gap-8">
          <span className="text-xl font-bold tracking-wide">📚 Library</span>
          <div className="flex gap-4">
            {navLinks.map(({ to, label }) => (
              <NavLink
                key={to}
                to={to}
                end={to === '/'}
                className={({ isActive }) =>
                  `px-3 py-1 rounded text-sm font-medium transition-colors ${
                    isActive ? 'bg-white text-blue-800' : 'hover:bg-blue-700'
                  }`
                }
              >
                {label}
              </NavLink>
            ))}
          </div>
        </div>
      </nav>
      <main className="max-w-5xl mx-auto px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}
