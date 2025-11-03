<template>
  <div>

    <header class="header">
      <h2 class="logo">Logo</h2>

      <div class="hamburger" @click="toggleMenu">
        <span></span><span></span><span></span>
      </div>

      <nav :class="['navbar', { active: isMenuActive }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update" class="active">Frissítések</router-link>
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum">Fórum</router-link>
      </nav>
    </header>


    <div class="cim">
      <h1>Frissítési Jegyzetek</h1>
    </div>


    <div class="dobozz">
      <div class="timeline">
        <ul>
          <li>
            <div class="timeline-content">
              <h2 class="date">2025. Október 26.</h2>
              <h1>Lorem</h1>
              <p>
                Lorem ipsum dolor sit amet consectetur adipisicing elit. Animi a laboriosam eos incidunt eaque
                repellendus molestias veritatis ea cum doloribus.
              </p>
            </div>
          </li>
          <li>
            <div class="timeline-content">
              <h2 class="date">2025. Október 26.</h2>
              <h1>Lorem 1</h1>
              <p>
                Lorem ipsum dolor sit amet consectetur adipisicing elit. Animi a laboriosam eos incidunt eaque
                repellendus molestias veritatis ea cum doloribus.
              </p>
            </div>
          </li>
          <li>
            <div class="timeline-content">
              <h2 class="date">2025. Október 26.</h2>
              <h1>Lorem 2</h1>
              <p>
                Lorem ipsum dolor sit amet consectetur adipisicing elit. Animi a laboriosam eos incidunt eaque
                repellendus molestias veritatis ea cum doloribus.
              </p>
            </div>
          </li>
          <li>
            <div class="timeline-content">
              <h2 class="date">2025. Október 26.</h2>
              <h1>Lorem 3</h1>
              <p>
                Lorem ipsum dolor sit amet consectetur adipisicing elit. Animi a laboriosam eos incidunt eaque
                repellendus molestias veritatis ea cum doloribus.
              </p>
            </div>
          </li>
          <li>
            <div class="timeline-content">
              <h2 class="date">2025. Október 26.</h2>
              <h1>Lorem 4</h1>
              <p>
                Lorem ipsum dolor sit amet consectetur adipisicing elit. Animi a laboriosam eos incidunt eaque
                repellendus molestias veritatis ea cum doloribus.
              </p>
            </div>
          </li>
        </ul>
      </div>
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
        Csapatunk 3 junior fejlesztőből áll össze, kiknek célja egy olyan játék fejlesztése, amely lépést tud
        tartani a mai modern videójátékipar termékeivel.
      </p>
    </section>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const isMenuActive = ref(false)
function toggleMenu() {
  isMenuActive.value = !isMenuActive.value
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
  color: #fae9d7;
}


.header {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  padding: 10px 12.5%;
  background: rgba(250, 233, 215, 0.15);
  display: flex;
  justify-content: space-between;
  align-items: center;
  backdrop-filter: blur(10px);
  z-index: 100;
}

.logo {
  font-size: 2em;
  color: #fae9d7;
}

.navbar a {
  font-size: 16px;
  color: #fae9d7;
  margin-right: 30px;
  text-decoration: none;
}
.navbar a:hover,
.navbar a.active {
  color: #e24c4f;
}


.cim {
  margin-top: 100px;
  text-align: center;
}
.cim h1 {
  font-size: 40px;
  color: #fae9d7;
}


.dobozz {
  display: flex;
  justify-content: center;
  padding: 80px 0;
}

.timeline {
  width: 80%;
  max-width: 800px;
}

.timeline ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.timeline ul li {
  position: relative;
  margin-bottom: 30px;
  border-radius: 10px;
  overflow: hidden;
  color: #fae9d7;
  background: rgba(255, 255, 255, 0.05);
}

.timeline ul li::before {
  content: "";
  position: absolute;
  inset: 0;
  background: url('/10.webp') center/cover no-repeat;
  filter: brightness(0.5);
  z-index: 0;
}

.timeline-content {
  position: relative;
  padding: 30px;
  z-index: 1;
}

.timeline-content h1 {
  font-size: 24px;
  margin-bottom: 8px;
  color: #fae9d7;
}
.timeline-content p {
  font-size: 16px;
  line-height: 28px;
}
.timeline-content .date {
  font-size: 13px;
  opacity: 0.8;
  margin-bottom: 10px;
}


.footer {
  margin-top: 60px;
  padding: 40px 0;
  background: #fae9d7;
  color: #1c1c1c;
  text-align: center;
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
</style>