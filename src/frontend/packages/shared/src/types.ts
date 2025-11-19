export interface Form {
  id: string
  name: string
  description?: string
  schema: string
  version: number
  status: string
  createdAt: string
  updatedAt?: string
}

export interface FormField {
  name: string
  type: string
  label: string
  required?: boolean
  validation?: Record<string, unknown>
}




