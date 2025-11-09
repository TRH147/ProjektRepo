<template>
    <div id="mod-area">
        <div class="fedo">
            <div class="doboz-area">
                <div class="doboz" v-for="mode in modes" :key="mode.id">
                    <img :src="mode.img" alt="" />
                    <div class="overlay2">
                        <h3>{{ mode.title }}</h3>
                        <p v-if="mode.desc">{{ mode.desc }}</p>
                        <p v-else class="p2">Coming Soon!</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script setup>
const modes = [
    { id: 1, img: '/src/assets/8.webp', title: 'DeathMatch', desc: 'A tipikus, klasszikus játékmód, ahol csapatod pontszáma számít! Ugorj egy meccsbe, ahol folytonosan pörög az akció, lövöldözés és újraéledés 4 percen keresztül!' },
    { id: 2, img: '/src/assets/9.webp', title: 'Search and Destroy' },
    { id: 3, img: '/src/assets/14.webp', title: 'DeathMatch+' },
]
</script>