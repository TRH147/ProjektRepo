<template>
  <div>

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
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum" class="active">Fórum</router-link>
      </nav>
    </header>

    <div class="banner">
      <img src="/src/assets/bigpicture.webp" alt="banner">
    </div>


    <div class="coming-text">
      <h1>Coming Soon!</h1>
    </div>


    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from '../components/SiteFooter.vue'
import { ref } from 'vue'

const menuActive = ref(false)
function toggleMenu() {
  menuActive.value = !menuActive.value
}
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
  background: #1c1c1c;
}


.header {
  position: absolute;
  width: 100%;
  padding: 10px 12.5%;
  background: transparent;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 100;
}

.logo {
  font-size: 2em;
  color: #fae9d7;
  user-select: none;
}

.navbar {
  display: flex;
  align-items: center;
}

.navbar a {
  position: relative;
  font-size: 15px;
  color: #fae9d7;
  text-decoration: none;
  text-transform: uppercase;
  text-shadow: 2px 2px 5px rgba(0,0,0,0.7);
  font-weight: 600;
  margin-right: 30px;
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
  transition: transform .5s;
}

.navbar a:hover::after {
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

.coming-text{
  margin-top: 50px;
  text-align: center;
  color: #fae9d7;
}

.coming-text h1{
  font-size: 100px;
}

@media (max-width: 768px){
  .coming-text h1{
    font-size: 60px;
  }
}


.footer{
  position: relative;
  margin-top: 60px;
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

.footer .social a{
  font-size: 24px;
  color: inherit;
  border: 1px solid #1c1c1c;
  width: 40px;
  height: 40px;
  line-height: 43px;
  display: inline-block;
  text-align: center;
  border-radius: 50%;
  margin: 0 8px;
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