import { Routes, Route } from 'react-router-dom'
import { Container } from '@mui/material'
import Layout from './components/Layout'
import HomePage from './pages/HomePage'
import AIFormDesignerPage from './pages/AIFormDesignerPage'
import AIWorkflowDesignerPage from './pages/AIWorkflowDesignerPage'
import FormsPage from './pages/FormsPage'
import FormViewPage from './pages/FormViewPage'

function App() {
  return (
    <Layout>
      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/forms" element={<FormsPage />} />
          <Route path="/forms/create" element={<AIFormDesignerPage />} />
          <Route path="/forms/:formId" element={<FormViewPage />} />
          <Route path="/workflows/create" element={<AIWorkflowDesignerPage />} />
        </Routes>
      </Container>
    </Layout>
  )
}

export default App

