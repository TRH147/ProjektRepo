<template>
  <header class="header">
    <img :src="logoImg" alt="Logo" class="logo" />

    <div class="hamburger" @click="toggleMobile" :class="{ active: mobileActive }">
      <span></span>
      <span></span>
      <span></span>
    </div>

    <nav :class="['navbar', { active: mobileActive }]" @click="handleNavClick">
      <router-link to="/" @click="closeMobile">Főoldal</router-link>
      <router-link to="/update" @click="closeMobile">Újdonságok</router-link>

      <a v-if="store.user" href="#" @click.prevent="checkAuth('/statics')">Statisztika</a>
      <a v-if="store.user" href="#" @click.prevent="checkAuth('/forum')">Fórum</a>

      <button v-if="!store.user" class="btnLogin-popup" @click="openAuth('login')">
        Bejelentkezés
      </button>

      <div v-else class="user-wrapper">
        <img 
          :src="profileImageUrl"
          @error="handleImageError"
          alt="user" 
          class="user-pic" 
          @click="toggleMenu" />

        <div class="user-menu" :class="{ active: menuActive, mobile: mobileActive }">
          <router-link to="/profile" @click="closeAll">Profilom</router-link>
          <a href="#" @click.prevent="logout">Kijelentkezés</a>
        </div>
      </div>
    </nav>
  </header>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import { useUserStore } from '../stores/user'
import { useRouter } from 'vue-router'
import logoImg from '/src/assets/Logo.png'

const store = useUserStore()
const router = useRouter()

const mobileActive = ref(false)
const menuActive = ref(false)
const imageError = ref(false)

const profileImageUrl = computed(() => {
  if (imageError.value) return '/src/assets/userrrr.png'
  
  if (store.user?.profileImages) {
    return `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${store.user.profileImages}`
  }
  
  return '/src/assets/userrrr.png'
})

watch(() => store.user, async (newUser) => {
  imageError.value = false
  if (newUser && !newUser.isAdmin) {
    await store.fetchProfileImage()
  }
}, { immediate: true })

function toggleMobile() {
  mobileActive.value = !mobileActive.value
  if (mobileActive.value) {
    document.body.classList.add('nav-open')
  } else {
    document.body.classList.remove('nav-open')
  }
}

function closeMobile() {
  mobileActive.value = false
  document.body.classList.remove('nav-open')
}

function toggleMenu() {
  menuActive.value = !menuActive.value
}

function closeAll() {
  mobileActive.value = false
  menuActive.value = false
  document.body.classList.remove('nav-open')
}

function handleNavClick(e) {
  if (mobileActive.value && !e.target.closest('.user-wrapper')) {
    closeMobile()
  }
}

function handleImageError() {
  imageError.value = true
}

function openAuth(tab = 'login') {
  store.openAuth(tab)
  closeMobile()
}

function logout() {
  store.logout()
  menuActive.value = false
  closeMobile()
}

function checkAuth(path) {
  if (!store.user) {
    alert('Bejelentkezés szükséges')
    openAuth('login')
  } else {
    router.push(path)
    closeMobile()
  }
}

function handleClickOutside(e) {
  if (menuActive.value && !e.target.closest('.user-wrapper')) {
    menuActive.value = false
  }
}

function handleEscapeKey(e) {
  if (e.key === 'Escape') {
    closeAll()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  document.addEventListener('keydown', handleEscapeKey)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  document.removeEventListener('keydown', handleEscapeKey)
  document.body.classList.remove('nav-open')
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

.header {
    position: absolute;
    width: 100%;
    padding: 0px 3%;
    background: transparent;
    display: flex;
    justify-content: space-between;
    align-items: center;
    z-index: 100;
}

.logo {
    width: clamp(60px, 8vw, 80px);
    height: auto;
    user-select: none;
}


.hamburger {
    display: none; 
    flex-direction: column;
    justify-content: space-between;
    width: 30px;
    height: 21px;
    cursor: pointer;
    z-index: 1001;
    position: relative;
    order: 3; 
    margin-left: auto; 
}

.hamburger span {
    height: 3px;
    width: 100%;
    background: #fae9d7;
    border-radius: 2px;
    transition: all 0.3s ease;
    transform-origin: center;
}


.hamburger.active span:nth-child(1) {
    transform: translateY(9px) rotate(45deg);
}

.hamburger.active span:nth-child(2) {
    opacity: 0;
    transform: scaleX(0);
}

.hamburger.active span:nth-child(3) {
    transform: translateY(-9px) rotate(-45deg);
}


.navbar {
    display: flex;
    align-items: center;
    gap: 20px;
    order: 2;
    margin-left: auto;
}

.navbar a {
    position: relative;
    font-size: clamp(14px, 1.2vw, 15px);
    color: #fae9d7;
    text-decoration: none;
    font-weight: 600;
    text-transform: uppercase;
    text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
    transition: color 0.3s ease;
    white-space: nowrap;
}

.navbar a:hover {
    color: #e24c4f;
}

.navbar a.active {
    color: #e24c4f;
}

.navbar a::after {
    content: '';
    position: absolute;
    left: 0;
    bottom: -6px;
    width: 100%;
    height: 2px;
    background: #e24c4f;
    border-radius: 5px;
    transform-origin: right;
    transform: scaleX(0);
    transition: transform 0.3s ease;
}

.navbar a:hover::after,
.navbar a.active::after {
    transform-origin: left;
    transform: scaleX(1);
}


.navbar .btnLogin-popup {
    min-width: 120px;
    height: 45px;
    background: transparent;
    border: 2px solid #fae9d7;
    outline: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: clamp(14px, 1.2vw, 15px);
    color: #fae9d7;
    font-weight: 600;
    text-transform: uppercase;
    text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
    transition: all 0.3s ease;
    padding: 0 15px;
    white-space: nowrap;
}

.navbar .btnLogin-popup:hover {
    background: #d55053;
    border-color: #d55053;
    color: #1c1c1c;
    transform: translateY(-2px);
}

.user-wrapper {
    position: relative;
    display: inline-block;
}

.user-pic {
    width: 45px;
    height: 45px;
    border-radius: 50%;
    object-fit: cover;
    object-position: center;
    cursor: pointer;
    border: 2px solid transparent;
    transition: border-color 0.3s ease, transform 0.3s ease;
}

.user-pic:hover {
    border-color: #d55053;
    transform: scale(1.05);
}

.user-menu {
    position: absolute;
    top: 55px;
    right: 0;
    background: #1c1c1c;
    border: 1px solid #fae9d7;
    border-radius: 8px;
    padding: 8px 0;
    width: 160px;
    display: none;
    flex-direction: column;
    z-index: 200;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.user-menu.active {
    display: flex;
    animation: fadeIn 0.3s ease;
}

.user-menu a {
    color: #fae9d7;
    text-decoration: none;
    padding: 12px 20px;
    font-size: 14px;
    transition: all 0.3s ease;
    text-align: left;
    text-shadow: none;
    border: none;
    background: transparent;
    width: 100%;
    text-transform: none;
    font-weight: 500;
}

.user-menu a:hover {
    background: #d55053;
    color: #1c1c1c;
}

.user-menu a::after {
    display: none;
}

.user-menu.mobile {
    position: relative;
    top: auto;
    right: auto;
    margin-top: 10px;
    width: 100%;
    max-width: 200px;
    margin-left: auto;
    margin-right: auto;
    border: none;
    background: rgba(20, 20, 20, 0.95);
    backdrop-filter: blur(10px);
}

@media (max-width: 1024px) {
    .header {
        padding: 10px 3%;
    }
    
    .navbar {
        gap: 15px;
    }
}

@media (max-width: 850px) {
    .hamburger {
        display: flex; 
        order: 2; 
        margin-left: 0; 
    }

    .navbar {
        order: 3; 
        margin-left: 0; 
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100vh;
        backdrop-filter: blur(10px);
        flex-direction: column;
        justify-content: flex-start;
        padding-top: 100px;
        gap: 25px;
        transform: translateX(100%);
        transition: transform 0.4s cubic-bezier(0.4, 0, 0.2, 1);
        z-index: 1000;
        overflow-y: auto;
        display: none;
    }

    .navbar.active {
        display: flex; 
        transform: translateX(0);
    }

    .header > .navbar:not(.active) a:not(.user-menu a) {
        display: none;
    }
    
    .header > .navbar:not(.active) .btnLogin-popup {
        display: none;
    }
    
    .header > .navbar:not(.active) .user-wrapper .user-pic {
        display: none;
    }

    .navbar.active a:not(.user-menu a) {
        display: block;
        font-size: 18px;
        padding: 10px 20px;
        width: 80%;
        text-align: center;
        margin: 0 auto;
    }
    
    .navbar.active .btnLogin-popup {
        display: block;
        width: 80%;
        height: 50px;
        font-size: 16px;
        margin-top: 10px;
    }
    
    .navbar.active .user-wrapper .user-pic {
        display: block;
        width: 60px;
        height: 60px;
        margin-bottom: 10px;
    }
    
    .user-menu.mobile {
        position: relative;
        margin-top: 10px;
        width: 100%;
        max-width: none;
    }
    
    body.nav-open {
        overflow: hidden;
    }
}

@media (max-width: 768px) {
    .navbar.active a:not(.user-menu a) {
        width: 90%;
        font-size: 16px;
        padding: 12px 15px;
    }
}

@media (max-width: 480px) {
    .header {
        padding: 8px 4%;
    }
    
    .logo {
        width: 50px;
        order: 1; 
    }
    
    .hamburger {
        order: 2; 
    }
    
    .navbar.active {
        padding-top: 80px;
    }
    
    .navbar.active a:not(.user-menu a) {
        font-size: 16px;
        padding: 12px;
        width: 85%;
    }
    
    .navbar.active .btnLogin-popup {
        height: 48px;
        font-size: 15px;
        width: 85%;
    }
    
    .navbar.active .user-pic {
        width: 50px;
        height: 50px;
    }
}

@media (max-height: 600px) and (orientation: landscape) and (max-width: 850px) {
    .navbar.active {
        padding-top: 70px;
        gap: 15px;
    }
    
    .navbar.active a:not(.user-menu a) {
        padding: 8px;
        font-size: 16px;
    }
    
    .navbar.active .user-pic {
        width: 45px;
        height: 45px;
    }
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@media (hover: none) and (pointer: coarse) and (max-width: 850px) {
    .navbar.active a:not(.user-menu a) {
        padding: 15px 20px;
        min-height: 44px;
    }
    
    .navbar.active .btnLogin-popup {
        min-height: 50px;
    }
    
    .navbar.active .user-pic {
        min-width: 50px;
        min-height: 50px;
    }
    
    .user-menu a {
        padding: 15px 20px;
    }
}
</style>