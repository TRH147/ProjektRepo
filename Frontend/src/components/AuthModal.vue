<template>
    <div :class="['auth-container', { 'active-popup': store.isAuthModalOpen }]">
        <span class="icon-close" @click="store.closeAuth"><i class='bx bx-x'></i></span>


        <div class="content">
            <h2 class="logo-be">
              Combat<img :src="logoImg" alt="Logo" class="logo-img" />Master
            </h2>


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
                        <span class="icon"><i class='bx bxs-user'></i></span>
                        <input v-model="loginUsername" type="text" required />
                        <label>Felhasználónév</label>
                    </div>


                    <div class="input-box">
                        <span class="icon"><i class='bx bxs-lock-alt'></i></span>
                        <input v-model="loginPassword" type="password" required />
                        <label>Jelszó</label>
                    </div>

                    <a href="#" class="remember-forgot" @click.prevent="goToForgot">
                        Elfelejtett Jelszó
                    </a>




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
import { useRouter } from 'vue-router'
import logoImg from '/src/assets/Logo.png'

const router = useRouter()
const store = useUserStore()

const loginUsername = ref('')
const loginPassword = ref('')

const regUsername = ref('')
const regEmail = ref('')
const regPassword = ref('')

function switchTo(tab) { store.activeTab = tab }

function goToForgot() {
  store.closeAuth()
  router.push('/forgot-password')
}

async function onLogin() {
  try {
    const response = await store.login({
      username: loginUsername.value,
      password: loginPassword.value
    });

    loginUsername.value = '';
    loginPassword.value = '';

    if (response.isAdmin) {
      router.push('/admin'); 
    }
  } catch (e) {
  }
}

async function onRegister() {
  try {
    await store.register({ username: regUsername.value, email: regEmail.value, password: regPassword.value })
    regUsername.value = ''
    regEmail.value = ''
    regPassword.value = ''
  } catch (e) {
  }
}
</script>
<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: "Belleza", sans-serif;
}

.auth-container {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%) scale(0.7);
    width: 90%;
    max-width: 1100px;
    min-width: 300px;
    height: 550px;
    max-height: 90vh;
    background: url('/src/assets/bigpicture.webp') no-repeat center center / cover;
    border-radius: 10px;
    overflow: hidden;
    opacity: 0;
    pointer-events: none;
    transition: transform 0.5s ease, opacity 0.4s ease;
    z-index: 9999;
}

.auth-container.active-popup {
    transform: translate(-50%, -50%) scale(1);
    opacity: 1;
    pointer-events: auto;
}

.auth-container .icon-close {
    position: absolute;
    top: 0;
    right: 0;
    width: 45px;
    height: 45px;
    background: #1c1c1c;
    font-size: 2em;
    color: #fae9d7;
    display: flex;
    justify-content: center;
    align-items: center;
    border-bottom-left-radius: 20px;
    cursor: pointer;
    z-index: 1;
}

.auth-container .content {
    position: absolute;
    top: 0;
    left: 0;
    width: 58%;
    height: 100%;
    background: transparent;
    padding: 40px;
    color: #fae9d7;
    display: flex;
    justify-content: space-between;
    flex-direction: column;
}

.content .logo-be {
    font-size: clamp(24px, 3vw, 40px);
    color: #fae9d7;
    user-select: none;
    display: flex;
    align-items: center;
    gap: 5px;
}

.logo-img {
    width: clamp(40px, 6vw, 80px);
    margin: 0 5px;
    height: auto;
    filter: drop-shadow(2px 2px 5px rgba(0,0,0,0.7));
}

.auth-container h2 {
    font-size: clamp(28px, 4vw, 50px);
    line-height: 1.2;
}

.text-sci h2 {
    font-size: clamp(24px, 3vw, 30px);
    line-height: 1;
    margin-bottom: 10px;
}

.text-sci p {
    font-size: clamp(14px, 1.5vw, 16px);
    margin: 10px 0;
}

.auth-container .logreg-box {
    position: absolute;
    top: 0;
    right: 0;
    width: 42%;
    height: 100%;
}

.logreg-box .form-box {
    position: absolute;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 100%;
    background: rgba(28, 28, 28, 0.85);
    backdrop-filter: blur(10px);
    border-top-right-radius: 10px;
    border-bottom-right-radius: 10px;
    color: #fae9d7;
    overflow: hidden;
    padding: 20px;
}

.logreg-box .form-box.login {
    transform: translateX(0);
    transition: transform .6s ease;
    transition-delay: .7s;
}

.logreg-box.active .form-box.login {
    transform: translateX(100%);
    transition-delay: 0s;
}

.logreg-box .form-box.register {
    transform: translateX(100%);
    transition: transform .6s ease;
    transition-delay: 0s;
}

.logreg-box.active .form-box.register {
    transform: translateX(0);
    transition-delay: .7s;
}

.form-box h2 {
    font-size: clamp(24px, 3vw, 32px);
    text-align: center;
    margin-bottom: 20px;
}

.form-box .input-box {
    position: relative;
    width: 100%;
    max-width: 340px;
    height: 50px;
    border-bottom: 2px solid #fae9d7;
    margin: 25px 0;
}

.input-box input {
    width: 100%;
    height: 100%;
    background: transparent;
    border: none;
    outline: none;
    font-size: clamp(14px, 1.5vw, 16px);
    color: #fae9d7;
    font-weight: 500;
    padding-right: 35px;
}

.input-box label {
    position: absolute;
    top: 50%;
    left: 0;
    transform: translateY(-50%);
    font-size: clamp(14px, 1.5vw, 16px);
    font-weight: 500;
    pointer-events: none;
    transition: .5s ease;
}

.input-box input:focus~label,
.input-box input:valid~label {
    top: -5px;
    font-size: 14px;
}

.input-box .icon {
    position: absolute;
    top: 13px;
    right: 0;
    font-size: 19px;
}

.form-box .remember-forgot {
    font-size: clamp(12px, 1.3vw, 14.5px);
    font-weight: 500;
    margin: -10px 0 15px;
    color: #fae9d7;
    display: flex;
    justify-content: flex-end;
    text-align: right;
}

.remember-forgot:hover {
    color: #d55053;
    text-decoration: underline;
    cursor: pointer;
}

.btn {
    width: 100%;
    max-width: 340px;
    height: 45px;
    background: #1c1c1c;
    border: none;
    outline: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: clamp(14px, 1.5vw, 16px);
    color: #fae9d7;
    font-weight: 500;
    box-shadow: 0 0 10px rgba(0, 0, 0, .5);
    transition: background 0.3s, transform 0.2s;
}

.btn:hover {
    background: #d55053;
    transform: translateY(-2px);
}

.btn:active {
    transform: translateY(0);
}

.form-box .login-register {
    font-size: clamp(12px, 1.3vw, 14.5px);
    font-weight: 500;
    text-align: center;
    margin-top: 20px;
}

.login-register p {
    color: #fae9d7;
}

.login-register p a {
    color: #d55053;
    font-weight: 600;
    text-decoration: none;
    transition: color 0.3s;
}

.login-register p a:hover {
    color: #fae9d7;
    text-decoration: underline;
}

@media (max-width: 1200px) {
    .auth-container {
        width: 95%;
        max-width: 1000px;
    }
}

@media (max-width: 1024px) {
    .auth-container {
        width: 95%;
        height: auto;
        min-height: 500px;
        max-height: 85vh;
    }

    .auth-container .content {
        padding: 30px;
        width: 50%;
    }

    .auth-container .logreg-box {
        width: 50%;
    }

    .form-box .input-box {
        max-width: 300px;
        margin: 20px 0;
    }
}

@media (max-width: 850px) {
    .auth-container {
        width: 95%;
        min-height: 450px;
        max-height: 90vh;
        overflow-y: auto;
    }

    .auth-container .content {
        position: relative;
        width: 100%;
        height: auto;
        padding: 20px 20px;
        text-align: center;
    }

    .auth-container .logreg-box {
        position: relative;
        width: 100%;
        height: auto;
        padding: 20px;
    }

    .logreg-box .form-box {
        position: relative;
        background: rgba(28, 28, 28, 0.9);
        border-radius: 10px;
        margin-top: 20px;
        padding: 30px 20px;
    }

    .logreg-box .form-box.login {
        transform: translateX(0);
        display: none;
    }

    .logreg-box:not(.active) .form-box.login {
        display: flex;
    }

    .logreg-box .form-box.register {
        transform: translateX(0);
        display: none;
    }

    .logreg-box.active .form-box.register {
        display: flex;
    }

    .form-box .input-box {
        max-width: 100%;
        margin: 20px auto;
    }

    .btn {
        max-width: 100%;
    }

    .content .logo-be {
        justify-content: center;
        margin-bottom: 20px;
    }

    .text-sci {
        margin-bottom: 20px;
    }
}

@media (max-width: 600px) {
    .auth-container {
        width: 98%;
        min-height: auto;
        max-height: 95vh;
        border-radius: 8px;
    }

    .auth-container .icon-close {
        width: 40px;
        height: 40px;
        font-size: 1.5em;
    }

    .auth-container .content {
        padding: 20px 15px;
    }

    .logreg-box .form-box {
        padding: 25px 15px;
    }

    .form-box h2 {
        margin-bottom: 15px;
    }

    .form-box .input-box {
        height: 45px;
        margin: 15px auto;
    }

    .input-box input {
        font-size: 15px;
    }

    .input-box label {
        font-size: 15px;
    }

    .btn {
        height: 42px;
        font-size: 15px;
    }

    .form-box .remember-forgot {
        font-size: 13px;
    }

    .form-box .login-register {
        font-size: 13px;
        margin-top: 15px;
    }
}

@media (max-width: 400px) {
    .auth-container {
        width: 100%;
        border-radius: 0;
        max-height: 100vh;
        height: 100vh;
        transform: translate(-50%, -50%) scale(1);
    }

    .auth-container.active-popup {
        transform: translate(-50%, -50%) scale(1);
    }

    .auth-container .content {
        padding: 15px;
    }

    .logreg-box .form-box {
        padding: 20px 10px;
    }

    .form-box .input-box {
        height: 42px;
    }

    .input-box .icon {
        font-size: 17px;
        top: 12px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .auth-container {
        height: auto;
        min-height: auto;
        max-height: 95vh;
        overflow-y: auto;
    }

    .auth-container .content {
        height: auto;
        min-height: auto;
    }

    .auth-container .logreg-box {
        height: auto;
        min-height: auto;
    }

    .logreg-box .form-box {
        height: auto;
        min-height: auto;
        padding: 20px;
    }

    .form-box .input-box {
        height: 40px;
        margin: 12px 0;
    }

    .btn {
        height: 38px;
        margin-top: 10px;
    }

    .form-box h2 {
        margin-bottom: 15px;
        font-size: 22px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .btn {
        min-height: 44px;
    }

    .input-box {
        min-height: 44px; 
    }

    .remember-forgot,
    .login-register p a {
        padding: 8px 0;
    }
}
</style>