<template>
  <div>
    <header class="header">
    <img :src="logoImg" alt="Logo" class="logo" />
      <nav class="navbar">
        <button class="btnLogin-popup" @click="logout">Visszatérés a Főoldalra</button>
      </nav>
    </header>

    <section class="controls">
      <input type="text" v-model="searchValue" placeholder="Keresés..." />
      <button @click="deleteSelected">Kijelölt törlése</button>
    </section>

    <main class="custon_table">
      <section class="table_header">
        <h1>Eredmények</h1>
      </section>

      <section class="table_body">
        <table id="mainTable">
          <thead>
            <tr>
              <th></th>
              <th @click="sortTable('id')" :class="{ active: sortColumn === 'id' }">ID <span class="icon-arrow"><i class='bx bx-up-arrow-alt' :class="{ rotated: sortColumn === 'id' && sortOrder === 'desc' }"></i></span></th>
              <th @click="sortTable('name')" :class="{ active: sortColumn === 'name' }">Név <span class="icon-arrow"><i class='bx bx-up-arrow-alt' :class="{ rotated: sortColumn === 'name' && sortOrder === 'desc' }"></i></span></th>
              <th @click="sortTable('email')" :class="{ active: sortColumn === 'email' }">Email <span class="icon-arrow"><i class='bx bx-up-arrow-alt' :class="{ rotated: sortColumn === 'email' && sortOrder === 'desc' }"></i></span></th>
              <th @click="sortTable('createdAt')" :class="{ active: sortColumn === 'createdAt' }">Fiók létrehozásának dátuma <span class="icon-arrow"><i class='bx bx-up-arrow-alt' :class="{ rotated: sortColumn === 'createdAt' && sortOrder === 'desc' }"></i></span></th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="row in sortedAndFilteredRows" :key="row.id">
              <td><input type="checkbox" v-model="row.selected" class="row-check"></td>
              <td>{{ row.id }}</td>
              <td>{{ row.name }}</td>
              <td>{{ row.email }}</td>
              <td>{{ formatDate(row.createdAt) }}</td>
            </tr>
          </tbody>
        </table>
      </section>

      <div class="no-match-message" v-if="filteredRows.length === 0">No matching data found</div>
    </main>
  </div>
</template>

<script setup>
import { reactive, ref, computed, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'
import { notificationService } from '../services/notificationService'
import logoImg from '/src/assets/Logo.png'

const router = useRouter() 
const userStore = useUserStore()

const rows = reactive([])
const searchValue = ref('')
const sortColumn = ref('id')
const sortOrder = ref('asc')

const api = axios.create({
  baseURL: 'https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api'
})

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

onMounted(async () => {
  try {
    const isAdmin = userStore.isAdmin()
    const token = localStorage.getItem('token')
    
    console.log('Is admin?', isAdmin)
    console.log('Token exists?', !!token)
    
    if (!token) {
      notificationService.warning(
        'Nincs token',
        'Nincs token! Kérjük, jelentkezzen be.',
        5000
      )
      router.push('/')
      return
    }
    
    if (!isAdmin) {
      notificationService.error(
        'Hozzáférés megtagadva',
        'Hozzáférés megtagadva! Csak adminisztrátorok érhetik el ezt az oldalt.',
        5000
      )
      router.push('/')
      return
    }

    const response = await api.get('/Admin/users')
    response.data.forEach(user => {
      rows.push({
        id: user.id,
        name: user.username,
        email: user.email,
        createdAt: user.createdAt, 
        selected: false
      })
    })
    
    notificationService.success(
      'Felhasználók betöltve',
      `${rows.length} felhasználó sikeresen betöltve`,
      3000
    )
    
    console.log(`${rows.length} felhasználó betöltve`)
    
  } catch (err) {
    console.error('Hiba a felhasználók lekérésekor:', err)
    
    if (err.response?.status === 401) {
      notificationService.error(
        'Hitelesítési hiba',
        'Hitelesítés sikertelen! Kérjük, jelentkezzen be újra.',
        5000
      )
      userStore.logout()
      router.push('/')
    } else if (err.response?.status === 403) {
      notificationService.error(
        'Hozzáférés megtagadva',
        'Hozzáférés megtagadva! Admin jogosultság szükséges.',
        5000
      )
      router.push('/')
    } else {
      notificationService.error(
        'Betöltési hiba',
        'Hiba a felhasználók betöltésekor!',
        5000
      )
    }
  }
})

// Keresés
const filteredRows = computed(() => {
  return rows.filter(row =>
    row.id.toString().includes(searchValue.value) ||
    row.name.toLowerCase().includes(searchValue.value.toLowerCase()) ||
    (row.email && row.email.toLowerCase().includes(searchValue.value.toLowerCase()))
  )
})

// Rendezés
const sortedAndFilteredRows = computed(() => {
  let sorted = [...filteredRows.value]
  
  sorted.sort((a, b) => {
    let valA = a[sortColumn.value]
    let valB = b[sortColumn.value]
    
    // Dátum kezelése
    if (sortColumn.value === 'createdAt') {
      valA = new Date(valA).getTime()
      valB = new Date(valB).getTime()
    }
    
    // String összehasonlítás
    if (typeof valA === 'string') {
      valA = valA.toLowerCase()
      valB = valB.toLowerCase()
    }
    
    if (valA < valB) return sortOrder.value === 'asc' ? -1 : 1
    if (valA > valB) return sortOrder.value === 'asc' ? 1 : -1
    return 0
  })
  
  return sorted
})

// Rendezési függvény
function sortTable(column) {
  if (sortColumn.value === column) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortOrder.value = 'asc'
  }
}

async function deleteSelected() {
  const toDelete = rows.filter(row => row.selected)
  if (toDelete.length === 0) {
    notificationService.warning(
      'Nincs kijelölés',
      'Nincs kijelölt felhasználó!',
      4000
    )
    return
  }

  try {
    for (const row of toDelete) {
      await api.delete(`/Admin/users/${row.id}`)
      const index = rows.findIndex(r => r.id === row.id)
      if (index > -1) rows.splice(index, 1)
    }
    
    notificationService.success(
      'Törlés sikeres',
      `${toDelete.length} felhasználó sikeresen törölve!`,
      5000
    )
  } catch (err) {
    console.error('Hiba a törlés során:', err)
    
    notificationService.error(
      'Törlési hiba',
      'Hiba a felhasználók törlésekor!',
      5000
    )
  }
}

function logout() {
  localStorage.removeItem('token') 
  router.push('/')
  notificationService.info(
    'Kijelentkezés',
    'Sikeresen kijelentkeztél.',
    4000
  )
}

function formatDate(date) {
  if (!date) return ''
  const d = new Date(date)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const hours = String(d.getHours()).padStart(2, '0')
  const minutes = String(d.getMinutes()).padStart(2, '0')
  const seconds = String(d.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}
</script>


<style scoped>
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

*{
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: "Belleza", sans-serif;
}

body{
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    background: #1c1c1c;
}

.header{
    position: relative;
    width: 100%;
    padding: 10px 12.5%;
    background: transparent;
    display: flex;
    justify-content: space-between;
    align-items: center;
    z-index: 100;
}

.logo {
    width: clamp(60px, 8vw, 80px);
    height: auto;
    user-select: none;
}

.navbar {
    display: flex;
    align-items: center;
}

.navbar .btnLogin-popup{
    width: 140px;
    height: 50px;
    background: transparent;
    border: 2px solid #fae9d7;
    outline: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 15px;
    text-transform: uppercase;
    color: #fae9d7;
    font-weight: 600;
    text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
    transition: .5s;
}

.navbar .btnLogin-popup:hover{
    background: #d55053;
    border: 2px solid #d55053;
    color: #1c1c1c;
}

.controls{
    padding: 15px;
    display: flex;
    gap: 20px;
    justify-content: center;
    margin-top: 80px;
}

.controls input{
    padding: 10px;
    width: 250px;
    border-radius: 6px;
    border: 2px solid #fae9d7;
    background: #1c1c1c;
    color: #fae9d7;
}

.controls button{
    padding: 10px 20px;
    border-radius: 6px;
    background: #d55053;
    border: none;
    font-weight: bold;
    cursor: pointer;
    color: #1c1c1c;
}

.row-check {
    width: 20px;
    height: 20px;
    appearance: none;
    -webkit-appearance: none;
    background-color: #1c1c1c;
    border: 2px solid #fae9d7;
    border-radius: 4px;
    cursor: pointer;
    position: relative;
}

.row-check:hover {
    border-color: #fae9d7;
}

.row-check:checked {
    background-color: #fae9d7; 
    border-color: #fae9d7;
}

.row-check:checked::after {
    content: "✔";
    color: #000000;
    font-size: 16px;
    position: absolute;
    top: -3px;
    left: 2px;
    font-weight: bold;
}

main{
    width: 82vw;
    height: 50vh;
    background: rgba(0, 0, 0, 0.781);
    border: 2px solid #fae9d7;
    backdrop-filter: blur(7px);
    box-shadow: 0 .4rem .8rem black;
    border-radius: 0.9rem;
    margin: 50px auto 50px auto;
    overflow: hidden;
    position: relative;
}

.table_header{
    width: 100%;
    height: 10%;
    background: #fae9d7;
    padding: .8rem 1rem;
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
}

.table_header h1{
    color: #1c1c1c;
    margin-bottom: 5px;
}

.table_header::before{
    content: '';
    width: 42px;
    height: 42px;
    background: transparent;
    border-radius: 50%;
    position: absolute;
    top: 100%;
    left: 0;
    box-shadow: -20px -20px 0 #fae9d7;
}

.table_header::after{
    content: '';
    width: 42px;
    height: 42px;
    background: transparent;
    border-radius: 50%;
    position: absolute;
    top: 100%;
    right: 0;
    box-shadow: 20px -20px 0 #fae9d7;
}

.table_body{
    width: 98%;
    max-height: calc(89% - 1.6rem);
    background-color: #d55053;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
    margin: 1.1rem auto;
    overflow: auto;
    border-radius: 0.9rem;
}

.table_body::-webkit-scrollbar{
    width: 0;
}

table{
    width: 100%;
}

table thead th{
    position: sticky;
    top: 0;
    left: 0;
    background-color: #fae9d7;
    color: #000;
    cursor: pointer;
    text-transform: capitalize;
    z-index: 10;
}

table, th, td{
    border-collapse: collapse;
    padding: 1rem;
    text-align: left;
}

thead th span.icon-arrow{
    display: inline-block;
    text-align: center;
    font-size: 1rem;
    margin-left: 0.5rem;
    transition: .2s ease-in-out;
}

thead th span.icon-arrow i{
    display: flex;
    justify-content: center;
    align-items: center;
    width: 1.3rem;
    height: 1.3rem;
    border-radius: 50%;
    border: 1.4px solid transparent;
    color: #000;
    font-size: 15px;
    transition: transform 0.3s ease;
}

thead th span.icon-arrow i.rotated {
    transform: rotate(180deg);
}

thead th:hover span.icon-arrow i{
    border-color: #d55053;
}

thead th:hover{
    color: #d55053;
}

thead th.active span.icon-arrow i{
    background-color: #d55053;
    color: #fae9d7;
}

thead th.asc span.icon-arrow{
    transform: rotate(180deg);
}

thead th.active,
tbody td.active{
    color: #d55053;
}

table tr td{
    white-space: nowrap;
    color: #fae9d7;
}

td img{
    width: 45px;
    height: 45px;
    margin-right: 0.5rem;
    border-radius: 50%;
    vertical-align: middle;
}

tbody tr{
    --delay: 0.1s;
    transition: .5s ease-in-out var(--delay), backgroung-color 0s;
    font-weight: 600;
}

tbody tr:nth-child(even){
    background-color: #b24345;
}

tbody tr:hover{
    background-color: #000;
}

tbody tr td:nth-child(5){
    width: 300px;
}

tbody tr.hide{
    opacity: 0;
    transform: translateX(-100%);
}

tbody tr.hide td,
tbody tr.hide td p{
    padding: 0;
    font: 0 / 0 sans-serif;
    transition: .2s ease-in-out .5s;
}

.no-match-message{
    position: absolute;
    top: 58%;
    left: 50%;
    transform: translate(-50%, -50%);
    max-width: 350px;
    text-align: center;
    background: #d55053;
    color: #fae9d7;
    padding: 25px;
    font-size: 20px;
    border-radius: 15px;
    display: block;
}

@media screen and (max-width: 1250px){
    main.customers_table{
        width: 98vw;
    }
}

@media screen and (max-width: 1050px){
    td:first-child{
        min-width: 5rem;
    }

    td:not(:first-of-type){
        min-width: 12.1rem;
    }
}

@media screen and (max-width: 1000px){
    .table_body{
        max-height: calc(89% - 0.9rem);
        margin-top: 0.7rem;
    }
}

@media screen and (max-width: 600px){
    .table_header h1{
        font-size: 20px;
    }
}

@media screen and (max-width: 460px){
    .input_group{
        display: none !important;
    }
}
</style>