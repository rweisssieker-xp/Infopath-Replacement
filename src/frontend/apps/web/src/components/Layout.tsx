import { ReactNode } from 'react'
import { AppBar, Toolbar, Typography, Button, Box } from '@mui/material'
import { Link, useNavigate } from 'react-router-dom'

interface LayoutProps {
  children: ReactNode
}

export default function Layout({ children }: LayoutProps) {
  const navigate = useNavigate()

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            FormXChange Suite
          </Typography>
          <Button color="inherit" component={Link} to="/">
            Home
          </Button>
          <Button color="inherit" component={Link} to="/forms">
            Forms
          </Button>
          <Button color="inherit" component={Link} to="/forms/create">
            Create Form (AI)
          </Button>
          <Button color="inherit" component={Link} to="/workflows/create">
            Create Workflow (AI)
          </Button>
        </Toolbar>
      </AppBar>
      <Box component="main">
        {children}
      </Box>
    </>
  )
}




