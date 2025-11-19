import { useParams } from 'react-router-dom'
import { Typography, Box, CircularProgress } from '@mui/material'
import FormRenderer from '@formxchange/form-runtime'

export default function FormViewPage() {
  const { formId } = useParams<{ formId: string }>()

  if (!formId) {
    return (
      <Box>
        <Typography variant="h4" color="error">
          Form ID is required
        </Typography>
      </Box>
    )
  }

  const handleSubmit = (data: unknown) => {
    console.log('Form submitted:', data)
    // TODO: Submit form data to backend
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        Form View
      </Typography>
      <FormRenderer formId={formId} onSubmit={handleSubmit} />
    </Box>
  )
}




