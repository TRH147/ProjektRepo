import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '../services/api'

export const useUserStore = defineStore('user', () => {
  const isAuthModalOpen = ref(false)
  const activeTab = ref('login')
  const user = ref(null)

  function openAuth(tab = 'login') {
    activeTab.value = tab
    isAuthModalOpen.value = true
  }
  function closeAuth() { isAuthModalOpen.value = false }

  async function login(payload) {
    try {
      const response = await api.post('/Users/login', {
        Email: payload.email,
        Password: payload.password
      })
      user.value = response.data
      isAuthModalOpen.value = false
      alert('Bejelentkezés sikeres!')
      return user.value
    } catch (err) {
      console.error('Login error:', err.response?.data || err.message)
      alert('Hiba a bejelentkezés során: ' + (err.response?.data?.message || err.message))
      throw err
    }
  }

  async function register(payload) {
    try {
      const response = await api.post('/Users/register', {
        Username: payload.username,
        Email: payload.email,
        Password: payload.password
      })
      user.value = response.data
      isAuthModalOpen.value = false
      alert('Regisztráció sikeres!')
      return user.value
    } catch (err) {
      console.error('Register error:', err.response?.data || err.message)
      alert('Hiba a regisztráció során: ' + (err.response?.data?.message || err.message))
      throw err
    }
  }

  return { isAuthModalOpen, activeTab, user, openAuth, closeAuth, login, register }
})