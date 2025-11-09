<template>
  <div class="update-page">
    <header class="header">
      <h2 class="logo">Logo</h2>
      <div class="hamburger" @click="toggleHamburger">
        <span></span>
        <span></span>
        <span></span>
      </div>
      <nav :class="{ navbar: true, active: hamburgerActive }">
        <router-link to="/">Főoldal</router-link>
        <a href="#" class="active">Újdonságok</a>
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum">Fórum</router-link>
        <button class="btnLogin-popup" @click="openLogin">Bejelentkezés</button>
      </nav>
    </header>

    <div class="banner">
      <img src="/src/assets/bigpicture.webp" alt="banner">
    </div>

    <div class="container" :class="{ 'active-popup': loginPopup }">
      <span class="icon-close" @click="closeLogin"><i class='bx bx-x'></i></span>
      <div class="content">
        <h2 class="logo"><i class='bx bxl-firefox'></i>CombatMaster</h2>
        <div class="text-sci">
          <h2>Üdv!</h2>
          <p>Az igazi szórakozás nálunk kezdődik!</p>
        </div>
      </div>

      <div class="logreg-box" :class="{ active: activeTab === 'register' }">
        <div class="form-box login">
          <form @submit.prevent="onLogin">
            <h2>Bejelentkezés</h2>
            <div class="input-box">
              <span class="icon"><i class='bx bxs-envelope'></i></span>
              <input v-model="loginEmail" type="email" required />
              <label>Email</label>
            </div>
            <div class="input-box">
              <span class="icon"><i class='bx bxs-lock-alt'></i></span>
              <input v-model="loginPassword" type="password" required />
              <label>Jelszó</label>
            </div>
            <button type="submit" class="btn">Bejelentkezés</button>
            <div class="login-register">
              <p>Még nincs fiókod? <a href="#" @click.prevent="switchTab('register')">Regisztrálj</a></p>
            </div>
          </form>
        </div>

        <div class="form-box register">
          <form @submit.prevent="onRegister">
            <h2>Regisztráció</h2>
            <div class="input-box">
              <span class="icon"><i class='bx bxs-user'></i></span>
              <input v-model="regUsername" type="text" required />
              <label>Felhasználónév</label>
            </div>
            <div class="input-box">
              <span class="icon"><i class='bx bxs-envelope'></i></span>
              <input v-model="regEmail" type="email" required />
              <label>Email</label>
            </div>
            <div class="input-box">
              <span class="icon"><i class='bx bxs-lock-alt'></i></span>
              <input v-model="regPassword" type="password" required />
              <label>Jelszó</label>
            </div>
            <button type="submit" class="btn">Regisztráció</button>
            <div class="login-register">
              <p>Már van fiókod? <a href="#" @click.prevent="switchTab('login')">Bejelentkezés</a></p>
            </div>
          </form>
        </div>
      </div>
    </div>

    <div class="tabs">
      <button class="tab" :class="{active: activeContent==='news'}" @click="showTab('news')">Hírek</button>
      <button class="tab" :class="{active: activeContent==='updates'}" @click="showTab('updates')">Frissítések</button>
    </div>

    <div id="news" class="content" v-show="activeContent==='news'">
      <div class="container1">
        <div class="timeline">
          <ul>
            <li v-for="(item, index) in newsItems" :key="index">
              <div class="timeline-content">
                <h2 class="date">{{ item.date }}</h2>
                <h1>{{ item.title }}</h1>
                <p>{{ item.text }}</p>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <div id="updates" class="content" v-show="activeContent==='updates'">
      <div class="container2">
        <div class="timeline2">
          <ul>
            <li v-for="(item, index) in updatesItems" :key="index">
              <div class="timeline-content2">
                <h2 class="date2">{{ item.date }}</h2>
                <h1>{{ item.title }}</h1>
                <ul>
                  <li v-for="(li, idx) in item.list" :key="idx">{{ li }}</li>
                </ul>
              </div>
              <hr v-if="index<updatesItems.length-1">
            </li>
          </ul>
        </div>
      </div>
    </div>

    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from '../components/SiteFooter.vue'
import { ref } from 'vue'
import { useUserStore } from '../stores/user'

const loginPopup = ref(false)
const activeTab = ref('login')
const activeContent = ref('news')
const hamburgerActive = ref(false)

const loginEmail = ref('')
const loginPassword = ref('')
const regUsername = ref('')
const regEmail = ref('')
const regPassword = ref('')

const store = useUserStore()

function openLogin() { loginPopup.value = true }
function closeLogin() { loginPopup.value = false }
function switchTab(tab) { activeTab.value = tab }
function toggleHamburger() { hamburgerActive.value = !hamburgerActive.value }
function showTab(tab) { activeContent.value = tab }

async function onLogin() {
  try {
    await store.login({ email: loginEmail.value, password: loginPassword.value })
    loginEmail.value = ''
    loginPassword.value = ''
    closeLogin()
  } catch (e) { alert('Hiba a bejelentkezés során') }
}

async function onRegister() {
  try {
    await store.register({ username: regUsername.value, email: regEmail.value, password: regPassword.value })
    regUsername.value = ''
    regEmail.value = ''
    regPassword.value = ''
    switchTab('login')
  } catch (e) { alert('Hiba a regisztráció során') }
}

const newsItems = [
  { date: '2025. Október 26.', title: 'Lorem', text: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
  { date: '2025. Október 26.', title: 'Lorem 1', text: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
  { date: '2025. Október 26.', title: 'Lorem 2', text: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
  { date: '2025. Október 26.', title: 'Lorem 3', text: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
  { date: '2025. Október 26.', title: 'Lorem 4', text: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.' },
]

const updatesItems = [
  { date: '2025. Október 26.', title: 'Játékmenet', list: ['Lorem, ipsum.', 'Lorem ipsum dolor sit amet consectetur.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Maiores, quibusdam.'] },
  { date: '2025. Október 26.', title: 'Játékmenet', list: ['Lorem, ipsum.', 'Lorem ipsum dolor sit amet consectetur.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Maiores, quibusdam.'] },
  { date: '2025. Október 26.', title: 'Játékmenet', list: ['Lorem, ipsum.', 'Lorem ipsum dolor sit amet consectetur.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Maiores, quibusdam.'] },
  { date: '2025. Október 26.', title: 'Játékmenet', list: ['Lorem, ipsum.', 'Lorem ipsum dolor sit amet consectetur.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Maiores, quibusdam.'] },
  { date: '2025. Október 26.', title: 'Játékmenet', list: ['Lorem, ipsum.', 'Lorem ipsum dolor sit amet consectetur.', 'Lorem ipsum dolor sit amet consectetur adipisicing elit. Maiores, quibusdam.'] },
  
]
</script>

<style scoped>
@import "/src/assets/update.css";
</style>