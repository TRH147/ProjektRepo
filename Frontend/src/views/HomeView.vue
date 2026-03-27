<template>
  <main>
    <HeroCover />


    <AuthModal />

    <section>
      <SliderMap />
    </section>

    <section>
      <ModesGrid />
    </section>

    <section>
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