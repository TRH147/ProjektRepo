<template>
    <div id="mod-area">
        <h2 class="section-title">Játékmódok</h2>
        <div class="fedo">
            <div class="doboz-area">
                <div class="doboz" v-for="mode in modes" :key="mode.id">
                    <img :src="mode.img" :alt="mode.title" />
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
import img8 from '/src/assets/8.webp'
import img9 from '/src/assets/9.webp'
import img14 from '/src/assets/14.webp'

const modes = [
    { 
        id: 1, 
        img: img8,  
        title: 'DeathMatch', 
        desc: 'A tipikus, klasszikus játékmód, ahol csapatod pontszáma számít! Ugorj egy meccsbe, ahol folytonosan pörög az akció, lövöldözés és újraéledés 4 percen keresztül!' 
    },
    { 
        id: 2, 
        img: img9, 
        title: 'Search and Destroy' 
    },
    { 
        id: 3, 
        img: img14, 
        title: 'DeathMatch+' 
    },
]
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

#mod-area {
    padding: clamp(30px, 5vw, 60px) 0;
    background: #1c1c1c;
}

.section-title {
    color: #fae9d7;
    text-align: center;
    margin-bottom: clamp(20px, 4vw, 40px);
    font-size: clamp(28px, 4vw, 42px);
    text-transform: uppercase;
    letter-spacing: 2px;
    font-weight: 800;
    padding: 0 20px;
}

.fedo {
    padding: 0 clamp(5%, 8vw, 10%);
}

.doboz-area {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: clamp(20px, 4vw, 40px);
    margin-top: 20px;
}

.doboz {
    border-radius: 15px;
    position: relative;
    overflow: hidden;
    box-shadow: 
        0 10px 30px rgba(0, 0, 0, 0.4),
        0 5px 15px rgba(0, 0, 0, 0.3);
    transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    height: 400px;
    background: #2a2a2a;
}

.doboz:hover {
    transform: translateY(-10px);
    box-shadow: 
        0 20px 40px rgba(0, 0, 0, 0.6),
        0 10px 25px rgba(213, 80, 83, 0.3);
}

.doboz img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
    border-radius: 15px;
    transition: transform 0.6s ease;
}

.doboz:hover img {
    transform: scale(1.05);
}

.overlay2 {
    position: absolute;
    inset: 0;
    background: linear-gradient(
        to top,
        rgba(0, 0, 0, 0.9) 0%,
        rgba(0, 0, 0, 0.7) 30%,
        rgba(0, 0, 0, 0.4) 60%,
        transparent 100%
    );
    border-radius: 15px;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;
    padding: clamp(20px, 4vw, 30px);
    text-align: center;
    color: #fae9d7;
    opacity: 0;
    transition: opacity 0.4s ease;
}

.doboz:hover .overlay2 {
    opacity: 1;
}

.overlay2 h3 {
    font-weight: 700;
    margin-bottom: 15px;
    font-size: clamp(24px, 3vw, 32px);
    letter-spacing: 1.5px;
    text-transform: uppercase;
    color: #fae9d7;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
    transition: color 0.3s ease;
}

.doboz:hover .overlay2 h3 {
    color: #d55053;
}

.overlay2 p {
    margin-top: 15px;
    font-size: clamp(14px, 1.5vw, 16px);
    line-height: 1.6;
    opacity: 0.9;
    max-width: 500px;
    transition: all 0.3s ease 0.1s;
    transform: translateY(20px);
}

.doboz:hover .overlay2 p {
    transform: translateY(0);
}

.overlay2 .p2 {
    margin-top: 30px;
    font-weight: 800;
    font-size: clamp(28px, 3vw, 36px);
    color: #d55053;
    text-shadow: 0 0 10px rgba(213, 80, 83, 0.5);
    opacity: 0;
    transform: scale(0.8);
    transition: all 0.4s ease 0.2s;
}

.doboz:hover .overlay2 .p2 {
    opacity: 1;
    transform: scale(1);
    animation: pulseGlow 2s infinite;
}

@keyframes pulseGlow {
    0%, 100% {
        text-shadow: 0 0 10px rgba(213, 80, 83, 0.5);
    }
    50% {
        text-shadow: 0 0 20px rgba(213, 80, 83, 0.8);
    }
}

@media (max-width: 1200px) {
    .fedo {
        padding: 0 5%;
    }
    
    .doboz-area {
        grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    }
    
    .doboz {
        height: 380px;
    }
}

@media (max-width: 1024px) {
    .doboz-area {
        grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
        gap: 30px;
    }
    
    .doboz {
        height: 360px;
    }
    
    .overlay2 {
        padding: 25px;
    }
}

@media (max-width: 900px) {
    .doboz-area {
        grid-template-columns: repeat(2, 1fr);
    }
    
    .doboz {
        height: 350px;
    }
}

@media (max-width: 768px) {
    #mod-area {
        padding: 40px 0;
    }
    
    .section-title {
        font-size: 32px;
        margin-bottom: 30px;
    }
    
    .fedo {
        padding: 0 20px;
    }
    
    .doboz-area {
        grid-template-columns: 1fr;
        max-width: 500px;
        margin-left: auto;
        margin-right: auto;
        gap: 25px;
    }
    
    .doboz {
        height: 400px;
        max-width: 500px;
        margin: 0 auto;
    }
    
    .overlay2 {
        opacity: 0.9;
        background: linear-gradient(
            to top,
            rgba(0, 0, 0, 0.95) 0%,
            rgba(0, 0, 0, 0.8) 50%,
            rgba(0, 0, 0, 0.6) 100%
        );
    }
    
    .doboz:hover .overlay2 {
        opacity: 1;
    }
    
    .overlay2 h3 {
        font-size: 28px;
    }
    
    .overlay2 p {
        font-size: 15px;
        transform: translateY(0);
        opacity: 1;
    }
    
    .overlay2 .p2 {
        font-size: 30px;
        opacity: 1;
        transform: scale(1);
    }
}

@media (max-width: 600px) {
    .section-title {
        font-size: 28px;
        margin-bottom: 25px;
    }
    
    .doboz {
        height: 350px;
    }
    
    .overlay2 {
        padding: 20px;
    }
    
    .overlay2 h3 {
        font-size: 24px;
    }
    
    .overlay2 p {
        font-size: 14px;
    }
    
    .overlay2 .p2 {
        font-size: 26px;
    }
}

@media (max-width: 480px) {
    #mod-area {
        padding: 30px 0;
    }
    
    .section-title {
        font-size: 24px;
        margin-bottom: 20px;
    }
    
    .fedo {
        padding: 0 15px;
    }
    
    .doboz {
        height: 300px;
        border-radius: 12px;
    }
    
    .doboz img {
        border-radius: 12px;
    }
    
    .overlay2 {
        padding: 15px;
        border-radius: 12px;
    }
    
    .overlay2 h3 {
        font-size: 22px;
        margin-bottom: 10px;
    }
    
    .overlay2 p {
        font-size: 13px;
        margin-top: 10px;
    }
    
    .overlay2 .p2 {
        font-size: 22px;
        margin-top: 20px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .doboz {
        height: 280px;
    }
    
    .overlay2 {
        padding: 15px;
    }
    
    .overlay2 h3 {
        font-size: 20px;
        margin-bottom: 8px;
    }
    
    .overlay2 p {
        font-size: 12px;
        margin-top: 8px;
    }
    
    .overlay2 .p2 {
        font-size: 20px;
        margin-top: 15px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .doboz {
        transform: none !important;
    }
    
    .doboz:hover {
        transform: none !important;
    }
    
    .overlay2 {
        opacity: 0.9;
    }
    
    .doboz:hover .overlay2 {
        opacity: 1;
    }
    
    .doboz:hover img {
        transform: scale(1.02);
    }
    
    .overlay2 p,
    .overlay2 .p2 {
        transform: none !important;
        opacity: 1 !important;
    }
}
</style>