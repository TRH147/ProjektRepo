import { defineStore } from 'pinia'
import api from '../services/api'
import { ref } from 'vue'


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

            user.value = { email: payload.email, name: 'Demo user' }
            isAuthModalOpen.value = false
            return user.value
        } catch (err) {
            throw err
        }
    }


    async function register(payload) {
        try {

            user.value = { email: payload.email, name: payload.username }
            isAuthModalOpen.value = false
            return user.value
        } catch (err) {
            throw err
        }
    }


    return { isAuthModalOpen, activeTab, user, openAuth, closeAuth, login, register }
})