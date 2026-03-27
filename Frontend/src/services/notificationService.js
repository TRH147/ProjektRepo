class NotificationService {
  constructor() {
    this.callbacks = []
  }

  register(callback) {
    this.callbacks.push(callback)
    console.log('📝 Notification callback registered')
  }

  unregister(callback) {
    const index = this.callbacks.indexOf(callback)
    if (index !== -1) {
      this.callbacks.splice(index, 1)
    }
  }

  show(title, message, type = 'info', duration = 4000, persistent = false) {
  console.log('🔔 NotificationService.show:', { title, message, type, persistent })
  
  if (this.callbacks.length === 0) {
    console.warn('⚠️ No notification callbacks registered!')
    console.log(`[${type.toUpperCase()}] ${title}: ${message}`)
    return false
  }

  this.callbacks.forEach(callback => {
    try {
      callback(title, message, type, duration, persistent)
    } catch (error) {
      console.error('Error in notification callback:', error)
    }
  })
  
  return true
}

  success(title, message, duration = 4000) {
    return this.show(title, message, 'success', duration)
  }

  error(title, message, duration = 5000) {
    return this.show(title, message, 'error', duration)
  }

 warning(title, message, duration = 0, persistent = true) {
  return this.show(title, message, 'warning', duration, persistent)
}

  info(title, message, duration = 4000) {
    return this.show(title, message, 'info', duration)
  }
}

export const notificationService = new NotificationService()