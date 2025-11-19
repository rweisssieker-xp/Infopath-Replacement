import { Typography, Box, Paper, Grid, Card, CardContent, CardActions, Button } from '@mui/material'
import { useNavigate } from 'react-router-dom'

export default function HomePage() {
  const navigate = useNavigate()

  return (
    <Box>
      <Typography variant="h3" component="h1" gutterBottom>
        FormXChange Suite
      </Typography>
      <Typography variant="h5" color="text.secondary" paragraph>
        AI-First Form and Workflow Designer
      </Typography>

      <Grid container spacing={3} sx={{ mt: 2 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h5" component="h2" gutterBottom>
                AI Form Designer
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Create forms through natural language conversation. Describe what you need, and AI will generate a complete form schema.
              </Typography>
            </CardContent>
            <CardActions>
              <Button size="small" onClick={() => navigate('/forms/create')}>
                Create Form
              </Button>
            </CardActions>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h5" component="h2" gutterBottom>
                AI Workflow Designer
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Design approval workflows and business processes using natural language. AI analyzes your requirements and generates workflows automatically.
              </Typography>
            </CardContent>
            <CardActions>
              <Button size="small" onClick={() => navigate('/workflows/create')}>
                Create Workflow
              </Button>
            </CardActions>
          </Card>
        </Grid>
      </Grid>

      <Paper sx={{ mt: 4, p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Key Features
        </Typography>
        <ul>
          <li>Conversational Form Creation - Create forms through natural language</li>
          <li>AI-Generated Workflows - Automatic workflow generation from form analysis</li>
          <li>Git-like Versioning - Full version control with branching</li>
          <li>Schema-based Architecture - JSON Schema foundation</li>
          <li>Enterprise Integration - SharePoint, D365, SAP, and more</li>
        </ul>
      </Paper>
    </Box>
  )
}




