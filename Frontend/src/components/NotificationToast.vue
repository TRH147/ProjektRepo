<template>
  <transition name="slide-down">
    <div v-if="visible" class="notification-toast" :class="type" :style="toastStyle">
      <div class="notification-icon">
        <i :class="iconClass"></i>
      </div>
      <div class="notification-content">
        <div class="notification-title">{{ title }}</div>
        <div class="notification-message">{{ message }}</div>
      </div>
      <button class="notification-close" @click="handleClose" aria-label="Close notification">
        <i class="fas fa-times"></i>
      </button>
    </div>
  </transition>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const props = defineProps({
  title: {
    type: String,
    default: 'Értesítés'
  },
  message: {
    type: String,
    required: true
  },
  type: {
    type: String,
    default: 'success',
    validator: (value) => ['success', 'info', 'warning', 'error'].includes(value)
  },
  duration: {
    type: Number,
    default: 4000
  }
})

const emit = defineEmits(['close'])

const visible = ref(false)
let timer = null
let isClosing = false

const iconMap = {
  success: 'fas fa-check-circle',
  info: 'fas fa-info-circle',
  warning: 'fas fa-exclamation-triangle',
  error: 'fas fa-exclamation-circle'
}

const iconClass = iconMap[props.type]

const isPersistent = computed(() => props.type === 'warning')

const toastStyle = computed(() => ({
  '--duration': props.duration + 'ms'
}))

const show = () => {
  visible.value = true
}

const handleClose = () => {
  if (isClosing) return
  
  isClosing = true

  if (timer) {
    clearTimeout(timer)
    timer = null
  }

  const toastElement = document.querySelector('.notification-toast')
  if (toastElement) {
    toastElement.style.transition = 'none'
    toastElement.style.opacity = '0'
    toastElement.style.transform = 'translateX(-50%) translateY(-100%)'
  }

  visible.value = false

  emit('close')

  setTimeout(() => {
    isClosing = false
  }, 100)
}

onMounted(() => {
  show()

  if (!isPersistent.value && props.duration > 0) {
    timer = setTimeout(() => {
      handleClose()
    }, props.duration)
  }

  return () => {
    if (timer) {
      clearTimeout(timer)
    }
  }
})
</script>

<style scoped>
.notification-toast {
  position: fixed;
  top: 20px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  background: #2c2c2c;
  border-radius: 10px;
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 4px 10px rgba(213, 80, 83, 0.2);
  padding: 16px;
  min-width: 300px;
  max-width: 400px;
  display: flex;
  align-items: center;
  gap: 12px;
  border-left: 4px solid #d55053;
  pointer-events: auto;
  border: 1px solid #444;
  backdrop-filter: blur(10px);
  transition: opacity 0.15s ease-out, transform 0.15s ease-out;
}

.notification-toast.success {
  border-left-color: #F44336; 
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 4px 10px rgba(244, 67, 54, 0.2); 
}

.notification-toast.info {
  border-left-color: #F44336;
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 4px 10px rgba(33, 150, 243, 0.2);
}

.notification-toast.warning {
  border-left-color: #FF9800;
  border-left-width: 5px;
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 4px 10px rgba(255, 152, 0, 0.2),
    0 0 15px rgba(255, 152, 0, 0.3);
  animation: pulse-border 2s infinite;
}

@keyframes pulse-border {
  0% { 
    border-left-color: #FF9800;
    box-shadow: 
      0 8px 25px rgba(0, 0, 0, 0.5),
      0 4px 10px rgba(255, 152, 0, 0.2),
      0 0 15px rgba(255, 152, 0, 0.3);
  }
  50% { 
    border-left-color: #ffb74d;
    box-shadow: 
      0 8px 25px rgba(0, 0, 0, 0.5),
      0 4px 15px rgba(255, 152, 0, 0.3),
      0 0 20px rgba(255, 152, 0, 0.4);
  }
  100% { 
    border-left-color: #FF9800;
    box-shadow: 
      0 8px 25px rgba(0, 0, 0, 0.5),
      0 4px 10px rgba(255, 152, 0, 0.2),
      0 0 15px rgba(255, 152, 0, 0.3);
  }
}

.notification-toast.error {
  border-left-color: #F44336;
  box-shadow: 
    0 8px 25px rgba(0, 0, 0, 0.5),
    0 4px 10px rgba(244, 67, 54, 0.2);
}

.notification-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
}

.notification-toast.success .notification-icon {
  color: #F44336;
}

.notification-toast.info .notification-icon {
  color: #F44336;
}

.notification-toast.warning .notification-icon {
  color: #FF9800;
  animation: pulse-icon 1.5s infinite;
}

@keyframes pulse-icon {
  0% { transform: scale(1); }
  50% { transform: scale(1.1); }
  100% { transform: scale(1); }
}

.notification-toast.error .notification-icon {
  color: #F44336;
}

.notification-content {
  flex: 1;
  min-width: 0;
}

.notification-title {
  font-weight: 700;
  font-size: 1.05rem;
  margin-bottom: 4px;
  color: #fae9d7;
  letter-spacing: 0.3px;
  text-transform: uppercase;
  font-family: 'Belleza', sans-serif;
}

.notification-toast.warning .notification-title {
  color: #FF9800;
  text-shadow: 0 0 5px rgba(255, 152, 0, 0.3);
}

.notification-message {
  font-size: 0.9rem;
  color: #ddd;
  line-height: 1.4;
  font-family: 'Belleza', sans-serif;
}

.notification-close {
  background: rgba(255, 255, 255, 0.1);
  border: none;
  color: #aaa;
  cursor: pointer;
  padding: 6px;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s ease;
  border-radius: 50%;
  flex-shrink: 0;
}

.notification-close:hover {
  background: rgba(213, 80, 83, 0.2);
  color: #fae9d7;
  transform: scale(1.1);
}

.notification-toast.warning .notification-close:hover {
  background: rgba(255, 152, 0, 0.3);
  color: #FF9800;
}

.slide-down-enter-active {
  animation: slideDown 0.3s ease-out;
}

.slide-down-leave-active {
  animation: none !important;
  display: none !important;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateX(-50%) translateY(-100%);
  }
  to {
    opacity: 1;
    transform: translateX(-50%) translateY(0);
  }
}

.notification-toast:not(.warning)::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  background: currentColor;
  opacity: 0.3;
  animation: progressBar var(--duration) linear forwards;
  border-radius: 0 0 0 6px;
}

@keyframes progressBar {
  from {
    width: 100%;
  }
  to {
    width: 0%;
  }
}

@media (max-width: 768px) {
  .notification-toast {
    min-width: auto;
    max-width: calc(100vw - 40px);
    border-radius: 8px;
    padding: 14px;
  }
  
  .notification-icon {
    width: 28px;
    height: 28px;
    font-size: 1.3rem;
  }
  
  .notification-title {
    font-size: 1rem;
  }
  
  .notification-message {
    font-size: 0.85rem;
  }
}

@media (max-width: 480px) {
  .notification-toast {
    max-width: calc(100vw - 20px);
    padding: 12px;
    top: 10px;
  }
  
  .notification-title {
    font-size: 0.95rem;
  }
}
</style>