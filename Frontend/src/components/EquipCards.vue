<template>
    <div id="card-area">
        <h2 class="section-title">Felszerelések</h2>
        <div class="wrapper">
            <div class="box-area">
                <div class="box" v-for="card in cards" :key="card.id">
                    <img :src="card.img" :alt="card.title" />
                    <div class="overlay">
                        <h3>{{ card.title }}</h3>
                        <p v-if="card.desc">{{ card.desc }}</p>
                        <div v-else class="coming-soon">
                            <span>Coming Soon!</span>
                        </div>
                    </div>
                    <div class="card-badge" v-if="card.desc">Available</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import img13 from '/src/assets/13.webp'
import img12 from '/src/assets/12.webp'
import img3 from '/src/assets/3.webp'
import img5 from '/src/assets/5.webp'

const cards = [
    { id: 1, img: img13, title: 'Fegyverek', desc: 'Alap felszerelés' },
    { id: 2, img: img12, title: 'Fegyverek', desc: 'Alap felszerelés' },
    { id: 3, img: img3},
    { id: 4, img: img5},
]
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');

#card-area {
    padding: clamp(40px, 6vw, 70px) 0;
    background: #1c1c1c;
    position: relative;
}

.section-title {
    color: #fae9d7;
    text-align: center;
    margin-bottom: clamp(30px, 5vw, 50px);
    font-size: clamp(32px, 4vw, 44px);
    text-transform: uppercase;
    letter-spacing: 2px;
    font-weight: 800;
    padding: 0 20px;
}

.wrapper {
    padding: 0 clamp(5%, 8vw, 10%);
}

.box-area {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: clamp(25px, 4vw, 40px);
    margin-top: 20px;
}

.box {
    border-radius: 16px;
    position: relative;
    overflow: hidden;
    box-shadow: 
        0 12px 35px rgba(0, 0, 0, 0.5),
        0 6px 20px rgba(0, 0, 0, 0.3),
        inset 0 0 0 1px rgba(250, 233, 215, 0.1);
    transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    height: 350px;
    background: #2a2a2a;
    isolation: isolate;
}

.box:hover {
    transform: translateY(-12px);
    box-shadow: 
        0 25px 50px rgba(0, 0, 0, 0.7),
        0 15px 35px rgba(213, 80, 83, 0.2),
        inset 0 0 0 1px rgba(250, 233, 215, 0.2);
}

.box img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
    border-radius: 16px;
    transition: all 0.6s ease;
    filter: brightness(0.9);
}

.box:hover img {
    transform: scale(1.08);
    filter: brightness(1.1);
}

.overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(
        to bottom,
        transparent 0%,
        rgba(0, 0, 0, 0.1) 20%,
        rgba(0, 0, 0, 0.3) 40%,
        rgba(0, 0, 0, 0.6) 60%,
        rgba(0, 0, 0, 0.8) 80%,
        rgba(0, 0, 0, 0.9) 100%
    );
    border-radius: 16px;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;
    padding: clamp(20px, 4vw, 35px);
    text-align: center;
    color: #fae9d7;
    opacity: 0;
    transition: opacity 0.4s ease;
}

.box:hover .overlay {
    opacity: 1;
}

.overlay h3 {
    font-weight: 700;
    margin-bottom: 12px;
    font-size: clamp(26px, 3vw, 34px);
    letter-spacing: 1.5px;
    text-transform: uppercase;
    color: #fae9d7;
    text-shadow: 2px 2px 6px rgba(0, 0, 0, 0.7);
    transform: translateY(20px);
    transition: all 0.4s ease;
}

.box:hover .overlay h3 {
    transform: translateY(0);
    color: #d55053;
}

.overlay p {
    font-size: clamp(15px, 1.6vw, 17px);
    line-height: 1.5;
    opacity: 0.9;
    max-width: 300px;
    transform: translateY(20px);
    transition: all 0.4s ease 0.1s;
}

.box:hover .overlay p {
    transform: translateY(0);
}

.coming-soon {
    position: relative;
    margin-top: 20px;
    padding: 15px 25px;
    background: rgba(28, 28, 28, 0.8);
    border-radius: 30px;
    border: 2px solid rgba(213, 80, 83, 0.3);
    transform: scale(0.8);
    opacity: 0;
    transition: all 0.4s ease 0.2s;
}

.box:hover .coming-soon {
    transform: scale(1);
    opacity: 1;
    border-color: rgba(213, 80, 83, 0.6);
    box-shadow: 0 0 20px rgba(213, 80, 83, 0.3);
}

.coming-soon span {
    font-size: clamp(20px, 2.5vw, 26px);
    font-weight: 800;
    color: #d55053;
    text-transform: uppercase;
    letter-spacing: 2px;
    text-shadow: 0 0 10px rgba(213, 80, 83, 0.5);
    animation: glowPulse 2s infinite;
}

@keyframes glowPulse {
    0%, 100% {
        text-shadow: 0 0 10px rgba(213, 80, 83, 0.5);
    }
    50% {
        text-shadow: 0 0 20px rgba(213, 80, 83, 0.8);
    }
}

.card-badge {
    position: absolute;
    top: 20px;
    right: 20px;
    background: linear-gradient(135deg, #d55053, #e24c4f);
    color: #fae9d7;
    padding: 6px 15px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 700;
    letter-spacing: 1px;
    text-transform: uppercase;
    box-shadow: 0 4px 15px rgba(213, 80, 83, 0.4);
    opacity: 0;
    transform: translateX(20px);
    transition: all 0.4s ease 0.1s;
    z-index: 2;
}

.box:hover .card-badge {
    opacity: 1;
    transform: translateX(0);
}

@media (max-width: 1200px) {
    .wrapper {
        padding: 0 5%;
    }
    
    .box-area {
        grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
    }
    
    .box {
        height: 330px;
    }
}

@media (max-width: 1024px) {
    .box-area {
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: 30px;
    }
    
    .box {
        height: 320px;
    }
    
    .overlay {
        padding: 25px;
    }
}

@media (max-width: 900px) {
    .box-area {
        grid-template-columns: repeat(2, 1fr);
    }
    
    .box {
        height: 300px;
    }
}

@media (max-width: 768px) {
    #card-area {
        padding: 50px 0;
    }
    
    .section-title {
        font-size: 36px;
        margin-bottom: 40px;
    }
    
    .wrapper {
        padding: 0 25px;
    }
    
    .box-area {
        grid-template-columns: 1fr;
        max-width: 500px;
        margin-left: auto;
        margin-right: auto;
        gap: 30px;
    }
    
    .box {
        height: 350px;
        max-width: 500px;
        margin: 0 auto;
    }
    
    .overlay {
        opacity: 0.9;
        background: linear-gradient(
            to bottom,
            transparent 0%,
            rgba(0, 0, 0, 0.3) 30%,
            rgba(0, 0, 0, 0.6) 60%,
            rgba(0, 0, 0, 0.8) 80%,
            rgba(0, 0, 0, 0.95) 100%
        );
    }
    
    .box:hover .overlay {
        opacity: 1;
    }
    
    .overlay h3 {
        font-size: 30px;
        transform: translateY(0);
    }
    
    .overlay p {
        font-size: 16px;
        transform: translateY(0);
        opacity: 1;
    }
    
    .coming-soon {
        opacity: 1;
        transform: scale(1);
    }
    
    .card-badge {
        opacity: 1;
        transform: translateX(0);
    }
}

@media (max-width: 600px) {
    .section-title {
        font-size: 30px;
        margin-bottom: 35px;
    }
    
    .box {
        height: 320px;
    }
    
    .overlay {
        padding: 20px;
    }
    
    .overlay h3 {
        font-size: 26px;
    }
    
    .overlay p {
        font-size: 15px;
    }
    
    .coming-soon {
        padding: 12px 20px;
    }
    
    .coming-soon span {
        font-size: 22px;
    }
    
    .card-badge {
        top: 15px;
        right: 15px;
        padding: 5px 12px;
        font-size: 11px;
    }
}

@media (max-width: 480px) {
    #card-area {
        padding: 40px 0;
    }
    
    .section-title {
        font-size: 26px;
        margin-bottom: 30px;
    }
    
    .wrapper {
        padding: 0 20px;
    }
    
    .box {
        height: 280px;
        border-radius: 14px;
    }
    
    .box img {
        border-radius: 14px;
    }
    
    .overlay {
        padding: 15px;
        border-radius: 14px;
    }
    
    .overlay h3 {
        font-size: 22px;
        margin-bottom: 10px;
    }
    
    .overlay p {
        font-size: 14px;
    }
    
    .coming-soon {
        padding: 10px 18px;
        margin-top: 15px;
    }
    
    .coming-soon span {
        font-size: 18px;
    }
    
    .card-badge {
        top: 12px;
        right: 12px;
        padding: 4px 10px;
        font-size: 10px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .box {
        height: 250px;
    }
    
    .overlay {
        padding: 15px;
    }
    
    .overlay h3 {
        font-size: 20px;
        margin-bottom: 8px;
    }
    
    .overlay p {
        font-size: 13px;
    }
    
    .coming-soon {
        padding: 8px 15px;
        margin-top: 10px;
    }
    
    .coming-soon span {
        font-size: 16px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .box {
        transform: none !important;
    }
    
    .box:hover {
        transform: none !important;
    }
    
    .overlay {
        opacity: 0.9;
    }
    
    .box:hover .overlay {
        opacity: 1;
    }
    
    .box:hover img {
        transform: scale(1.03);
    }
    
    .overlay h3,
    .overlay p,
    .coming-soon,
    .card-badge {
        transform: none !important;
        opacity: 1 !important;
    }
    
    .coming-soon {
        opacity: 1;
        transform: scale(1) !important;
    }
    
    .card-badge {
        opacity: 1;
        transform: translateX(0) !important;
    }
}
</style>