<template>
  <div class="reset-container">
    <div class="form-box">
      <form class="LoginForm" @submit.prevent="onSubmit">
        <h2>Elfelejtett jelszó</h2>

        <!-- Kód mező -->
        <div class="input-box">
          <input type="text" v-model="code" required />
          <label>Kód</label>
          <i class='bx bxs-key'></i>
        </div>

        <!-- Új jelszó mező -->
        <div class="input-box">
          <input type="password" v-model="newPassword" required />
          <label>Új Jelszó</label>
          <i class='bx bxs-lock-alt'></i>
        </div>

        <button class="btn">Jelszó Megváltoztatása</button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const code = ref('')
const newPassword = ref('')

async function onSubmit() {
  try {
    await api.post('/Password/reset', {
      code: code.value,
      newPassword: newPassword.value
    })

    alert("A jelszó sikeresen megváltoztatva!")

    router.push('/')
  } 
  catch (err) {
    console.error("Hiba:", err.response?.data || err)
    alert("Hiba történt: " + (err.response?.data?.message || err.message))
  }
}

onMounted(() => {
  document.body.classList.add('forgot-password-bg')
})

onUnmounted(() => {
  document.body.classList.remove('forgot-password-bg')
})
</script>



<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    color: #fae9d7;
}

.reset-container {
    height: 550px;
    width: 450px;
    border: 2px solid #1c1c1ce6;
    border-radius: 10px;
    background: rgba(0, 0, 0, .2);
    backdrop-filter: blur(10px);
    box-shadow: 0 0 5px rgba(0, 0, 0, .2),
                0 0 15px rgba(0, 0, 0, .2),
                0 0 30px rgba(0, 0, 0, .2);
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
    margin: auto;
    margin-top: 80px;
}

.form-box {
    display: flex;
}

.reset-container .form-box h2 {
    text-align: center;
    text-transform: uppercase;
    font-size: 30px;
    font-weight: 600;
}

.form-box .input-box {
    margin: 35px 0;
    width: 330px;
    border-bottom: 2px solid #1c1c1ccf;
    position: relative;
}

.form-box .input-box input {
    background: transparent;
    border: none;
    outline: none;
    width: 100%;
    height: 40px;
    padding: 0 35px 0 5px;
}

.form-box .input-box label {
    font-weight: 400;
    letter-spacing: .5px;
    position: absolute;
    left: 5px;
    top: 50%;
    transform: translateY(-50%);
    transition: .5s ease;
}

.input-box input:focus ~ label,
.input-box input:valid ~ label {
    top: -5px;
}

.input-box i {
    position: absolute;
    right: 5px;
    top: 50%;
    transform: translateY(-50%);
    font-size: 20px;
}

.btn {
    height: 45px;
    width: 100%;
    border-radius: 50px;
    color: #fae9d7;
    background: #1c1c1c;
    border: 2px solid #1c1c1ccf;
    outline: none;
    font-size: 16px;
    cursor: pointer;
    letter-spacing: 1px;
}
</style>