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
    let response;

    // Admin login endpoint
    if (payload.username.toLowerCase() === 'admin') {
      response = await api.post('/AdminLogin/login', {
        username: payload.username,
        password: payload.password
      });

      user.value = response.data;
      alert('Admin bejelentkezés sikeres!');
      isAuthModalOpen.value = false;
      return { ...user.value, isAdmin: true }; // admin jelzés
    }

    // Normál felhasználó login
    response = await api.post('/Users/login', {
      Username: payload.username,
      Password: payload.password
    });

    if (response.data.token) {
      localStorage.setItem("token", response.data.token);
      console.log("TOKEN ELMENTVE:", response.data.token);
    }

    // --- Itt kérjük le a teljes felhasználó adatokat, így a profilkép is ---
    const userData = await api.get('/Users/users/me', {
  headers: { Authorization: `Bearer ${response.data.token}` }
});
user.value = userData.data;

// Ha a backend csak a relative path-et adja vissza:
if (user.value.image) {
  user.value.image = `http://localhost:7030${user.value.image}`; // backend URL előtag
}

    isAuthModalOpen.value = false;
    alert('Bejelentkezés sikeres!');
    return { ...user.value, isAdmin: false };
  } catch (err) {
    console.error('Login error:', err.response?.data || err.message);
    alert('Hiba a bejelentkezés során: ' + (err.response?.data?.message || err.message));
    throw err;
  }
}

  async function register(payload) {
  try {
    const response = await api.post('/Users/register', {
      Username: payload.username,
      Email: payload.email,
      Password: payload.password
    })

    alert('Regisztráció sikeres!')

    isAuthModalOpen.value = true
    activeTab.value = 'login'

    return response.data
  } catch (err) {
    console.error('Register error:', err.response?.data || err.message)
    alert('Hiba a regisztráció során: ' + (err.response?.data?.message || err.message))
    throw err
  }
}


  return { isAuthModalOpen, activeTab, user, openAuth, closeAuth, login, register }
})