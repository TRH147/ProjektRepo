<template>
  <div class="page">
    
    <header class="header">
      <h2 class="logo">Logo</h2>

      <div class="hamburger" @click="toggleMenu">
        <span></span>
        <span></span>
        <span></span>
      </div>

      <nav :class="['navbar', { active: menuActive }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update">Frissítések</router-link>
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
              <th @click="sortTable(1)">Játszott meccsek <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(2)">Győzelem <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(3)">Vereség <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
              <th @click="sortTable(4)">Gyilkosságok <span class="icon-arrow"><i class='bx bx-up-arrow-alt'></i></span></th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="(player, i) in sortedData" :key="i">
              <td>{{ player.name }}</td>
              <td>{{ player.played }}</td>
              <td>{{ player.wins }}</td>
              <td>{{ player.losses }}</td>
              <td>{{ player.kills }}</td>
            </tr>
          </tbody>
        </table>
      </section>

      <div v-if="sortedData.length === 0" class="no-match-message">No matching data found</div>
    </main>

    
    <footer class="footer">
      <h3>Kövess minket közösségi médián</h3>
      <div class="social">
        <a href="#"><i class='bx bxl-facebook-circle'></i></a>
        <a href="#"><i class='bx bxl-instagram-alt'></i></a>
        <a href="#"><i class='bx bxl-twitter'></i></a>
      </div>

      <ul class="list">
        <li><router-link to="/">Főoldal</router-link></li>
        <li><router-link to="/update">Frissítések</router-link></li>
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
import { ref, computed } from 'vue'


const menuActive = ref(false)
function toggleMenu() {
  menuActive.value = !menuActive.value
}

const data = ref([
  { name: "A", played: 10, wins: 8, losses: 2, kills: 22 },
  { name: "B", played: 5, wins: 1, losses: 4, kills: 3 },
  { name: "C", played: 23, wins: 15, losses: 8, kills: 10 },
  { name: "D", played: 1, wins: 0, losses: 1, kills: 6 },
  { name: "E", played: 34, wins: 4, losses: 30, kills: 3 },
  { name: "F", played: 4, wins: 4, losses: 0, kills: 10 }
])


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

  const keyMap = ["name", "played", "wins", "losses", "kills"]
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
    position: absolute;
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

.navbar a{
    position: relative;
    font-size: 15px;
    color: #fae9d7;
    text-decoration: none;
    text-transform: uppercase;
    text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
    font-weight: 600;
    margin-right: 30px;
}

.navbar a:hover{
    color: #e24c4f;
}

.navbar a.active{
    color: #e24c4f;
}

.navbar a::after{
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
    transition: transform .5s;
}

.navbar a:hover::after{
    transform-origin: left;
    transform: scaleX(1);
}

.hamburger {
    display: none;
    flex-direction: column;
    justify-content: space-between;
    width: 26px;
    height: 20px;
    cursor: pointer;
    z-index: 101;
}

.hamburger span {
    height: 3px;
    width: 100%;
    background: #fae9d7;
    border-radius: 2px;
    transition: 0.3s;
}

@media (max-width: 730px), (max-height: 480px) {
    .hamburger {
        display: flex;
    }

    .navbar {
        position: absolute;
        top: 100%;
        left: 0;
        width: 100%;
        flex-direction: column;
        background: rgba(20, 20, 20, 0.95);
        text-align: center;
        padding: 20px 0;
        gap: 20px;
        transform: translateY(-200%);
        transition: transform 0.4s ease;
    }

    .navbar.active {
        transform: translateY(0);
    }

    .navbar a {
        margin: 0;
    }

    .hamburger.active span:nth-child(1) {
        transform: rotate(45deg) translate(4px, 6px);
    }
    .hamburger.active span:nth-child(2) {
        opacity: 0;
    }
    .hamburger.active span:nth-child(3) {
        transform: rotate(-45deg) translate(6px, -7px);
    }
}

.banner {
  width: 100%;
  max-height: 500px;
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
}

.banner img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}



main{
    width: 82vw;
    height: 50vh;
    background: rgba(0, 0, 0, 0.781);
    border: 2px solid #fae9d7;
    backdrop-filter: blur(7px);
    box-shadow: 0 .4rem .8rem black;
    border-radius: 0.9rem;
    margin: 100px auto 0 auto;
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
