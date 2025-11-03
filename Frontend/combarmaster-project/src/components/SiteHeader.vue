<template>
    <header class="header">
        <h2 class="logo">Logo</h2>


        <div class="hamburger" @click="toggle">
            <span></span>
            <span></span>
            <span></span>
        </div>


        <nav :class="['navbar', { active: mobileActive }]">
            <router-link to="/" class="active">Főoldal</router-link>
            <router-link to="/update">Frissítések</router-link>
            <router-link to="/statics">Statisztika</router-link>
            <router-link to="/forum">Fórum</router-link>
            <button class="btnLogin-popup" @click="openAuth">Bejelentkezés</button>
        </nav>
    </header>
</template>


<script setup>
import { ref } from 'vue'
import { useUserStore } from '../stores/user'


const mobileActive = ref(false)
const store = useUserStore()
function toggle() { mobileActive.value = !mobileActive.value }
function openAuth() { store.openAuth('login') }
</script>