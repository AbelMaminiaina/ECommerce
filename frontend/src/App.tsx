import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import Home from './pages/Home'
import Products from './pages/Products'
import ProductDetail from './pages/ProductDetail'
import Cart from './pages/Cart'
import Checkout from './pages/Checkout'
import Login from './pages/Login'
import Register from './pages/Register'
import Profile from './pages/Profile'
import Orders from './pages/Orders'
import Support from './pages/Support'
import SupportTicketDetail from './pages/SupportTicketDetail'
import AdminDashboard from './pages/admin/Dashboard'
import AdminProducts from './pages/admin/Products'
import AdminOrders from './pages/admin/Orders'
import AdminUsers from './pages/admin/Users'
import AdminReturns from './pages/admin/Returns'
import AdminSupport from './pages/admin/Support'
import AdminWarranty from './pages/admin/Warranty'
import AdminShipping from './pages/admin/Shipping'
import PrivateRoute from './components/PrivateRoute'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Home />} />
        <Route path="products" element={<Products />} />
        <Route path="products/:id" element={<ProductDetail />} />
        <Route path="cart" element={<Cart />} />
        <Route path="checkout" element={<PrivateRoute><Checkout /></PrivateRoute>} />
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="profile" element={<PrivateRoute><Profile /></PrivateRoute>} />
        <Route path="orders" element={<PrivateRoute><Orders /></PrivateRoute>} />

        {/* Support Routes */}
        <Route path="support" element={<PrivateRoute><Support /></PrivateRoute>} />
        <Route path="support/:id" element={<PrivateRoute><SupportTicketDetail /></PrivateRoute>} />

        {/* Admin Routes */}
        <Route path="admin" element={<PrivateRoute adminOnly><AdminDashboard /></PrivateRoute>} />
        <Route path="admin/products" element={<PrivateRoute adminOnly><AdminProducts /></PrivateRoute>} />
        <Route path="admin/orders" element={<PrivateRoute adminOnly><AdminOrders /></PrivateRoute>} />
        <Route path="admin/users" element={<PrivateRoute adminOnly><AdminUsers /></PrivateRoute>} />

        {/* Legal Compliance Admin Routes */}
        <Route path="admin/returns" element={<PrivateRoute adminOnly><AdminReturns /></PrivateRoute>} />
        <Route path="admin/support" element={<PrivateRoute adminOnly><AdminSupport /></PrivateRoute>} />
        <Route path="admin/warranty" element={<PrivateRoute adminOnly><AdminWarranty /></PrivateRoute>} />
        <Route path="admin/shipping" element={<PrivateRoute adminOnly><AdminShipping /></PrivateRoute>} />
      </Route>
    </Routes>
  )
}

export default App
