import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '../services/api'
import { notificationService } from '../services/notificationService'
import router from '../router' 

export const useUserStore = defineStore('user', () => {
  const isAuthModalOpen = ref(false)
  const activeTab = ref('login')
  const user = ref(null)
  const isLoading = ref(false)
  const isInitialized = ref(false)
  const profileImage = ref(null)
  const profileImageLoading = ref(false)

  const showNotification = (title, message, type = 'info', duration = 4000) => {
    return notificationService.show(title, message, type, duration)
  }

  const redirectToHomeIfNeeded = () => {
    const currentRoute = router.currentRoute.value
    if (currentRoute.path === '/') {
      return
    }
    router.push('/')
  }

  const testNotification = () => {
    const result = showNotification('Test', 'This is a test notification', 'error', 3000)
    return result
  }

  const setLogoutFlag = () => {    
    try {
      sessionStorage.setItem('loggedOutManually', 'true')
    } catch (e) {}
    
    try {
      localStorage.setItem('logout_flag', 'true')
    } catch (e) {}
    
    try {
      document.cookie = 'logged_out=true; path=/; max-age=300'
    } catch (e) {}
  }
  
  const checkLogoutFlag = () => {    
    const sessionFlag = sessionStorage.getItem('loggedOutManually')
    if (sessionFlag === 'true') {
      return true
    }
    
    const localFlag = localStorage.getItem('logout_flag')
    if (localFlag === 'true') {
      return true
    }
    
    const cookies = document.cookie.split(';')
    const cookieFlag = cookies.some(cookie => 
      cookie.trim().startsWith('logged_out=')
    )
    if (cookieFlag) {
      return true
    }
    
    return false
  }
  
  const clearLogoutFlags = () => {
    sessionStorage.removeItem('loggedOutManually')
    localStorage.removeItem('logout_flag')
    document.cookie = 'logged_out=; path=/; max-age=0'
  }

  const initialize = () => {
    if (isInitialized.value) return
    
    if (checkLogoutFlag()) {
      clearAllData()
      clearLogoutFlags()
      isInitialized.value = true
      return
    }
    
    loadUserFromStorage()
    isInitialized.value = true
  }

  const clearAllData = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('isAdmin')
    localStorage.removeItem('userData')
    
    user.value = null
    profileImage.value = null
    isAuthModalOpen.value = false
  }

  const openAuth = (tab = 'login') => {
    activeTab.value = tab
    isAuthModalOpen.value = true
  }
  
  const closeAuth = () => { isAuthModalOpen.value = false }

  const fetchProfileImage = async () => {
  if (!user.value) return null

  try {
    profileImageLoading.value = true
    const token = localStorage.getItem('token')

    const response = await api.get('/Users/profile', {
      headers: { Authorization: `Bearer ${token}` }
    })

    console.log('Profile data response:', response.data)

    if (response.data && response.data.profileImages) {
      user.value.profileImages = response.data.profileImages
      
      const userData = JSON.parse(localStorage.getItem('userData') || '{}')
      userData.profileImages = response.data.profileImages
      localStorage.setItem('userData', JSON.stringify(userData))
      
      return response.data.profileImages
    } else {
      user.value.profileImages = null
      return null
    }
  } catch (err) {
    console.error('Failed to fetch profile data:', err)
    user.value.profileImages = null
    return null
  } finally {
    profileImageLoading.value = false
  }
}

  const login = async (payload) => {
    try {
      isLoading.value = true
      
      if (payload.username.toLowerCase() === 'admin') {
        const response = await api.post('/AdminLogin/login', {
          username: payload.username,
          password: payload.password
        })

        if (response.data.token) {
          localStorage.setItem('token', response.data.token)
          localStorage.setItem('isAdmin', 'true')
        }

        const userData = {
          username: payload.username,
          isAdmin: true,
          ...response.data
        }
        
        localStorage.setItem('userData', JSON.stringify(userData))
        user.value = userData
        
        clearLogoutFlags()
        
        isAuthModalOpen.value = false
        isLoading.value = false

        showNotification(
          'Admin bejelentkezés sikeres',
          'Üdv újra az admin felületen!',
          'success',
          3000
        )
        
        return user.value
      } else {
        const response = await api.post('/Users/login', {
          Username: payload.username,
          Password: payload.password
        })

        if (!response.data.token) throw new Error('No token received')

        const userData = {
          ...response.data.user,
          isAdmin: false
        }
        localStorage.setItem('token', response.data.token)
        localStorage.removeItem('isAdmin')
        localStorage.setItem('userData', JSON.stringify(userData))
        user.value = userData
        
        clearLogoutFlags()
        
        isAuthModalOpen.value = false
        isLoading.value = false

        fetchProfileImage()

        showNotification(
          'Sikeres bejelentkezés',
          `Üdv újra, ${userData.username}!`,
          'success',
          3000
        )
        
        return user.value
      }

    } catch (err) {
      isLoading.value = false
      
      let errorTitle = 'Bejelentkezési hiba'
      let errorMessage = 'Ismeretlen hiba történt'

      if (err.response?.status === 401) {
        errorTitle = 'Hibás adatok'
        errorMessage = 'Hibás felhasználónév vagy jelszó'
      } else if (err.response?.status === 404) {
        errorTitle = 'Felhasználó nem található'
        errorMessage = 'A megadott felhasználónév nem létezik'
      } else if (err.response?.status === 400) {
        const apiMessage = err.response?.data?.message?.toLowerCase() || ''
        const apiError = err.response?.data?.error?.toLowerCase() || ''
        
        if (apiMessage.includes('username') || apiMessage.includes('felhasználónév') || 
            apiError.includes('username') || apiError.includes('felhasználónév')) {
          errorTitle = 'Hibás felhasználónév'
          errorMessage = 'A felhasználónév nem megfelelő'
        } else if (apiMessage.includes('password') || apiMessage.includes('jelszó') ||
                   apiError.includes('password') || apiError.includes('jelszó')) {
          errorTitle = 'Hibás jelszó'
          errorMessage = 'A megadott jelszó nem helyes'
        } else if (apiMessage.includes('invalid') || apiMessage.includes('érvénytelen') ||
                   apiError.includes('invalid') || apiError.includes('érvénytelen')) {
          errorTitle = 'Érvénytelen adatok'
          errorMessage = 'A megadott adatok formátuma nem megfelelő'
        } else {
          errorMessage = err.response?.data?.message || err.response?.data?.error || 'Hibás kérés'
        }
      } else if (err.response?.status === 429) {
        errorTitle = 'Túl sok próbálkozás'
        errorMessage = 'Próbálkozz újra később'
      } else if (err.response?.status === 500) {
        errorTitle = 'Szerver hiba'
        errorMessage = 'Kérlek próbáld újra később'
      } else if (err.response?.data?.message) {
        errorMessage = err.response.data.message
      } else if (err.response?.data?.error) {
        errorMessage = err.response.data.error
      }

      if (err.message === 'Network Error') {
        errorTitle = 'Hálózati hiba'
        errorMessage = 'Nem sikerült csatlakozni a szerverhez'
      }
      
      showNotification(
        errorTitle,
        errorMessage,
        'error',
        5000
      )
      
      throw err
    }
  }

  const register = async (payload) => {
    try {
      const response = await api.post('/Users/register', {
        Username: payload.username,
        Email: payload.email,
        Password: payload.password
      })

      showNotification(
        'Regisztráció sikeres',
        'Most már bejelentkezhetsz az új fiókoddal!',
        'success',
        4000
      )
      
      isAuthModalOpen.value = true
      activeTab.value = 'login'

      return response.data
    } catch (err) {
      let errorTitle = 'Regisztrációs hiba'
      let errorMessage = 'Ismeretlen hiba történt'

      if (err.response?.status === 409) {
        const apiMessage = err.response?.data?.message?.toLowerCase() || ''
        if (apiMessage.includes('username') || apiMessage.includes('felhasználónév')) {
          errorTitle = 'Felhasználónév foglalt'
          errorMessage = 'Ez a felhasználónév már használatban van'
        } else if (apiMessage.includes('email')) {
          errorTitle = 'Email cím foglalt'
          errorMessage = 'Ez az email cím már regisztrálva van'
        } else {
          errorTitle = 'Már létező fiók'
          errorMessage = 'Ez a felhasználó már regisztrálva van'
        }
      } else if (err.response?.status === 400) {
        const apiMessage = err.response?.data?.message?.toLowerCase() || ''
        if (apiMessage.includes('password') || apiMessage.includes('jelszó')) {
          errorTitle = 'Gyenge jelszó'
          errorMessage = 'A jelszónak erősebbnek kell lennie'
        } else if (apiMessage.includes('email')) {
          errorTitle = 'Érvénytelen email'
          errorMessage = 'Kérlek érvényes email címet adj meg'
        } else if (apiMessage.includes('username') || apiMessage.includes('felhasználónév')) {
          errorTitle = 'Érvénytelen felhasználónév'
          errorMessage = 'A felhasználónév nem megfelelő'
        }
      } else if (err.response?.data?.message) {
        errorMessage = err.response.data.message
      }

      showNotification(
        errorTitle,
        errorMessage,
        'error',
        5000
      )
      
      throw err
    }
  }

  const loadUserFromStorage = async () => {
    if (isLoading.value) return
    
    isLoading.value = true
    
    try {
      if (checkLogoutFlag()) {
        clearAllData()
        clearLogoutFlags()
        return
      }
      
      const token = localStorage.getItem('token')
      
      if (!token) {
        user.value = null
        redirectToHomeIfNeeded()
        return
      }

      const cachedUser = localStorage.getItem('userData')
      if (cachedUser) {
        try {
          user.value = JSON.parse(cachedUser)
          if (!user.value?.isAdmin) {
            fetchProfileImage()
          }
        } catch (e) {
          localStorage.removeItem('userData')
        }
      }

      try {
        await api.get('/Users/verify-token', {
          headers: { Authorization: `Bearer ${token}` }
        })
        
      } catch (err) {
        if (err.response?.status === 401 || err.response?.status === 403) {
          clearAllData()
          redirectToHomeIfNeeded()
        }
      }
      
    } finally {
      isLoading.value = false
    }
  }

  const logout = () => {
    showNotification(
      'Kijelentkezés',
      'Sikeresen kijelentkeztél',
      'success',
      3000
    )

    if (profileImage.value && profileImage.value.startsWith('blob:')) {
      URL.revokeObjectURL(profileImage.value)
    }
    profileImage.value = null
    
    setLogoutFlag()
    clearAllData()
    sessionStorage.clear()
    redirectToHomeIfNeeded()
    isAuthModalOpen.value = false
  }

  const logoutAndRedirect = () => {
    logout()
  }

  const logoutSessionExpired = () => {
    showNotification(
      'Lejárt Munkamenet',
      'Inaktivitás miatt kijelentkeztél. Kérlek jelentkezz be újra.',
      'warning',
      0
    )

    clearAllData()
    setLogoutFlag()
    sessionStorage.clear()
    redirectToHomeIfNeeded()
  }

  const isAdmin = () => {
    return user.value?.isAdmin || localStorage.getItem('isAdmin') === 'true'
  }

  return {
    isAuthModalOpen,
    activeTab,
    user,
    isLoading,
    isInitialized,
    profileImage,
    profileImageLoading,
    initialize,
    openAuth,
    closeAuth,
    login,
    register,
    loadUserFromStorage,
    logout, 
    logoutAndRedirect, 
    logoutSessionExpired,
    isAdmin,
    clearAllData,
    testNotification,
    fetchProfileImage
  }
})