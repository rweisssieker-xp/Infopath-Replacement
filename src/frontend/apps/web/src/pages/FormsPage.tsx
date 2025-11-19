import { useState, useEffect } from 'react'
import { Typography, Box, Button, CircularProgress } from '@mui/material'
import { useNavigate } from 'react-router-dom'

export default function FormsPage() {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Simulate loading
    setTimeout(() => setLoading(false), 500)
  }, [])

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4" component="h1">
          Forms
        </Typography>
        <Button
          variant="contained"
          onClick={() => navigate('/forms/create')}
        >
          Create New Form (AI)
        </Button>
      </Box>

      <Typography variant="body1" color="text.secondary">
        Your forms will appear here. Use the AI Form Designer to create new forms through natural language conversation.
      </Typography>
    </Box>
  )
}




