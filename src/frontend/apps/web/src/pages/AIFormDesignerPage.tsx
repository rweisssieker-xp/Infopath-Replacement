import { useState } from 'react'
import { Typography, Box, Paper } from '@mui/material'
import AIFormDesigner from '@formxchange/ai-form-designer'

export default function AIFormDesignerPage() {
  const [sessionId] = useState(() => crypto.randomUUID())

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        AI Form Designer
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Create forms through natural language conversation. Describe what you need, and AI will generate a complete form schema.
      </Typography>

      <Paper sx={{ mt: 3, p: 3 }}>
        <AIFormDesigner
          sessionId={sessionId}
          userId="00000000-0000-0000-0000-000000000001"
          tenantId="00000000-0000-0000-0000-000000000001"
        />
      </Paper>
    </Box>
  )
}




