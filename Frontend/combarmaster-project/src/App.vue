<template>
  <div id="app">

    <SiteHeader v-if="$route.path === '/'" />
    
    <router-view />
    

    <SiteFooter v-if="$route.path === '/'" />
  </div>
</template>

<style>
body.forgot-password-bg {
    background: url('/src/assets/picture.webp') no-repeat;
    background-size: cover;
    background-position: center;
}
</style>

<script setup>
import { useRoute } from 'vue-router'
import SiteHeader from './components/SiteHeader.vue'
import SiteFooter from './components/SiteFooter.vue'

const route = useRoute()
</script>