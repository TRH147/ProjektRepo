<template>
  <div>
    <!-- Header -->
    <header class="header">
      <h2 class="logo">Logo</h2>

      <div class="hamburger" @click="toggleMenu">
        <span></span>
        <span></span>
        <span></span>
      </div>

      <nav :class="['navbar', { active: menuActive }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update">Újdonságok</router-link>
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum">Fórum</router-link>
      </nav>
    </header>

    <section class="profile-container">
      <div class="profile-card">
        <img :src="profile.image" class="profile-pic">

        <ul class="profile-info">
          <li><strong>Üdvözlünk,</strong> {{ profile.name }}</li>
          <li><strong>Email:</strong> {{ profile.email }}</li>
          <li><strong>Regisztráció:</strong> {{ profile.registered }}</li>
          <li class="delete" @click="deleteAccount"><strong>Fiók eltávolítása</strong></li>
        </ul>
      </div>
    </section>

    <section class="profile-edit-container">
      <div class="profile-edit-left">
        <h3>Név változtatás</h3>
        <form @submit.prevent="updateName">
          <label for="oldName">Régi név:</label>
          <input type="text" id="oldName" v-model="oldName" placeholder="Régi név">

          <label for="newName">Új név:</label>
          <input type="text" id="newName" v-model="newName" placeholder="Új név">

          <button type="submit">Mentés</button>
        </form>
      </div>

      <div class="profile-edit-right">
        <h3>Profilkép cseréje</h3>
        <form @submit.prevent="updateProfilePic">
          <input type="file" @change="onFileChange" accept="image/*" style="color: #fae9d7;">
          <button type="submit">Feltöltés</button>
        </form>
      </div>
    </section>

    <footer class="footer">
      <h3>Kövess minket közösségi médián</h3>
      <div class="social">
        <a href="#"><i class='bx bxl-facebook-circle'></i></a>
        <a href="#"><i class='bx bxl-instagram-alt'></i></a>
        <a href="#"><i class='bx bxl-twitter'></i></a>
      </div>

      <ul class="list">
        <li><router-link to="/">Főoldal</router-link></li>
        <li><router-link to="/update">Újdonságok</router-link></li>
        <li><router-link to="/statics">Statisztika</router-link></li>
        <li><router-link to="/forum">Fórum</router-link></li>
      </ul>

      <h4>Rólunk</h4>
      <p class="copyright">
        Csapatunk 3 junior fejlesztőből áll össze, kiknek célja egy olyan játék fejlesztése,
        amely lépést tud tartani a mai modern videójátékipar termékeivel.
      </p>
    </footer>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import axios from 'axios'

// -------------------------------------------------
// AXIOS PÉLDÁNY BEÁLLÍTÁS TOKEN-NEL
// -------------------------------------------------
const api = axios.create({ baseURL: 'http://localhost:7030/api' })

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token")
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// -------------------------------------------------
// FELHASZNÁLÓ ADATOK
// -------------------------------------------------
const userId = ref(null)
const profile = reactive({
  name: '',
  image: '/userrrr.png',
  email: '',
  registered: '',
  lastAction: ''
})

// -------------------------------------------------
// FELHASZNÁLÓ LEKÉRÉSE MOUNT-KOR
// -------------------------------------------------
onMounted(async () => {
  try {
    const response = await api.get('/Users/users/me')

    userId.value = response.data.id
    profile.name = response.data.username
    profile.email = response.data.email
    profile.image = response.data.profileImages
                    ? `http://localhost:7030${response.data.profileImages}`
                    : '/userrrr.png'

    // CreatedAt mező formázása (regisztráció dátuma)
    if (response.data.createdAt) {
      const date = new Date(response.data.createdAt)
      profile.registered = date.toLocaleDateString('hu-HU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    }

    // opcionális: utolsó művelet
    profile.lastAction = response.data.lastAction || ''
  } catch (error) {
    console.error("Nem sikerült betölteni a felhasználót:", error)
  }
})

// -------------------------------------------------
// MENÜ
// -------------------------------------------------
const isMenuOpen = ref(false)
function toggleMenu() {
  isMenuOpen.value = !isMenuOpen.value
}

// -------------------------------------------------
// NÉV MÓDOSÍTÁS
// -------------------------------------------------
const newName = ref('')

async function updateName() {
  if (!newName.value) {
    alert("Kérlek adj meg egy új nevet!")
    return
  }

  try {
    const response = await api.put(`/Users/users/${userId.value}/username`, {
      currentUsername: profile.name,
      newUsername: newName.value
    })

    profile.name = response.data.updatedUser.username
    newName.value = ''
    alert("Felhasználónév frissítve!")
  } catch (error) {
    alert(error.response?.data?.message || "Hiba történt a név frissítésekor!")
  }
}

// -------------------------------------------------
// PROFILKÉP FELTÖLTÉS
// -------------------------------------------------
let selectedFile = null
function onFileChange(e) {
  selectedFile = e.target.files[0]
}

async function updateProfilePic() {
  if (!selectedFile) {
    alert("Válassz egy képet!")
    return
  }

  const formData = new FormData()
  formData.append("file", selectedFile)

  try {
    const response = await api.put(
      `/Users/users/${userId.value}/profile-image`,
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    )

    profile.image = response.data.updatedUser.profileImages
                    ? `http://localhost:7030${response.data.updatedUser.profileImages}`
                    : '/userrrr.png'

    selectedFile = null
    alert("Profilkép frissítve!")
  } catch (error) {
    alert(error.response?.data?.message || "Hiba történt a profilkép frissítésekor!")
  }
}
</script>




<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');
*{
    margin:0;
    padding:0;
    box-sizing:border-box;
    font-family:"Belleza",sans-serif;
}
body{
    background:#1c1c1c;
}
.header{
    position:relative;
    width:100%;padding:10px 12.5%;
    display:flex;
    justify-content:space-between;
    align-items:center;
    z-index:100;
}
.logo{
    font-size:2em;
    color:#fae9d7;
    user-select:none;
}
.navbar{
    display:flex;align-items:center;
}
.navbar a{
    position:relative;
    font-size:15px;
    color:#fae9d7;
    text-decoration:none;
    font-weight:600;
    text-transform:uppercase;
    text-shadow:2px 2px 5px rgba(0,0,0,0.7);
    margin-right:30px;
}
.navbar a:hover,.navbar a.active{
    color:#e24c4f;
}
.navbar a::after{
    content:'';
    position:absolute;
    left:0;
    bottom:-6px;
    width:100%;
    height:2px;
    background:#e24c4f;
    border-radius:5px;
    transform-origin:right;
    transform:scaleX(0);
    transition:transform .5s;
}
.navbar a:hover::after{
    transform-origin:left;
    transform:scaleX(1);
}
.hamburger{
    display:none;
    flex-direction:column;
    justify-content:space-between;
    width:26px;
    height:20px;
    cursor:pointer;
    z-index:101;
}
.hamburger span{
    height:3px;
    width:100%;
    background:#fae9d7;
    border-radius:2px;
    transition:0.3s;
}
.hamburger.active span:nth-child(1){
    transform:rotate(45deg) translate(4px,6px);
}
.hamburger.active span:nth-child(2){
    opacity:0;
}
.hamburger.active span:nth-child(3){
    transform:rotate(-45deg) translate(6px,-7px);
}
@media (max-width:730px),(max-height:480px){
    .hamburger{
        display:flex;
    }
    .navbar{
        position:absolute;
        top:100%;
        left:0;
        width:100%;
        flex-direction:column;
        background:rgba(20,20,20,0.95);
        text-align:center;
        padding:20px 0;
        gap:20px;
        transform:translateY(-200%);
        transition:transform 0.4s ease;
    }
    .navbar.active{
        transform:translateY(0);
    }
    .navbar a{
        margin:0;
    }
}
.profile-container{
    margin-top:140px;
    width:100%;
    display:flex;
    justify-content:center;
}
.profile-card{
    width:70%;
    max-width:
    850px;
    background:rgba(255,255,255,0.07);
    border-radius:12px;
    padding:30px;
    display:flex;
    align-items:center;
    gap:30px;
    backdrop-filter:blur(4px);
}
.profile-pic{
    width:140px;
    height:140px;
    border-radius:50%;
    object-fit:cover;
    border:3px solid #fae9d7;
}
.profile-info{
    list-style:none;
    font-size:18px;
    color:#fae9d7;
    line-height:1.8;
}
.profile-info .delete{
    color:#e24c4f;
    font-weight:bold;
    cursor:pointer;
}
@media (max-width:650px){
    .profile-card{
        flex-direction:column;
        text-align:center;
    }
    .profile-pic{
        width:120px;
        height:120px;
    }
}
.profile-edit-container{
    width:70%;
    max-width:850px;
    margin:40px auto;
    display:flex;
    gap:30px;
    flex-wrap:wrap;
}
.profile-edit-left,
.profile-edit-right{
    flex:1;
    padding:20px;
    border-radius:12px;
    backdrop-filter:blur(4px);
}
.profile-edit-left h3,
.profile-edit-right h3{
    color:#fae9d7;
    margin-bottom:15px;
}
.profile-edit-left label,
.profile-edit-right label{
    color:#fae9d7;
}
.profile-edit-left form,
.profile-edit-right form{
    display:flex;
    flex-direction:column;
    gap:12px;
}
.profile-edit-left input[type="text"]{
    background-color:black;
    color:#fae9d7;
    padding:8px 12px;
    border-radius:6px;
    border:none;
    outline:none;
}
.profile-edit-right input[type="file"]::file-selector-button{
    background-color:rgb(11,11,11);
    color:#fae9d7;
    border:none;
    border-radius:6px;
    padding:8px 12px;
    cursor:pointer;
    font-weight:600;
}
.profile-edit-left button,
.profile-edit-right button{
    padding:10px 15px;
    background:#d55053;
    border:none;
    border-radius:6px;
    color:#fae9d7;
    cursor:pointer;
    font-weight:bold;
    transition:background 0.3s;
}
.profile-edit-left button:hover,
.profile-edit-right button:hover{
    background:#a52d31;
}
.footer{
    margin-top: 60px;
    padding: 40px 0;
    background: #fae9d7;
}

.footer h3{
    text-align: center;
    color: #1c1c1c;
    margin-bottom: 25px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.footer h4{
    text-align: center;
    color: #1c1c1c;
    margin-top: 15px;
    font-size: 18px;
}

.footer .social{
    text-align: center;
    padding-bottom: 25px;
    color: #1c1c1c;
}

.footer .social a {
  font-size: 24px;
  border: 1px solid #1c1c1c;
  width: 40px;
  height: 40px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  margin: 0 8px;
  text-decoration: none;
  transition: 0.3s;
}

.footer .social a:hover{
    color: #d55053;
}

.footer ul{
    margin-top: 0;
    padding: 0;
    list-style: none;
    font-size: 18px;
    line-height: 1.6;
    margin-bottom: 0;
    text-align: center;
}

.footer ul li a{
    color: inherit;
    text-decoration: none;
}

.footer ul li{
    display: inline-block;
    padding: 0 15px;
}

.footer ul li a:hover{
    color: #d55053;
}

.footer .copyright{
    margin-top: 15px;
    text-align: center;
    font-size: 15px;
    color: #1c1c1c;
}
</style>
