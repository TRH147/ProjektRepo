<template>
    <div class="container1">
        <img class="my-cover" src="/src/assets/banner.jpg" alt="banner" />
        <div class="overlay3">
            <h2>CombatMaster</h2>
            <p>deathmatch-re épülő FPS játék</p>
            <button class="download-btn" @click="handleDownload">
                <span class="btn-text">Letöltés</span>
                <span class="btn-icon">
                    <i class='bx bx-download'></i>
                </span>
                <span class="btn-glow"></span>
            </button>
        </div>
    </div>
</template>

<script setup>
import { useUserStore } from '../stores/user'
import { notificationService } from '../services/notificationService'

const userStore = useUserStore()

function handleDownload() {
  if (!userStore.user) {
    notificationService.warning(
      'Bejelentkezés szükséges',
      'A játék letöltéséhez először jelentkezz be!'
    )
    return
  }

  const downloadUrl = '/downloads/CombatMaster.exe'

  fetch(downloadUrl, { method: 'HEAD' })
    .then(res => {
      if (!res.ok) {
        notificationService.error(
          'Letöltési hiba',
          'A fájl nem elérhető.'
        )
        return
      }

      const link = document.createElement('a')
      link.href = downloadUrl
      link.download = 'CombatMasterSetup.exe'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      
      notificationService.success(
        'Letöltés indul',
        'A játék letöltése megkezdődött!'
      )
    })
    .catch(err => {
      console.error(err)
      notificationService.error(
        'Letöltési hiba',
        'Hiba történt a letöltés során.'
      )
    })
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');

.container1 {
    width: 100%;
    height: 100vh;
    min-height: 600px;
    max-height: 1200px;
    position: relative;
    overflow: hidden;
}

.my-cover {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
    filter: brightness(0.7);
    animation: zoomEffect 20s ease-in-out infinite alternate;
}

.overlay3 {
    position: absolute;
    inset: 0;
    background: linear-gradient(
        to bottom,
        rgba(0, 0, 0, 0.3) 0%,
        rgba(0, 0, 0, 0.5) 50%,
        rgba(0, 0, 0, 0.7) 100%
    );
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    color: #fae9d7;
    padding: 20px;
}

.overlay3 h2 {
    font-size: clamp(3rem, 8vw, 65px);
    font-weight: 800;
    text-transform: uppercase;
    letter-spacing: 3px;
    margin-bottom: 15px;
    text-shadow: 3px 3px 8px rgba(0, 0, 0, 0.7);
    animation: fadeInDown 1s ease-out;
}

.overlay3 p {
    font-size: clamp(1.2rem, 2.5vw, 24px);
    font-weight: 300;
    text-transform: uppercase;
    letter-spacing: 2px;
    margin-bottom: 40px;
    opacity: 0.9;
    animation: fadeInUp 1s ease-out 0.3s both;
}

.download-btn {
    position: relative;
    background: linear-gradient(135deg, #d55053 0%, #e24c4f 50%, #a52d31 100%);
    border: none;
    border-radius: 50px;
    padding: 18px 45px;
    color: #fae9d7;
    font-size: clamp(18px, 2vw, 22px);
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 1.5px;
    cursor: pointer;
    overflow: hidden;
    transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    box-shadow: 
        0 10px 30px rgba(213, 80, 83, 0.3),
        0 5px 15px rgba(0, 0, 0, 0.5),
        inset 0 1px 0 rgba(255, 255, 255, 0.2);
    animation: fadeInUp 1s ease-out 0.6s both;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 15px;
    min-width: 200px;
}

.download-btn:hover {
    transform: translateY(-5px) scale(1.05);
    box-shadow: 
        0 15px 40px rgba(213, 80, 83, 0.5),
        0 10px 25px rgba(0, 0, 0, 0.6),
        inset 0 1px 0 rgba(255, 255, 255, 0.3);
    background: linear-gradient(135deg, #e24c4f 0%, #d55053 50%, #c54245 100%);
}

.download-btn:active {
    transform: translateY(-2px) scale(1.02);
    box-shadow: 
        0 8px 25px rgba(213, 80, 83, 0.4),
        0 3px 10px rgba(0, 0, 0, 0.5),
        inset 0 1px 0 rgba(255, 255, 255, 0.2);
}

.download-btn::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(
        135deg,
        transparent 30%,
        rgba(255, 255, 255, 0.1) 50%,
        transparent 70%
    );
    transform: translateX(-100%);
    transition: transform 0.6s ease;
}

.download-btn:hover::before {
    transform: translateX(100%);
}

.btn-text {
    position: relative;
    z-index: 2;
    text-shadow: 1px 1px 3px rgba(0, 0, 0, 0.3);
}

.btn-icon {
    position: relative;
    z-index: 2;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 24px;
    animation: bounce 2s infinite;
}

.btn-glow {
    position: absolute;
    top: -10px;
    left: -10px;
    right: -10px;
    bottom: -10px;
    background: radial-gradient(
        circle at center,
        rgba(213, 80, 83, 0.3) 0%,
        transparent 70%
    );
    border-radius: 60px;
    opacity: 0;
    transition: opacity 0.3s ease;
    z-index: 1;
}

.download-btn:hover .btn-glow {
    opacity: 1;
}

@keyframes fadeInDown {
    from {
        opacity: 0;
        transform: translateY(-30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes zoomEffect {
    0% {
        transform: scale(1);
    }
    100% {
        transform: scale(1.1);
    }
}

@keyframes bounce {
    0%, 20%, 50%, 80%, 100% {
        transform: translateY(0);
    }
    40% {
        transform: translateY(-5px);
    }
    60% {
        transform: translateY(-3px);
    }
}

@media (max-width: 1024px) {
    .container1 {
        height: 80vh;
        min-height: 500px;
    }
    
    .download-btn {
        padding: 16px 40px;
        min-width: 180px;
    }
}

@media (max-width: 768px) {
    .container1 {
        height: 70vh;
        min-height: 400px;
    }
    
    .overlay3 h2 {
        font-size: clamp(2.5rem, 6vw, 50px);
    }
    
    .overlay3 p {
        font-size: clamp(1rem, 2vw, 20px);
        margin-bottom: 30px;
    }
    
    .download-btn {
        padding: 14px 35px;
        font-size: 18px;
        min-width: 160px;
        gap: 12px;
    }
    
    .btn-icon {
        font-size: 20px;
    }
}

@media (max-width: 600px) {
    .container1 {
        height: 60vh;
        min-height: 350px;
    }
    
    .overlay3 {
        padding: 15px;
    }
    
    .overlay3 h2 {
        margin-bottom: 10px;
    }
    
    .overlay3 p {
        margin-bottom: 25px;
    }
    
    .download-btn {
        padding: 12px 30px;
        font-size: 16px;
        min-width: 140px;
        gap: 10px;
        border-radius: 40px;
    }
    
    .btn-icon {
        font-size: 18px;
    }
}

@media (max-width: 480px) {
    .container1 {
        height: 50vh;
        min-height: 300px;
    }
    
    .overlay3 h2 {
        font-size: 2rem;
        letter-spacing: 2px;
    }
    
    .overlay3 p {
        font-size: 1rem;
        letter-spacing: 1px;
    }
    
    .download-btn {
        padding: 10px 25px;
        font-size: 15px;
        min-width: 130px;
        gap: 8px;
        letter-spacing: 1px;
    }
    
    .btn-icon {
        font-size: 16px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .container1 {
        height: 100vh;
    }
    
    .overlay3 {
        padding-top: 60px;
        justify-content: flex-start;
    }
    
    .overlay3 h2 {
        font-size: 2.5rem;
        margin-bottom: 10px;
    }
    
    .overlay3 p {
        font-size: 1.1rem;
        margin-bottom: 20px;
    }
    
    .download-btn {
        padding: 10px 25px;
        font-size: 16px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .download-btn {
        min-height: 60px;
        padding: 15px 35px;
    }
    
    .download-btn:active {
        transform: translateY(-2px) scale(0.98);
    }
}
</style>