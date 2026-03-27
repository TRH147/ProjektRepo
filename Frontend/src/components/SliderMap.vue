<template>
    <div class="container2">
        <h2 class="section-title">Pályáink</h2>
        <div class="container2-inner">
            <div class="slider-wrapper" ref="sliderWrapper">
                <div class="slide-track" :style="{ transform: `translateX(${translateX}%)` }">
                    <div v-for="(item, i) in items" :key="item.id" class="slide-item"
                        :style="{ backgroundImage: `url(${item.img})` }"
                        :class="{ active: currentIndex === i }">
                        <div class="slide-content">
                            <div class="name">{{ item.name }}</div>
                            <div class="des">{{ item.des }}</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="slider-controls">
                <button class="prev" @click="prev" aria-label="Previous slide">
                    <i class='bx bx-chevron-left'></i>
                </button>
                
                <div class="slide-dots">
                    <button v-for="(item, i) in items" :key="`dot-${item.id}`" 
                        class="dot" 
                        :class="{ active: currentIndex === i }"
                        @click="goToSlide(i)"
                        :aria-label="`Go to slide ${i + 1}`">
                    </button>
                </div>
                
                <button class="next" @click="next" aria-label="Next slide">
                    <i class='bx bx-chevron-right'></i>
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const items = ref([
    { id: 1, img: '/src/assets/1.webp', name: 'Sivatagi', des: 'Taktikai a harc a sivatagi falu szűk utcáin, ahol minden tetőpont egy potenciális mesterlövészfészek.' },
    { id: 2, img: '/src/assets/2.webp', name: 'Raktár', des: 'Rozsdás konténerek, sötét folyosók és minden sarok mögött leselkedő ellenfelek.' },
    { id: 3, img: '/src/assets/10.webp', name: 'Állomás', des: 'A peronokon visszhangzik a lövések hangja, a szerelvények között pedig mindenlépés veszélyt rejt.' },
    { id: 4, img: '/src/assets/4.webp', name: 'Városi', des: 'Coming Soon!' },
    { id: 5, img: '/src/assets/6.webp', name: 'Erdei', des: 'Coming Soon!' },
    { id: 6, img: '/src/assets/11.webp', name: 'Építkezés', des: 'Coming Soon!' },
])

const currentIndex = ref(0)
const translateX = ref(0)
const autoSlideInterval = ref(null)

const calculateTranslateX = () => {
    translateX.value = -currentIndex.value * 100
}

function next() {
    currentIndex.value = (currentIndex.value + 1) % items.value.length
    calculateTranslateX()
    resetAutoSlide()
}

function prev() {
    currentIndex.value = (currentIndex.value - 1 + items.value.length) % items.value.length
    calculateTranslateX()
    resetAutoSlide()
}

function goToSlide(index) {
    currentIndex.value = index
    calculateTranslateX()
    resetAutoSlide()
}

function startAutoSlide() {
    autoSlideInterval.value = setInterval(() => {
        next()
    }, 5000) 
}

function resetAutoSlide() {
    if (autoSlideInterval.value) {
        clearInterval(autoSlideInterval.value)
        startAutoSlide()
    }
}

function pauseAutoSlide() {
    if (autoSlideInterval.value) {
        clearInterval(autoSlideInterval.value)
    }
}

function resumeAutoSlide() {
    if (!autoSlideInterval.value) {
        startAutoSlide()
    }
}

onMounted(() => {
    startAutoSlide()
})

onUnmounted(() => {
    if (autoSlideInterval.value) {
        clearInterval(autoSlideInterval.value)
    }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');

.container2 {
    width: 90%;
    max-width: 1200px;
    margin: 40px auto 70px;
    position: relative;
    overflow: hidden;
}

.section-title {
    color: #fae9d7;
    text-align: center;
    margin-bottom: 30px;
    font-size: clamp(28px, 4vw, 40px);
    text-transform: uppercase;
    letter-spacing: 2px;
    font-weight: 800;
}

.container2-inner {
    position: relative;
    width: 100%;
    background: #1c1c1c;
    border-radius: 20px;
    overflow: hidden;
    padding: 20px;
}

.slider-wrapper {
    position: relative;
    width: 100%;
    height: 500px;
    overflow: hidden;
    border-radius: 15px;
    margin-bottom: 20px;
}

.slide-track {
    display: flex;
    height: 100%;
    transition: transform 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94);
    will-change: transform;
}

.slide-item {
    flex: 0 0 100%;
    height: 100%;
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;
    position: relative;
    display: flex;
    align-items: flex-end;
    justify-content: center;
    padding: 30px;
    min-width: 100%;
}

.slide-item::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(
        to top,
        rgba(0, 0, 0, 0.8) 0%,
        rgba(0, 0, 0, 0.4) 50%,
        rgba(0, 0, 0, 0.2) 100%
    );
    z-index: 1;
}

.slide-content {
    position: relative;
    z-index: 2;
    text-align: center;
    color: #fae9d7;
    max-width: 600px;
    padding: 30px;
    background: rgba(28, 28, 28, 0.7);
    backdrop-filter: blur(10px);
    border-radius: 15px;
    animation: slideUp 0.8s ease-out;
}

.slide-content .name {
    font-size: clamp(28px, 3vw, 42px);
    text-transform: uppercase;
    font-weight: bold;
    margin-bottom: 15px;
    color: #d55053;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
    letter-spacing: 1px;
}

.slide-content .des {
    font-size: clamp(16px, 1.5vw, 20px);
    line-height: 1.6;
    opacity: 0.9;
    margin: 0 auto;
}

.slider-controls {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 20px;
    position: relative;
}

.prev, .next {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    border: 2px solid #fae9d7;
    background: rgba(28, 28, 28, 0.8);
    color: #fae9d7;
    cursor: pointer;
    font-size: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.3s ease;
    flex-shrink: 0;
    z-index: 10;
}

.prev:hover, .next:hover {
    background: #d55053;
    transform: scale(1.1);
    border-color: #d55053;
}

.prev:active, .next:active {
    transform: scale(0.95);
}

.slide-dots {
    display: flex;
    gap: 12px;
    justify-content: center;
    flex: 1;
    max-width: 400px;
    margin: 0 20px;
}

.dot {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    background: rgba(250, 233, 215, 0.3);
    border: none;
    cursor: pointer;
    transition: all 0.3s ease;
    padding: 0;
}

.dot:hover {
    background: rgba(250, 233, 215, 0.6);
    transform: scale(1.2);
}

.dot.active {
    background: #d55053;
    transform: scale(1.3);
    box-shadow: 0 0 10px rgba(213, 80, 83, 0.5);
}

@keyframes slideUp {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@media (max-width: 1200px) {
    .container2 {
        width: 95%;
    }
    
    .slider-wrapper {
        height: 450px;
    }
}

@media (max-width: 1024px) {
    .container2 {
        width: 98%;
        margin: 30px auto 60px;
    }
    
    .slider-wrapper {
        height: 400px;
    }
    
    .slide-content {
        padding: 25px;
        max-width: 500px;
    }
}

@media (max-width: 768px) {
    .container2 {
        margin: 20px auto 50px;
    }
    
    .container2-inner {
        padding: 15px;
    }
    
    .slider-wrapper {
        height: 350px;
    }
    
    .slide-content {
        padding: 20px;
        max-width: 400px;
        backdrop-filter: blur(5px);
    }
    
    .slide-content .name {
        font-size: 24px;
    }
    
    .slide-content .des {
        font-size: 16px;
    }
    
    .slider-controls {
        padding: 0;
        position: relative;
        min-height: 60px;
    }
    
    .prev, .next {
        width: 45px;
        height: 45px;
        font-size: 20px;
        position: absolute;
        top: 50%;
        transform: translateY(-50%);
        background: rgba(28, 28, 28, 0.9);
        backdrop-filter: blur(5px);
        border-width: 2px;
    }
    
    .prev {
        left: 10px;
    }
    
    .next {
        right: 10px;
    }
    
    .prev:hover, .next:hover {
        transform: translateY(-50%) scale(1.1);
    }
    
    .prev:active, .next:active {
        transform: translateY(-50%) scale(0.95);
    }
    
    .slide-dots {
        gap: 10px;
        margin: 0 auto;
        max-width: 100%;
        padding: 0 60px;
    }
    
    .dot {
        width: 10px;
        height: 10px;
    }
}

@media (max-width: 600px) {
    .container2-inner {
        border-radius: 15px;
        padding: 10px;
    }
    
    .slider-wrapper {
        height: 300px;
        border-radius: 10px;
    }
    
    .slide-item {
        padding: 20px;
    }
    
    .slide-content {
        padding: 15px;
        width: 90%;
        max-width: none;
    }
    
    .slide-content .name {
        font-size: 20px;
        margin-bottom: 8px;
    }
    
    .slide-content .des {
        font-size: 14px;
    }
    
    .slider-controls {
        min-height: 50px;
    }
    
    .prev, .next {
        width: 40px;
        height: 40px;
        font-size: 18px;
    }
    
    .prev {
        left: 5px;
    }
    
    .next {
        right: 5px;
    }
    
    .slide-dots {
        gap: 8px;
        padding: 0 50px;
    }
    
    .dot {
        width: 8px;
        height: 8px;
    }
}

@media (max-width: 480px) {
    .container2 {
        margin: 15px auto 40px;
    }
    
    .section-title {
        font-size: 24px;
        margin-bottom: 20px;
    }
    
    .slider-wrapper {
        height: 250px;
    }
    
    .slide-item {
        padding: 15px;
    }
    
    .slide-content {
        padding: 12px;
    }
    
    .slide-content .name {
        font-size: 18px;
    }
    
    .slide-content .des {
        font-size: 13px;
    }
    
    .prev, .next {
        width: 35px;
        height: 35px;
        font-size: 16px;
    }
    
    .slide-dots {
        gap: 6px;
        padding: 0 45px;
    }
    
    .dot {
        width: 8px;
        height: 8px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .slider-wrapper {
        height: 300px;
    }
    
    .slide-content {
        padding: 20px;
        max-width: 500px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .prev, .next, .dot {
        min-width: 44px;
        min-height: 44px;
    }
    
    .dot {
        min-width: 12px;
        min-height: 12px;
    }
    
    .slider-wrapper {
        touch-action: pan-y pinch-zoom;
    }
}
</style>