<template>
  <div>
    <header class="header">
      <h2 class="logo">Logo</h2>
      <nav class="navbar">
        <button class="btnLogin-popup" @click="logout">Kijelentkezés</button>
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
              <th @click="sortTable(1)">ID <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(2)">Név <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(3)">Email <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(4)">Jelszó <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="(row, index) in filteredRows" :key="row.id">
              <td><input type="checkbox" v-model="row.selected" class="row-check"></td>
              <td>{{ row.id }}</td>
              <td>{{ row.name }}</td>
              <td>{{ row.email }}</td>
              <td>{{ row.password }}</td>
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

const router = useRouter() // router példány

const rows = reactive([]) // ide töltjük majd az adatbázisból jövő felhasználókat
const searchValue = ref('')

// Axios példány a backend eléréséhez
const api = axios.create({
  baseURL: 'http://localhost:7030/api' // változtasd a saját backend URL-edre
})

// Felhasználók lekérése mount-kor
onMounted(async () => {
  try {
    const response = await api.get('/Admin/users')
    response.data.forEach(user => {
      rows.push({
        id: user.id,
        name: user.username,
        email: user.email,
        password: user.password,
        selected: false
      })
    })
  } catch (err) {
    console.error('Hiba a felhasználók lekérésekor:', err)
    alert('Hiba a felhasználók betöltésekor!')
  }
})

// Szűrt sorok keresés alapján (ID, név, email)
const filteredRows = computed(() => {
  return rows.filter(row =>
    row.id.toString().includes(searchValue.value) ||
    row.name.toLowerCase().includes(searchValue.value.toLowerCase()) ||
    (row.email && row.email.toLowerCase().includes(searchValue.value.toLowerCase()))
  )
})

// Törlés (frontend + backend)
async function deleteSelected() {
  const toDelete = rows.filter(row => row.selected)
  if (toDelete.length === 0) return alert('Nincs kijelölt felhasználó!')

  try {
    for (const row of toDelete) {
      await api.delete(`/Admin/users/${row.id}`)
      const index = rows.findIndex(r => r.id === row.id)
      if (index > -1) rows.splice(index, 1) // frontendről is töröljük
    }
    alert('Kijelölt felhasználók törölve!')
  } catch (err) {
    console.error('Hiba a törlés során:', err)
    alert('Hiba a felhasználók törlésekor!')
  }
}

// Logout és visszanavigálás a HomeView-ra
function logout() {
  localStorage.removeItem('token') // token törlése
  router.push('/') // vissza a home oldalra
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

.logo{
    font-size: 2em;
    color: #fae9d7;
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
    display: none;
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
