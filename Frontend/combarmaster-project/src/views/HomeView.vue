<template>
  <main>
    <HeroCover />


    <AuthModal />

    <section>
      <h1 class="palya">Pályáink</h1>
      <SliderMap />
    </section>

    <section>
      <h1 class="palya">Játék módok</h1>
      <ModesGrid />
    </section>

    <section>
      <h1 class="palya">Felszerelések</h1>
      <EquipCards />
    </section>
  </main>
</template>

<script setup>
import HeroCover from '../components/HeroCover.vue'
import AuthModal from '../components/AuthModal.vue'
import SliderMap from '../components/SliderMap.vue'
import ModesGrid from '../components/ModesGrid.vue'
import EquipCards from '../components/EquipCards.vue'
</script>

<style scoped>
.palya {
  text-align: center;
  color: #fae9d7;
  font-size: 2rem;
  margin: 60px 0 30px;
}
</style>