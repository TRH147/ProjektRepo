<template>
  <div class="page">
    
    <header class="header">
      <img :src="logoImg" alt="Logo" class="logo" />

      <div class="hamburger" @click="toggleMenu">
        <span></span>
        <span></span>
        <span></span>
      </div>

      <nav :class="['navbar', { active: menuActive }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update">Újdonságok</router-link>
        <router-link to="/statics" class="active">Statisztika</router-link>
        <router-link to="/forum">Fórum</router-link>
      </nav>
    </header>

    <div class="banner">
      <img src="/src/assets/bigpicture.webp" alt="banner">
    </div>

    <main class="custon_table">
      <section class="table_header">
        <h1>Eredmények</h1>
      </section>

      <section class="table_body">
        <table id="mainTable">
          <thead>
            <tr>
              <th @click="sortTable(0)">Név <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(1)">Score <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <!-- <th @click="sortTable(2)">Győzelem <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(3)">Vereség <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th> -->
              <th @click="sortTable(2)">Gyilkosságok <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
            </tr>
          </thead>

          <tbody>
            <tr v-if="initialLoading">
              <td colspan="3" class="loading">Adatok betöltése...</td>
            </tr>
            <tr v-else-if="sortedData.length === 0">
              <td colspan="3" class="no-data">Nincs megjeleníthető adat</td>
            </tr>
            <tr v-else v-for="(player, i) in sortedData" :key="i">
              <td>{{ player.username }}</td>
              <td>{{ player.score }}</td>
              <!-- <td>{{ player.wins }}</td>
              <td>{{ player.losses }}</td> -->
              <td>{{ player.kills }}</td>
            </tr>
          </tbody>
        </table>
      </section>
    </main>

    
    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from '../components/SiteFooter.vue'
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import axios from 'axios' 
import logoImg from '/src/assets/Logo.png'

const menuActive = ref(false)
function toggleMenu() {
  menuActive.value = !menuActive.value
}

const data = ref([])
const initialLoading = ref(true) // Renamed to only track initial load
let refreshTimer = null

const sortColumn = ref(null)
const sortAsc = ref(true)

function sortTable(columnIndex) {
  if (sortColumn.value === columnIndex) {
    sortAsc.value = !sortAsc.value
  } else {
    sortColumn.value = columnIndex
    sortAsc.value = true
  }
}

const sortedData = computed(() => {
  if (sortColumn.value === null) return data.value

  // Módosított keyMap a szerver által küldött mezőnevekhez
  const keyMap = ["username", "score", "kills"]
  const key = keyMap[sortColumn.value]

  return [...data.value].sort((a, b) => {
    if (typeof a[key] === 'string') {
      return sortAsc.value
        ? a[key].localeCompare(b[key], 'hu', { sensitivity: 'base' })
        : b[key].localeCompare(a[key], 'hu', { sensitivity: 'base' })
    }
    return sortAsc.value ? a[key] - b[key] : b[key] - a[key]
  })
})

async function fetchStats(isInitialLoad = false) {
  // Only show loading on initial load
  if (isInitialLoad) {
    initialLoading.value = true
  }
  
  try {
    const response = await axios.get('https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api/UserStats/all', {
      headers: {
        'accept': '*/*'
      }
    })
    
    data.value = response.data
    console.log('Fetched data:', data.value) 
    
  } catch (err) {
    console.error('Hiba az adatok lekérésekor:', err)
    // Only clear data on initial load error
    if (isInitialLoad) {
      data.value = []
    }
  } finally {
    if (isInitialLoad) {
      initialLoading.value = false
    }
  }
}

onMounted(() => {
  // Initial load with loading indicator
  fetchStats(true)
  
  // Refresh every 5 seconds without showing loading
  refreshTimer = setInterval(() => fetchStats(false), 5000)
})

onBeforeUnmount(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
  }
})
</script>

<style scoped>
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: "Belleza", sans-serif;
}

body {
    margin: 0;
    padding: 0;
    background: #1c1c1c;
    box-sizing: border-box;
}

.page {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
}

.header {
    position: absolute;
    width: 100%;
    padding: 10px 5%;
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
    transition: transform 0.3s ease;
}

.logo:hover {
    transform: scale(1.05);
}

.navbar {
    display: flex;
    align-items: center;
    gap: clamp(15px, 2vw, 30px);
}

.navbar a {
    position: relative;
    font-size: clamp(14px, 1.5vw, 16px);
    color: #fae9d7;
    text-decoration: none;
    text-transform: uppercase;
    text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
    font-weight: 600;
    padding: 5px 0;
    transition: color 0.3s ease;
    white-space: nowrap;
}

.navbar a:hover {
    color: #e24c4f;
}

.navbar a.active {
    color: #e24c4f;
}

.navbar a::after {
    content: '';
    position: absolute;
    left: 0;
    bottom: -6px;
    width: 100%;
    height: 2px;
    background: #e24c4f;
    border-radius: 5px;
    transform-origin: right;
    transform: scaleX(0);
    transition: transform 0.5s ease;
}

.navbar a:hover::after {
    transform-origin: left;
    transform: scaleX(1);
}

.hamburger {
    display: none;
    flex-direction: column;
    justify-content: space-between;
    width: 30px;
    height: 21px;
    cursor: pointer;
    z-index: 101;
    padding: 0;
    background: transparent;
    border: none;
    transition: transform 0.3s ease;
}

.hamburger span {
    height: 3px;
    width: 100%;
    background: #fae9d7;
    border-radius: 2px;
    transition: 0.3s;
}

.hamburger:hover {
    transform: scale(1.1);
}

.banner {
    width: 100%;
    max-height: min(500px, 50vh);
    overflow: hidden;
    display: flex;
    justify-content: center;
    align-items: center;
}

.banner img {
    width: 100%;
    height: auto;
    max-height: 500px;
    object-fit: cover;
    display: block;
}

main.custon_table {
    width: 90%;
    max-width: 1400px;
    min-height: 400px;
    background: rgba(0, 0, 0, 0.85);
    border: 2px solid #fae9d7;
    backdrop-filter: blur(10px);
    box-shadow: 0 0.4rem 1.2rem rgba(0, 0, 0, 0.8);
    border-radius: 1rem;
    margin: clamp(30px, 5vh, 50px) auto;
    overflow: hidden;
    position: relative;
    flex: 1;
}

.table_header {
    width: 100%;
    height: clamp(60px, 10%, 80px);
    background: #fae9d7;
    padding: clamp(0.5rem, 1.5vw, 1rem) clamp(0.8rem, 2vw, 1.5rem);
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
}

.table_header h1 {
    color: #1c1c1c;
    font-size: clamp(1.5rem, 3vw, 2.2rem);
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.table_header::before {
    content: '';
    width: clamp(30px, 4vw, 42px);
    height: clamp(30px, 4vw, 42px);
    background: transparent;
    border-radius: 50%;
    position: absolute;
    top: 100%;
    left: 0;
    box-shadow: -20px -20px 0 #fae9d7;
}

.table_header::after {
    content: '';
    width: clamp(30px, 4vw, 42px);
    height: clamp(30px, 4vw, 42px);
    background: transparent;
    border-radius: 50%;
    position: absolute;
    top: 100%;
    right: 0;
    box-shadow: 20px -20px 0 #fae9d7;
}

.table_body {
    width: 98%;
    max-height: calc(100% - clamp(60px, 10%, 80px));
    height: auto;
    min-height: 300px;
    background-color: #d55053;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
    margin: 1rem auto;
    overflow: auto;
    border-radius: 0.8rem;
}

.table_body::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.table_body::-webkit-scrollbar-track {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 4px;
}

.table_body::-webkit-scrollbar-thumb {
    background: rgba(250, 233, 215, 0.5);
    border-radius: 4px;
}

.table_body::-webkit-scrollbar-thumb:hover {
    background: rgba(250, 233, 215, 0.7);
}

table {
    width: 100%;
    min-width: 600px;
    border-collapse: collapse;
}

table thead th {
    position: sticky;
    top: 0;
    left: 0;
    background-color: #fae9d7;
    color: #000;
    cursor: pointer;
    text-transform: capitalize;
    font-size: clamp(0.9rem, 1.5vw, 1.1rem);
    padding: clamp(0.8rem, 1.5vw, 1rem);
    user-select: none;
    transition: background-color 0.3s ease;
}

table thead th:hover {
    background-color: #e8d8c4;
    color: #d55053;
}

thead th span.icon-arrow {
    display: inline-block;
    text-align: center;
    font-size: clamp(0.9rem, 1.5vw, 1rem);
    margin-left: 0.3rem;
    transition: transform 0.3s ease;
}

thead th span.icon-arrow i {
    display: inline-flex;
    justify-content: center;
    align-items: center;
    width: 1.3rem;
    height: 1.3rem;
    border-radius: 50%;
    border: 1.4px solid transparent;
    color: #000;
    font-size: 15px;
    transition: all 0.3s ease;
}

thead th:hover span.icon-arrow i {
    border-color: #d55053;
}

thead th.active span.icon-arrow i {
    background-color: #d55053;
    color: #fae9d7;
}

thead th.asc span.icon-arrow {
    transform: rotate(180deg);
}

tbody {
    display: block;
    max-height: calc(400px - clamp(60px, 10%, 80px));
    overflow-y: auto;
}

thead, tbody tr {
    display: table;
    width: 100%;
    table-layout: fixed;
}

tbody tr {
    --delay: 0.1s;
    transition: background-color 0.3s ease;
    font-weight: 600;
    cursor: pointer;
}

tbody tr:nth-child(even) {
    background-color: rgba(178, 67, 69, 0.8);
}

tbody tr:hover {
    background-color: rgba(0, 0, 0, 0.7);
}

table tbody td {
    padding: clamp(0.8rem, 1.5vw, 1rem);
    text-align: center;
    font-size: clamp(1rem, 1.8vw, 1.2rem);
    color: #fae9d7;
    border-bottom: 1px solid rgba(250, 233, 215, 0.1);
    vertical-align: middle;
    transition: all 0.3s ease;
}


table tbody td:first-child {
    font-weight: 700;
    color: #fae9d7;
    text-align: left;
    font-size: clamp(1rem, 1.8vw, 1.3rem);
    padding-left: clamp(1rem, 2vw, 1.5rem);
}

table tbody td:nth-child(2),
table tbody td:nth-child(3),
table tbody td:nth-child(4),
table tbody td:nth-child(5) {
    font-weight: 700;
    font-size: clamp(1.1rem, 2vw, 1.4rem);
    color: #fae9d7;
    padding: clamp(0.8rem, 1.5vw, 1rem);
    background: transparent;
    border: none;
    border-radius: 0;
    margin: 0;
    min-width: auto;
    position: relative;
    cursor: default;
}

table tbody td:nth-child(2)::before,
table tbody td:nth-child(3)::before,
table tbody td:nth-child(4)::before,
table tbody td:nth-child(5)::before {
    display: none;
}

tbody tr:hover td:nth-child(2),
tbody tr:hover td:nth-child(3),
tbody tr:hover td:nth-child(4),
tbody tr:hover td:nth-child(5) {
    background: transparent;
    border-color: transparent;
    transform: none;
    box-shadow: none;
    color: #fff;
}

.loading, .no-data {
    text-align: center;
    padding: 20px;
    color: #fae9d7;
    font-style: italic;
    font-size: clamp(1.1rem, 2vw, 1.5rem);
    background-color: rgba(178, 67, 69, 0.8);
    border-radius: 8px;
    margin: 20px;
}

.loading {
    color: #fae9d7;
    animation: pulse 1.5s infinite;
}

@keyframes pulse {
    0% { opacity: 0.6; }
    50% { opacity: 1; }
    100% { opacity: 0.6; }
}

.no-match-message {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 90%;
    max-width: 350px;
    text-align: center;
    background: rgba(213, 80, 83, 0.95);
    color: #fae9d7;
    padding: 25px;
    font-size: clamp(1rem, 2vw, 1.3rem);
    border-radius: 15px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.5);
    display: block;
    animation: fadeIn 0.5s ease;
    z-index: 10;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translate(-50%, -45%); }
    to { opacity: 1; transform: translate(-50%, -50%); }
}

table thead th:nth-child(2),
table thead th:nth-child(3),
table thead th:nth-child(4),
table thead th:nth-child(5) {
    text-align: center;
    font-size: clamp(0.85rem, 1.3vw, 1rem);
}

@media screen and (max-width: 1024px) {
    .navbar a {
        font-size: 15px;
        gap: 15px;
    }
    
    main.custon_table {
        width: 95%;
        margin: 30px auto;
    }
    
    .banner {
        max-height: min(400px, 35vh);
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: clamp(1rem, 1.8vw, 1.3rem);
    }
}

@media screen and (max-width: 850px) {
    .hamburger {
        display: flex;
    }
    
    .navbar {
        position: fixed;
        top: 0;
        right: -100%;
        width: 100%;
        height: 100vh;
        flex-direction: column;
        backdrop-filter: blur(10px);
        text-align: center;
        padding: 100px 20px 40px;
        gap: 30px;
        transition: right 0.4s cubic-bezier(0.4, 0, 0.2, 1);
        z-index: 100;
    }
    
    .navbar.active {
        right: 0;
    }
    
    .navbar a {
        font-size: 18px;
        padding: 12px;
        width: 100%;
        max-width: 200px;
        margin: 0;
    }
    
    .hamburger.active span:nth-child(1) {
        transform: rotate(45deg) translate(6px, 6px);
    }
    .hamburger.active span:nth-child(2) {
        opacity: 0;
    }
    .hamburger.active span:nth-child(3) {
        transform: rotate(-45deg) translate(6px, -6px);
    }
    
    body.nav-open {
        overflow: hidden;
    }
    
    .banner {
        max-height: min(300px, 30vh);
    }
    
    .table_header {
        height: 70px;
    }
    
    .table_header h1 {
        font-size: 1.8rem;
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: clamp(1rem, 1.6vw, 1.2rem);
        padding: 0.7rem;
    }
}

@media screen and (max-width: 768px) {
    .header {
        padding: 10px 20px;
    }
    
    .logo {
        width: 60px;
    }
    
    main.custon_table {
        width: 96%;
        margin: 25px auto;
        min-height: 350px;
    }
    
    .table_body {
        max-height: 400px;
    }
    
    .table_header h1 {
        font-size: 1.5rem;
    }
    
    table thead th {
        padding: 0.7rem;
        font-size: 0.9rem;
    }
    
    table tbody td:first-child {
        font-size: 1rem;
        padding-left: 0.8rem;
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: 1.1rem;
        padding: 0.6rem;
    }
}

@media screen and (max-width: 600px) {
    .banner {
        max-height: min(250px, 25vh);
    }
    
    .table_header {
        height: 60px;
        padding: 0.5rem 1rem;
    }
    
    .table_header h1 {
        font-size: 1.3rem;
    }
    
    .table_header::before,
    .table_header::after {
        width: 25px;
        height: 25px;
    }
    
    .table_body {
        margin: 0.8rem auto;
        border-radius: 0.6rem;
    }
    
    table {
        min-width: 500px;
    }
    
    .no-match-message {
        padding: 20px;
        font-size: 1.1rem;
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: 1rem;
        padding: 0.5rem;
    }
}

@media screen and (max-width: 480px) {
    .header {
        padding: 10px 15px;
    }
    
    .logo {
        width: 50px;
    }
    
    .banner {
        max-height: min(200px, 20vh);
    }
    
    main.custon_table {
        width: 98%;
        margin: 20px auto;
        border-radius: 0.8rem;
        min-height: 300px;
    }
    
    .table_header {
        height: 55px;
    }
    
    .table_header h1 {
        font-size: 1.2rem;
    }
    
    .table_header::before,
    .table_header::after {
        width: 20px;
        height: 20px;
        box-shadow: -15px -15px 0 #fae9d7;
    }
    
    .table_header::after {
        box-shadow: 15px -15px 0 #fae9d7;
    }
    
    table thead th {
        padding: 0.6rem;
        font-size: 0.85rem;
    }
    
    table tbody td:first-child {
        font-size: 0.9rem;
        padding-left: 0.6rem;
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: 0.9rem;
        padding: 0.4rem;
    }
    
    thead th span.icon-arrow {
        font-size: 0.9rem;
        margin-left: 0.2rem;
    }
    
    thead th span.icon-arrow i {
        width: 1.1rem;
        height: 1.1rem;
        font-size: 13px;
    }
    
    .no-match-message {
        padding: 15px;
        font-size: 1rem;
        max-width: 280px;
    }
}

@media screen and (max-width: 360px) {
    .table_header h1 {
        font-size: 1.1rem;
    }
    
    table thead th {
        padding: 0.5rem;
        font-size: 0.8rem;
    }
    
    table tbody td:first-child {
        font-size: 0.85rem;
        padding-left: 0.5rem;
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: 0.85rem;
        padding: 0.3rem;
    }
    
    .navbar {
        padding: 80px 15px 30px;
    }
    
    .navbar a {
        font-size: 16px;
        padding: 10px;
    }
}

@media screen and (min-width: 1600px) {
    .header {
        padding: 20px 15%;
    }
    
    main.custon_table {
        width: 85%;
        max-width: 1600px;
    }
    
    .banner {
        max-height: min(600px, 45vh);
    }
    
    table tbody td:nth-child(2),
    table tbody td:nth-child(3),
    table tbody td:nth-child(4),
    table tbody td:nth-child(5) {
        font-size: clamp(1.3rem, 2vw, 1.6rem);
    }
}

@media (hover: none) and (pointer: coarse) {
    .navbar a {
        padding: 12px;
        margin: 5px 0;
    }
    
    table thead th,
    table tbody td {
        padding: 12px 8px;
    }
    
    tbody tr {
        min-height: 50px;
    }
    
    .tab {
        min-height: 48px;
        padding: 12px 15px;
    }
}
</style>