<template>
    <div class="container2">
        <div class="container2-inner">
            <div class="slide" ref="slideRef">
                <div v-for="(item, i) in items" :key="item.id" class="item"
                    :style="{ backgroundImage: `url(${item.img})` }">
                    <div class="content2">
                        <div class="name">{{ item.name }}</div>
                        <div class="des">{{ item.des }}</div>
                    </div>
                </div>
            </div>


            <div class="button">
                <button class="prev" @click="prev"><i class='bx bx-chevron-left'></i></button>
                <button class="next" @click="next"><i class='bx bx-chevron-right'></i></button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'


const items = ref([
    { id: 1, img: '/src/assets/1.webp', name: 'Sivatagi', des: 'Egy pár pálya, amin a csaták zajlanak.' },
    { id: 2, img: '/src/assets/2.png', name: 'Raktár', des: 'Egy pár pálya, amin a csaták zajlanak.' },
    { id: 3, img: '/src/assets/4.png', name: 'Városi', des: 'Egy pár pálya, amin a csaták zajlanak.' },
    { id: 4, img: '/src/assets/10.png', name: 'Pályaudvar', des: 'Egy pár pálya, amin a csaták zajlanak.' },
    { id: 5, img: '/src/assets/6.png', name: 'Erdei', des: 'Egy pár pálya, amin a csaták zajlanak.' },
    { id: 6, img: '/src/assets/11.png', name: 'Építkezés', des: 'Egy pár pálya, amin a csaták zajlanak.' },
])


const slideRef = ref(null)


function next() {

    const first = items.value.shift()
    items.value.push(first)
}
function prev() {
    const last = items.value.pop()
    items.value.unshift(last)
}
</script>