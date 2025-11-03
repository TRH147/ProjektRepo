<template>
    <div id="card-area">
        <div class="wrapper">
            <div class="box-area">
                <div class="box" v-for="card in cards" :key="card.id">
                    <img :src="card.img" alt="" />
                    <div class="overlay">
                        <h3>{{ card.title }}</h3>
                        <p v-if="card.desc">{{ card.desc }}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script setup>
const cards = [
    { id: 1, img: '/src/assets/13.webp', title: 'Fegyverek', desc: 'Alap felszerelés' },
    { id: 2, img: '/src/assets/12.webp', title: 'Fegyverek', desc: 'Alap felszerelés' },
    { id: 3, img: '/src/assets/3.webp', title: 'Coming Soon!' },
    { id: 4, img: '/src/assets/5.webp', title: 'Coming Soon!' },
]
</script>