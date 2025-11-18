<template>
  <header class="header">
    <h2 class="logo">Logo</h2>

    <!-- Hamburger -->
    <div class="hamburger" @click="toggleMobile" :class="{ active: mobileActive }">
      <span></span>
      <span></span>
      <span></span>
    </div>

    <!-- Navbar -->
    <nav :class="['navbar', { active: mobileActive }]">
      <router-link to="/">Főoldal</router-link>
      <router-link to="/update">Újdonságok</router-link>

      <router-link
        to="/statics"
        @click.prevent="checkAuth('/statics')"
      >Statisztika</router-link>

      <router-link
        to="/forum"
        @click.prevent="checkAuth('/forum')"
      >Fórum</router-link>

      <!-- Bejelentkezés vagy felhasználói menü -->
      <button
        v-if="!store.user"
        class="btnLogin-popup"
        @click="openAuth('login')"
      >
        Bejelentkezés
      </button>

      <div v-else class="user-wrapper">
        <!-- profilkép -->
        <img
          :src="store.user.image || '/src/assets/userrrr.png'"
          alt="user"
          class="user-pic"
          @click="toggleMenu"
        />

        <!-- dropdown menü -->
        <div class="user-menu" :class="{ active: menuActive, mobile: mobileActive }">
          <router-link to="/profile" @click="menuActive = false">Profilom</router-link>
          <a href="#" @click.prevent="logout">Kijelentkezés</a>
        </div>
      </div>
    </nav>
  </header>
</template>

<script setup>
import { ref } from 'vue'
import { useUserStore } from '../stores/user'
import { useRouter } from 'vue-router'

const store = useUserStore()
const router = useRouter()

const mobileActive = ref(false)
const menuActive = ref(false)

function toggleMobile() {
  mobileActive.value = !mobileActive.value
}

function toggleMenu() {
  menuActive.value = !menuActive.value
}

function openAuth(tab = 'login') {
  store.openAuth(tab)
}

function logout() {
  store.user = null
  menuActive.value = false
  alert('Sikeres kijelentkezés!')
}

function checkAuth(path) {
  if (!store.user) {
    alert('Bejelentkezés szükséges')
  } else {
    router.push(path)
  }
}
</script>