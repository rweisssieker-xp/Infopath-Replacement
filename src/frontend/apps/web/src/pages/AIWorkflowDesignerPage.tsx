import { useState } from 'react'
import { Typography, Box, Paper } from '@mui/material'
import AIWorkflowDesigner from '@formxchange/ai-workflow-designer'

export default function AIWorkflowDesignerPage() {
  const [sessionId] = useState(() => crypto.randomUUID())

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        AI Workflow Designer
      </Typography>
      <Typography variant="body1" color="text.secondary" paragraph>
        Design approval workflows and business processes using natural language. AI analyzes your requirements and generates workflows automatically.
      </Typography>

      <Paper sx={{ mt: 3, p: 3 }}>
        <AIWorkflowDesigner
          sessionId={sessionId}
          userId="00000000-0000-0000-0000-000000000001"
          tenantId="00000000-0000-0000-0000-000000000001"
        />
      </Paper>
    </Box>
  )
}




