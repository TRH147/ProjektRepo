<template>
    <div :class="['container', { 'active-popup': store.isAuthModalOpen }]">
        <span class="icon-close" @click="store.closeAuth"><i class='bx bx-x'></i></span>


        <div class="content">
            <h2 class="logo"><i class='bx bxl-firefox'></i>CombatMaster</h2>


            <div class="text-sci">
                <h2>Üdv!</h2>
                <p>Az igazi szórakozás nálunk kezdődik!</p>
            </div>
        </div>


        <div class="logreg-box" :class="{ active: store.activeTab === 'register' }">
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
                        <p>Még nincs fiókod? <a href="#" class="register-link"
                                @click.prevent="switchTo('register')"><span>Regisztrálj</span></a></p>
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
                        <p>Már van fiókod? <a href="#" class="login-link"
                                @click.prevent="switchTo('login')"><span>Bejelentkezés</span></a></p>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useUserStore } from '../stores/user'


const store = useUserStore()


const loginEmail = ref('')
const loginPassword = ref('')
const regUsername = ref('')
const regEmail = ref('')
const regPassword = ref('')


function switchTo(tab) { store.activeTab = tab }


async function onLogin() {
    try {
        await store.login({ email: loginEmail.value, password: loginPassword.value })

        loginEmail.value = ''
        loginPassword.value = ''
    } catch (e) {
        alert('Hiba a bejelentkezés során')
    }
}


async function onRegister() {
    try {
        await store.register({ username: regUsername.value, email: regEmail.value, password: regPassword.value })
        regUsername.value = ''
        regEmail.value = ''
        regPassword.value = ''
    } catch (e) {
        alert('Hiba a regisztráció során')
    }
}
</script>