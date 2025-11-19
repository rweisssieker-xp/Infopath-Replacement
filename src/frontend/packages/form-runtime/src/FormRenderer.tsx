import { useState, useEffect } from 'react'
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  FormControlLabel,
  Checkbox,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  CircularProgress,
  Alert,
} from '@mui/material'
import { useForm, Controller } from 'react-hook-form'
import { useQuery, useMutation } from '@tanstack/react-query'

interface FormRendererProps {
  formId?: string
  schema?: string
  onSubmit?: (data: unknown) => void
}

interface FormField {
  name: string
  type: string
  label: string
  required: boolean
  properties?: Record<string, unknown>
}

interface FormRenderResult {
  success: boolean
  schema: string
  fields: FormField[]
  errorMessage?: string
}

export default function FormRenderer({ formId, schema, onSubmit }: FormRendererProps) {
  const [formData, setFormData] = useState<Record<string, unknown>>({})
  const { control, handleSubmit, formState: { errors } } = useForm()

  // Fetch form schema if formId is provided
  const { data: renderResult, isLoading } = useQuery<FormRenderResult>({
    queryKey: ['form-render', formId],
    queryFn: async () => {
      if (formId) {
        const response = await fetch(`/api/runtime/forms/${formId}/render`)
        if (!response.ok) throw new Error('Failed to load form')
        return response.json()
      } else if (schema) {
        const response = await fetch('/api/runtime/forms/render-from-schema', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ schema }),
        })
        if (!response.ok) throw new Error('Failed to render form')
        return response.json()
      }
      throw new Error('Either formId or schema must be provided')
    },
    enabled: !!formId || !!schema,
  })

  const validateMutation = useMutation({
    mutationFn: async (data: Record<string, unknown>) => {
      if (!formId) return { isValid: true, errors: [] }
      
      const response = await fetch(`/api/runtime/forms/${formId}/validate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ formData: JSON.stringify(data) }),
      })
      return response.json()
    },
  })

  const onSubmitForm = async (data: Record<string, unknown>) => {
    const validation = await validateMutation.mutateAsync(data)
    
    if (validation.isValid) {
      setFormData(data)
      if (onSubmit) {
        onSubmit(data)
      }
    } else {
      console.error('Validation errors:', validation.errors)
    }
  }

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={3}>
        <CircularProgress />
      </Box>
    )
  }

  if (!renderResult?.success) {
    return (
      <Alert severity="error">
        {renderResult?.errorMessage || 'Failed to load form'}
      </Alert>
    )
  }

  const renderField = (field: FormField) => {
    const fieldName = field.name

    switch (field.type) {
      case 'string':
      case 'text':
        return (
          <Controller
            key={fieldName}
            name={fieldName}
            control={control}
            rules={{ required: field.required }}
            render={({ field: controllerField }) => (
              <TextField
                {...controllerField}
                fullWidth
                label={field.label}
                required={field.required}
                error={!!errors[fieldName]}
                helperText={errors[fieldName]?.message as string}
                multiline={field.type === 'text'}
                rows={field.type === 'text' ? 4 : 1}
                margin="normal"
              />
            )}
          />
        )

      case 'number':
      case 'integer':
        return (
          <Controller
            key={fieldName}
            name={fieldName}
            control={control}
            rules={{ required: field.required }}
            render={({ field: controllerField }) => (
              <TextField
                {...controllerField}
                fullWidth
                type="number"
                label={field.label}
                required={field.required}
                error={!!errors[fieldName]}
                helperText={errors[fieldName]?.message as string}
                margin="normal"
              />
            )}
          />
        )

      case 'boolean':
        return (
          <Controller
            key={fieldName}
            name={fieldName}
            control={control}
            render={({ field: controllerField }) => (
              <FormControlLabel
                control={
                  <Checkbox
                    {...controllerField}
                    checked={controllerField.value || false}
                  />
                }
                label={field.label}
              />
            )}
          />
        )

      case 'select':
      case 'enum':
        return (
          <Controller
            key={fieldName}
            name={fieldName}
            control={control}
            rules={{ required: field.required }}
            render={({ field: controllerField }) => (
              <FormControl fullWidth margin="normal" required={field.required}>
                <InputLabel>{field.label}</InputLabel>
                <Select
                  {...controllerField}
                  label={field.label}
                  error={!!errors[fieldName]}
                >
                  {Array.isArray(field.properties?.enum) &&
                    field.properties.enum.map((option: unknown) => (
                      <MenuItem key={String(option)} value={String(option)}>
                        {String(option)}
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            )}
          />
        )

      default:
        return (
          <Controller
            key={fieldName}
            name={fieldName}
            control={control}
            rules={{ required: field.required }}
            render={({ field: controllerField }) => (
              <TextField
                {...controllerField}
                fullWidth
                label={field.label}
                required={field.required}
                error={!!errors[fieldName]}
                helperText={errors[fieldName]?.message as string}
                margin="normal"
              />
            )}
          />
        )
    }
  }

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Form
      </Typography>

      <form onSubmit={handleSubmit(onSubmitForm)}>
        {renderResult.fields.map((field) => (
          <Box key={field.name}>{renderField(field)}</Box>
        ))}

        {validateMutation.data && !validateMutation.data.isValid && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {validateMutation.data.errors?.map((error: string, index: number) => (
              <div key={index}>{error}</div>
            ))}
          </Alert>
        )}

        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <Button
            type="submit"
            variant="contained"
            disabled={validateMutation.isPending}
          >
            Submit
          </Button>
          <Button type="button" variant="outlined" onClick={() => setFormData({})}>
            Reset
          </Button>
        </Box>
      </form>
    </Paper>
  )
}




