<template>
  <div id="app">

    <SiteHeader v-if="$route.path === '/'" />
    
    <router-view />
    

    <SiteFooter v-if="$route.path === '/'" />
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import SiteHeader from './components/SiteHeader.vue'
import SiteFooter from './components/SiteFooter.vue'

const route = useRoute()
</script>