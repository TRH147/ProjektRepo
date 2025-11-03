<template>
    <section class="footer">
        <h3>Kövess minket közösségi médián</h3>
        <div class="social">
            <a href="#"><i class='bx bxl-facebook-circle'></i></a>
            <a href="#"><i class='bx bxl-instagram-alt'></i></a>
            <a href="#"><i class='bx bxl-twitter'></i></a>
        </div>


        <ul class="list">
            <li><router-link to="/" @click.native="scrollToTop">Főoldal</router-link></li>
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
</template>


<script setup>
function scrollToTop() {
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
</script>