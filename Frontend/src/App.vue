<template>
  <div class="app" id="app">
    <SiteHeader v-if="$route.path === '/'" />
    
    <main>
      <router-view />
    </main>
    
    <SiteFooter v-if="$route.path === '/' && !showFullscreenNotification" />

    <div class="notification-container">
      <NotificationToast
        v-for="notification in notifications"
        :key="notification.id"
        :title="notification.title"
        :message="notification.message"
        :type="notification.type"
        :duration="notification.duration"
        @close="removeNotification(notification.id)"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch, provide } from 'vue'
import { useRoute } from 'vue-router'
import SiteHeader from './components/SiteHeader.vue'
import SiteFooter from './components/SiteFooter.vue'
import NotificationToast from './components/NotificationToast.vue'
import { useUserStore } from './stores/user'
import { notificationService } from './services/notificationService'

const route = useRoute()
const userStore = useUserStore()
let inactivityTimer

const notifications = ref([])
const showFullscreenNotification = ref(false)

const addNotification = (title, message, type = 'success', duration = 4000, persistent = false) => {
  console.log('📱 App.vue: Adding notification', { title, message, type, persistent })
  const id = Date.now()
  notifications.value.push({
    id,
    title,
    message,
    type,
    duration: persistent ? 0 : duration, 
    persistent 
  })

  if (!persistent && duration > 0) {
    setTimeout(() => {
      removeNotification(id)
    }, duration)
  }
  
  return true
}

const removeNotification = (id) => {
  const index = notifications.value.findIndex(n => n.id === id)
  if (index !== -1) {
    notifications.value.splice(index, 1)
  }
}

provide('notifications', {
  addNotification,
  removeNotification
})

function startInactivityTimer() {
  window.addEventListener('mousemove', resetInactivityTimer)
  window.addEventListener('keydown', resetInactivityTimer)
  window.addEventListener('click', resetInactivityTimer)
  window.addEventListener('scroll', resetInactivityTimer)
  resetInactivityTimer()
}

function stopInactivityTimer() {
  clearTimeout(inactivityTimer)
  window.removeEventListener('mousemove', resetInactivityTimer)
  window.removeEventListener('keydown', resetInactivityTimer)
  window.removeEventListener('click', resetInactivityTimer)
  window.removeEventListener('scroll', resetInactivityTimer)
}

function resetInactivityTimer() {
  if (!userStore.user) return

  clearTimeout(inactivityTimer)
  inactivityTimer = setTimeout(() => {
    userStore.logoutSessionExpired()
  }, 30 * 1000) // 30 seconds
}

watch(
  () => userStore.user,
  (newUser, oldUser) => {
    if (newUser && !oldUser) {
      console.log('👤 User logged in:', newUser.username)
    } else if (!newUser && oldUser) {
      console.log('👤 User logged out')
    }
  }
)

const handleThreadCreated = (event) => {
  if (event.data.type === 'THREAD_CREATED') {
    addNotification(
      'Téma létrehozva',
      'Az új téma sikeresen létrehozásra került!',
      'success',
      4000
    )
  }
}

onMounted(() => {
  const sessionExpired = sessionStorage.getItem('sessionExpired')
  if (sessionExpired === 'true') {
    notificationService.warning(
      'Lejárt Munkamenet',
      'Inaktivitás miatt kijelentkeztél',
      10000
    )
    sessionStorage.removeItem('sessionExpired')
  }
})

onMounted(() => {
  console.log('🔗 App.vue: Registering with notification service')
  notificationService.register(addNotification)
  
  userStore.loadUserFromStorage()
  window.addEventListener('message', handleThreadCreated)
})

onBeforeUnmount(() => {
  console.log('🔗 App.vue: Unregistering from notification service')
  notificationService.unregister(addNotification)
  stopInactivityTimer()
  window.removeEventListener('message', handleThreadCreated)
})

watch(
  () => userStore.user,
  (newVal) => {
    if (newVal) {
      startInactivityTimer()
    } else {
      stopInactivityTimer()  
    }
  },
  { immediate: true }
)
</script>

<style>
body.forgot-password-bg {
    background: url('/src/assets/picture.webp') no-repeat;
    background-size: cover;
    background-position: center;
}

.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

main {
  flex: 1;
}

.notification-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 99999; 
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 350px;
  pointer-events: none;
}

.notification-container > * {
  pointer-events: auto;
}
</style>