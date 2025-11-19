import { useState, useRef, useEffect } from 'react'
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  List,
  ListItem,
  ListItemText,
  CircularProgress,
  Avatar,
} from '@mui/material'
import SendIcon from '@mui/icons-material/Send'
import SmartToyIcon from '@mui/icons-material/SmartToy'
import PersonIcon from '@mui/icons-material/Person'
import { useMutation } from '@tanstack/react-query'

interface AIWorkflowDesignerProps {
  sessionId: string
  userId: string
  tenantId: string
  formId?: string
}

interface Message {
  id: string
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
}

export default function AIWorkflowDesigner({ sessionId, userId, tenantId, formId }: AIWorkflowDesignerProps) {
  const [messages, setMessages] = useState<Message[]>([])
  const [input, setInput] = useState('')
  const messagesEndRef = useRef<HTMLDivElement>(null)

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  useEffect(() => {
    scrollToBottom()
  }, [messages])

  // Initial welcome message
  useEffect(() => {
    setMessages([
      {
        id: '1',
        role: 'assistant',
        content: 'Hello! I\'m your AI Workflow Designer. Describe the workflow you want to create, and I\'ll generate it for you. For example: "Create an approval workflow for purchase orders over $10,000 that requires manager approval and sends a notification to the requester."',
        timestamp: new Date(),
      },
    ])
  }, [])

  const generateWorkflowMutation = useMutation({
    mutationFn: async (message: string) => {
      // TODO: Implement workflow generation API
      const response = await fetch('/api/ai/workflows/conversation', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          sessionId,
          userId,
          tenantId,
          message,
          formId,
        }),
      })

      if (!response.ok) {
        throw new Error('Failed to generate workflow')
      }

      return response.json()
    },
    onSuccess: (data) => {
      setMessages((prev) => [
        ...prev,
        {
          id: Date.now().toString(),
          role: 'user',
          content: input,
          timestamp: new Date(),
        },
        {
          id: (Date.now() + 1).toString(),
          role: 'assistant',
          content: data.message || 'Workflow generated successfully!',
          timestamp: new Date(),
        },
      ])
      setInput('')
    },
    onError: (error) => {
      setMessages((prev) => [
        ...prev,
        {
          id: Date.now().toString(),
          role: 'assistant',
          content: `Error: ${error instanceof Error ? error.message : 'Failed to generate workflow'}`,
          timestamp: new Date(),
        },
      ])
    },
  })

  const handleSend = () => {
    if (!input.trim() || generateWorkflowMutation.isPending) return
    generateWorkflowMutation.mutate(input)
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '600px' }}>
      <Typography variant="h6" gutterBottom>
        Conversational Workflow Designer
      </Typography>

      <Paper
        sx={{
          flex: 1,
          overflow: 'auto',
          p: 2,
          mb: 2,
          backgroundColor: '#f5f5f5',
        }}
      >
        <List>
          {messages.map((message) => (
            <ListItem
              key={message.id}
              sx={{
                flexDirection: message.role === 'user' ? 'row-reverse' : 'row',
                alignItems: 'flex-start',
              }}
            >
              <Avatar
                sx={{
                  bgcolor: message.role === 'user' ? 'primary.main' : 'secondary.main',
                  mr: message.role === 'user' ? 0 : 1,
                  ml: message.role === 'user' ? 1 : 0,
                }}
              >
                {message.role === 'user' ? <PersonIcon /> : <SmartToyIcon />}
              </Avatar>
              <Paper
                sx={{
                  p: 2,
                  maxWidth: '70%',
                  bgcolor: message.role === 'user' ? 'primary.light' : 'white',
                  color: message.role === 'user' ? 'white' : 'text.primary',
                }}
              >
                <Typography variant="body1">{message.content}</Typography>
                <Typography variant="caption" sx={{ opacity: 0.7, display: 'block', mt: 1 }}>
                  {message.timestamp.toLocaleTimeString()}
                </Typography>
              </Paper>
            </ListItem>
          ))}
          {generateWorkflowMutation.isPending && (
            <ListItem>
              <Avatar sx={{ bgcolor: 'secondary.main', mr: 1 }}>
                <SmartToyIcon />
              </Avatar>
              <CircularProgress size={24} />
            </ListItem>
          )}
          <div ref={messagesEndRef} />
        </List>
      </Paper>

      <Box sx={{ display: 'flex', gap: 1 }}>
        <TextField
          fullWidth
          multiline
          maxRows={4}
          placeholder="Describe the workflow you want to create..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={handleKeyPress}
          disabled={generateWorkflowMutation.isPending}
        />
        <Button
          variant="contained"
          onClick={handleSend}
          disabled={!input.trim() || generateWorkflowMutation.isPending}
          startIcon={<SendIcon />}
        >
          Send
        </Button>
      </Box>
    </Box>
  )
}

