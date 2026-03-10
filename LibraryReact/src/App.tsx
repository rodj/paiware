import { HashRouter, Routes, Route } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import Layout from './components/Layout'
import BooksPage     from './pages/BooksPage'
import CheckoutPage  from './pages/CheckoutPage'
import ReturnPage    from './pages/ReturnPage'
import OverduePage   from './pages/OverduePage'
import DashboardPage from './pages/DashboardPage'

const queryClient = new QueryClient()

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <HashRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index            element={<BooksPage />}    />
            <Route path="checkout"  element={<CheckoutPage />} />
            <Route path="return"    element={<ReturnPage />}   />
            <Route path="overdue"   element={<OverduePage />}  />
            <Route path="dashboard" element={<DashboardPage />}/>
          </Route>
        </Routes>
      </HashRouter>
    </QueryClientProvider>
  )
}
