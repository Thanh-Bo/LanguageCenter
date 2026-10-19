import { RouterProvider } from 'react-router'
import { NhaCungCapToast } from './components/ui/toast'
import { router } from './tuyenDuong'

export default function App() {
  return (
    <NhaCungCapToast>
      <RouterProvider router={router} />
    </NhaCungCapToast>
  )
}
