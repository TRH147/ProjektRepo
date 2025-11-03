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


    <div class="cim">
      <h1>Coming Soon!</h1>
    </div>


    <section class="footer">
      <h3>Kövess minket közösségi médián</h3>
      <div class="social">
        <a href="#"><i class="bx bxl-facebook-circle"></i></a>
        <a href="#"><i class="bx bxl-instagram-alt"></i></a>
        <a href="#"><i class="bx bxl-twitter"></i></a>
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
    </section>
  </div>
</template>

<script setup>
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
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  padding: 10px 12.5%;
  background: #fae9d729;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 100;
  backdrop-filter: blur(10px);
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
  font-size: 16px;
  color: #fae9d7;
  text-decoration: none;
  font-weight: 500;
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


.cim {
  color: #fae9d7;
  font-size: 55px;
  margin-top: 100px;
  text-align: center;
}

.container h1 {
  color: #fae9d7;
  font-size: 100px;
}


.footer {
  padding: 40px 0;
  background: #fae9d7;
  margin-top: 214px;
}

.footer h3 {
  text-align: center;
  color: #1c1c1c;
  margin-bottom: 25px;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.footer h4 {
  text-align: center;
  color: #1c1c1c;
  margin-top: 15px;
  font-size: 18px;
}

.footer .social {
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

.footer .social a:hover {
  color: #d55053;
}

.footer ul {
  list-style: none;
  font-size: 18px;
  line-height: 1.6;
  text-align: center;
  padding: 0;
}

.footer ul li {
  display: inline-block;
  padding: 0 15px;
}

.footer ul li a {
  color: inherit;
  text-decoration: none;
}

.footer ul li a:hover {
  color: #d55053;
}

.footer .copyright {
  margin-top: 15px;
  text-align: center;
  font-size: 15px;
  color: #1c1c1c;
}
</style>