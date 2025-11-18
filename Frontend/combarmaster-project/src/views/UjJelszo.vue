<template>
  <div class="reset-container">
    <div class="form-box">
      <form class="LoginForm" @submit.prevent="onSubmit">
        <h2>Elfelejtett jelszó</h2>

        <div class="input-box">
          <input type="email" v-model="email" required />
          <label>Email</label>
          <i class='bx bxs-envelope'></i>
        </div>

        <button class="btn">Igény indítása</button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'   // ⬅️ HELYES IMPORT

const email = ref('')
const router = useRouter()

async function onSubmit() {
  try {
    await api.post('/Password/forgot', {
      email: email.value
    })

    alert("Jelszó-visszaállító kérés elküldve: " + email.value)

    router.push('/helyre-jelszo')
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