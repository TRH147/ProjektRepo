<template>
  <div class="update-page">
    <header class="header">
      <img :src="logoImg" alt="Logo" class="logo" />
      <div class="hamburger" @click="toggleHamburger" :class="{ active: hamburgerActive }">
        <span></span>
        <span></span>
        <span></span>
      </div>
      <nav :class="{ navbar: true, active: hamburgerActive }" @click="handleNavClick">
        <router-link to="/" @click="closeHamburger">Főoldal</router-link>
        <a href="#" class="active">Újdonságok</a>

        <a v-if="store.user" href="#" @click.prevent="checkAuth('/statics')">Statisztika</a>
        <a v-if="store.user" href="#" @click.prevent="checkAuth('/forum')">Fórum</a>
      </nav>
    </header>

    <div class="banner">
      <img src="/src/assets/bigpicture.webp" alt="banner">
    </div>

    <div class="tabs">
      <button class="tab" :class="{active: activeContent==='news'}" @click="showTab('news')">Hírek</button>
      <button class="tab" :class="{active: activeContent==='updates'}" @click="showTab('updates')">Frissítések</button>
    </div>

    <div id="news" class="content" v-show="activeContent==='news'">
      <div class="container1">
        <div class="timeline">
          <ul>
            <li v-for="(item, index) in newsItems" :key="index">
              <div class="timeline-content">
                <h2 class="date">{{ item.date }}</h2>
                <h1>{{ item.title }}</h1>
                <p>{{ item.text }}</p>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <div id="updates" class="content" v-show="activeContent==='updates'">
      <div class="container2">
        <div class="timeline2">
          <ul>
            <li v-for="(item, index) in updatesItems" :key="index">
              <div class="timeline-content2">
                <h2 class="date2">{{ item.date }}</h2>
                <h1>{{ item.title }}</h1>
                <ul>
                  <li v-for="(li, idx) in item.list" :key="idx">{{ li }}</li>
                </ul>
              </div>
              <hr v-if="index<updatesItems.length-1">
            </li>
          </ul>
        </div>
      </div>
    </div>

    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from '../components/SiteFooter.vue'
import { ref } from 'vue'
import { useUserStore } from '../stores/user'
import { useRouter } from 'vue-router'
import logoImg from '/src/assets/Logo.png'

const router = useRouter()
const store = useUserStore()
const activeContent = ref('news')
const hamburgerActive = ref(false)

function toggleHamburger() { 
  hamburgerActive.value = !hamburgerActive.value
  if (hamburgerActive.value) {
    document.body.classList.add('nav-open')
  } else {
    document.body.classList.remove('nav-open')
  }
}

function closeHamburger() { 
  hamburgerActive.value = false
  document.body.classList.remove('nav-open')
}

function handleNavClick(e) {
  if (hamburgerActive.value && !e.target.closest('.navbar')) {
    closeHamburger()
  }
}

function showTab(tab) { 
  activeContent.value = tab 
  closeHamburger()
}

function checkAuth(path) {
  if (!store.user) {
    alert('Bejelentkezés szükséges')
  } else {
    router.push(path)
    closeHamburger()
  }
}

const newsItems = [
  { date: '2026. Augusztus 2.', title: 'Tesztelési lehetőség', text: 'Szeretnél már most belelátni a jövőbe? Októberben zárt bétát indítunk a Search and Destroy módhoz. Jelentkezz a szerverünkön, hogy tesztelhess!' },
  { date: '2026. Június 18.', title: 'Tervek 2026 nyarára', text: 'Merre tovább CombatMater? Mutatjuk a nyári ütemtervet: Deathmatch+ mód finomhangolása, új low-poly pálya és egy exkluzív fegyver skin érkezik.' },
  { date: '2026. Május 20.', title: 'Előkészületben: Search and Destroy', text: 'Már dolgozunk a következő nagy játékmódunkon! Készülj fel a taktikaiabb, lassabb tempójú "Search and Destroy" élményre, ahol a csapatmunka lesz a kulcs.' },
  { date: '2026. Április 12.', title: 'Új pályák érkeznek: Raktár és Erdei', text: 'Két vadonatúj low-poly arénával bővül a CombatMater! A szűk folyosókkal teli "Raktár" és a nyílt, napfényes "Erdei" teljesen új taktikákra kényszeríti majd a játékosokat. Melyik pálya lesz a kedvenced?' },
  { date: '2026. Március 3.', title: 'Betekintés: Karakterdizájnok', text: 'Izgalmas sneak peek! Mutatjuk az első koncepcióterveket a jövőbeli karakterekhez. Célunk, hogy minden játékos megtalálja a saját stílusát az arénában.' },
  { date: '2026. Február 10.', title: 'Új fegyverek érkeznek', text: 'A közösségi visszajelzések alapján hamarosan két új fegyverrel bővül a CombatMater arzenálja. Készülj a taktikai váltásokra és durva új hanghatásokra!' },
  { date: '2026. Január 28.', title: 'CombatMater hivatalosan is ÉL!', text: 'Véget ért a fejlesztési időszak, a CombatMater mostantól mindenki számára elérhető! Merülj el a low-poly arénákban és próbáld ki a deathmatch játékmódot. A kemény munka meghozta gyümölcsét, de a fejlesztés nem áll le!' },
]

const updatesItems = [
  { 
    date: '2026. Május 10.',
    title: 'JÁTÉKMENET', 
    list: [
      'Deathmatch módban finomhangoltuk a spawn védelmi időt 3 másodpercről 2 másodpercre a gyorsabb játékmenet érdekében.',
      'Javítottuk a fegyverek újratöltési animációjának szinkronizációját, így a hang és a vizuális effekt pontosabban egyezik.',
      'Beállítottuk a fegyvercsere sebességét, hogy természetesebb érzetet adjon közelharc közben.'
    ] 
  },
  { 
    date: '2026. Március 25.', 
    title: 'HANG', 
    list: [
      'Új lövéshangokat kapott az M4A1 és az AK-47, élethűbb visszhanghatással a Sivatagi pálya nyílt terein.',
      'Javítottuk a lépéshangok irányhallgatását homokos felületeken, így pontosan behatárolható az ellenség pozíciója.',
      'Különféle hangerő-egyensúlyozások a fegyverek és a szél környezeti hangjai között.'
    ] 
  },
  { 
    date: '2026. Március 15.', 
    title: 'PÁLYA - SIVATAG', 
    list: [
      'Átdolgoztuk a központi épület környékét, új ládákkal bővítve a fedezékek számát.',
      'Javítottuk a textúrák betöltését a romos épületeknél, megszüntetve a FPS-ingadozást.',
      'Optimalizáltuk a pálmafák és bokrok renderelését, növelve a teljesítményt gyengébb gépeken is.',
      'Javítottuk a napfény beesésének irányát, hogy ne vakítsa el a játékosokat bizonyos szögekből.'
    ] 
  },
  { 
    date: '2026. Február 15.', 
    title: 'SPAWN PONTOK', 
    list: [
      'Sivatag pályán áthelyeztünk 5 spawn pontot, hogy elkerüljük a spawn killing lehetőségét.',
      'Eltávolítottunk 3 spawn pontot, ahol a játékosok korábban egymásba spawnolhattak.',
      'Új spawn pontokat adtunk a pálya távolabbi részein, hogy változatosabb legyen az éledési helyszín.'
    ] 
  },
  { 
    date: '2026. Február 6.', 
    title: 'EGYÉB JAVÍTÁSOK', 
    list: [
      'Kijavítottunk egy ritka hibát, ahol a játékosok beragadhattak egy doboz és egy fa közé.',
      'Különféle stabilitási fejlesztések és memóriaoptimalizálások a szerveroldali teljesítmény növelésére.',
      'Javítottuk a találati effektek megjelenését, ha homokfelületet ér a lövés.'
    ] 
  },
]
</script>

<style scoped>
@import "/src/assets/update.css";
</style>