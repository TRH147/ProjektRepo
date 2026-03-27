<template>
    <section class="footer">
        <div class="footer-content">
            <h3>Kövess minket közösségi médián</h3>
            <div class="social">
                <a href="#" aria-label="Facebook" class="social-link">
                    <i class='bx bxl-facebook-circle'></i>
                    <span class="social-tooltip">Facebook</span>
                </a>
                <a href="#" aria-label="Instagram" class="social-link">
                    <i class='bx bxl-instagram-alt'></i>
                    <span class="social-tooltip">Instagram</span>
                </a>
                <a href="#" aria-label="Twitter" class="social-link">
                    <i class='bx bxl-twitter'></i>
                    <span class="social-tooltip">Twitter</span>
                </a>
            </div>

            <div class="footer-nav">
                <ul class="list">
                    <li><router-link to="/" @click.native="scrollToTop" class="nav-link">Főoldal</router-link></li>
                    <li><router-link to="/update" class="nav-link">Újdonságok</router-link></li>
                    <li><router-link to="/statics" class="nav-link">Statisztika</router-link></li>
                    <li><router-link to="/forum" class="nav-link">Fórum</router-link></li>
                </ul>
            </div>

            <div class="about-section">
                <h4>Rólunk</h4>
                <p class="copyright">
                    Csapatunk 3 junior fejlesztőből áll össze, kiknek célja egy olyan játék fejlesztése, 
                    amely lépést tud tartani a mai modern videójátékipar termékeivel.
                </p>
            </div>

            <div class="footer-bottom">
                <p class="copyright-year">
                    &copy; {{ currentYear }} CombatMaster. Minden jog fenntartva.
                </p>
            </div>
        </div>
    </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const currentYear = ref(new Date().getFullYear())

function scrollToTop() {
    window.scrollTo({ 
        top: 0, 
        behavior: 'smooth' 
    })
}

onMounted(() => {
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Belleza&display=swap');
@import url('https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css');

.footer {
    padding: clamp(40px, 6vw, 40px) 0 20px;
    background: linear-gradient(135deg, #fae9d7 0%, #f5e1c9 100%);
    position: relative;
    overflow: hidden;
}

.footer::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(to right, transparent, #d55053, transparent);
}

.footer-content {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 clamp(20px, 5vw, 40px);
    position: relative;
    z-index: 1;
}

.footer h3 {
    text-align: center;
    color: #1c1c1c;
    margin-bottom: clamp(25px, 4vw, 25px);
    font-size: clamp(24px, 3vw, 25px);
    text-transform: uppercase;
    letter-spacing: 1.5px;
    font-weight: 700;
    text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.1);
}

.social {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: clamp(20px, 4vw, 30px);
    margin-bottom: clamp(30px, 5vw, 25px);
    flex-wrap: wrap;
}

.social-link {
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    width: clamp(45px, 6vw, 30px);
    height: clamp(45px, 6vw, 30px);
    background: #1c1c1c;
    border-radius: 50%;
    color: #fae9d7;
    font-size: clamp(25px, 3vw, 22px);
    text-decoration: none;
    transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    overflow: hidden;
    box-shadow: 
        0 8px 20px rgba(0, 0, 0, 0.2),
        inset 0 0 0 2px transparent;
}

.social-link::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(135deg, #d55053, #e24c4f, #a52d31);
    border-radius: 50%;
    opacity: 0;
    transition: opacity 0.3s ease;
}

.social-link i {
    position: relative;
    z-index: 2;
    transition: transform 0.5s ease;
}

.social-link:hover {
    transform: translateY(-8px) scale(1.1);
    box-shadow: 
        0 15px 30px rgba(213, 80, 83, 0.3),
        0 8px 25px rgba(0, 0, 0, 0.3),
        inset 0 0 0 2px #d55053;
}

.social-link:hover::before {
    opacity: 1;
}

.social-link:hover i {
    transform: rotate(360deg);
    color: #fae9d7;
}

.social-tooltip {
    position: absolute;
    bottom: -40px;
    left: 50%;
    transform: translateX(-50%);
    background: #1c1c1c;
    color: #fae9d7;
    padding: 6px 12px;
    border-radius: 6px;
    font-size: 12px;
    font-weight: 600;
    white-space: nowrap;
    opacity: 0;
    visibility: hidden;
    transition: all 0.3s ease;
    z-index: 10;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.social-tooltip::before {
    content: '';
    position: absolute;
    top: -5px;
    left: 50%;
    transform: translateX(-50%);
    border-left: 5px solid transparent;
    border-right: 5px solid transparent;
    border-bottom: 5px solid #1c1c1c;
}

.social-link:hover .social-tooltip {
    opacity: 1;
    visibility: visible;
    bottom: -35px;
}

.footer-nav {
    margin-bottom: clamp(30px, 5vw, 25px);
    padding: 0 20px;
}

.list {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-wrap: wrap;
    gap: clamp(15px, 3vw, 25px);
    list-style: none;
    padding: 0;
    margin: 0;
}

.list li {
    position: relative;
}

.nav-link {
    color: #1c1c1c;
    text-decoration: none;
    font-size: clamp(16px, 1.8vw, 18px);
    font-weight: 600;
    padding: 8px 16px;
    border-radius: 25px;
    transition: all 0.3s ease;
    position: relative;
    overflow: hidden;
}

.nav-link::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(135deg, #d55053, #e24c4f);
    border-radius: 25px;
    opacity: 0;
    transition: opacity 0.3s ease;
    z-index: -1;
}

.nav-link:hover {
    color: #fae9d7;
}

.nav-link:hover::before {
    opacity: 1;
}

.nav-link::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 0;
    height: 2px;
    background: #d55053;
    transition: width 0.3s ease;
}

.nav-link:hover::after {
    width: calc(100% - 32px);
}

.about-section {
    max-width: 800px;
    margin: 0 auto clamp(20px, 5vw, 15px);
    padding: 0 20px;
    text-align: center;
}

.about-section h4 {
    color: #1c1c1c;
    font-size: clamp(20px, 2.5vw, 20px);
    margin-bottom: 15px;
    text-transform: uppercase;
    letter-spacing: 1px;
    position: relative;
    display: inline-block;
}

.about-section h4::after {
    content: '';
    position: absolute;
    bottom: -5px;
    left: 50%;
    transform: translateX(-50%);
    width: 60px;
    height: 3px;
    background: #d55053;
    border-radius: 2px;
}

.copyright {
    color: #1c1c1c;
    font-size: clamp(15px, 1.6vw, 17px);
    line-height: 1.7;
    opacity: 0.9;
}

.footer-bottom {
    padding-top: 20px;
    border-top: 1px solid rgba(28, 28, 28, 0.1);
    text-align: center;
}

.copyright-year {
    color: #1c1c1c;
    font-size: clamp(14px, 1.5vw, 16px);
    opacity: 0.8;
    margin: 0;
}

@media (max-width: 1024px) {
    .footer-content {
        padding: 0 30px;
    }
    
    .social {
        gap: 25px;
    }
    
    .list {
        gap: 20px;
    }
}

@media (max-width: 768px) {
    .footer {
        padding: 50px 0 15px;
    }
    
    .footer-content {
        padding: 0 25px;
    }
    
    .footer h3 {
        font-size: 28px;
        margin-bottom: 30px;
    }
    
    .social {
        gap: 20px;
        margin-bottom: 35px;
    }
    
    .social-link {
        width: 45px;
        height: 45px;
        font-size: 25px;
    }
    
    .footer-nav {
        margin-bottom: 35px;
    }
    
    .list {
        gap: 15px;
    }
    
    .nav-link {
        font-size: 17px;
        padding: 6px 14px;
    }
    
    .about-section {
        margin-bottom: 35px;
    }
    
    .about-section h4 {
        font-size: 22px;
    }
    
    .copyright {
        font-size: 16px;
    }
}

@media (max-width: 600px) {
    .footer {
        padding: 40px 0 15px;
    }
    
    .footer-content {
        padding: 0 20px;
    }
    
    .footer h3 {
        font-size: 24px;
        margin-bottom: 25px;
    }
    
    .social {
        gap: 15px;
        margin-bottom: 30px;
    }
    
    .social-link {
        width: 40px;
        height: 40px;
        font-size: 23px;
    }
    
    .social-tooltip {
        display: none;
    }
    
    .footer-nav {
        margin-bottom: 30px;
    }
    
    .list {
        flex-direction: column;
        gap: 12px;
    }
    
    .nav-link {
        font-size: 16px;
        padding: 8px 20px;
        display: inline-block;
    }
    
    .about-section {
        margin-bottom: 30px;
    }
    
    .about-section h4 {
        font-size: 20px;
    }
    
    .copyright {
        font-size: 15px;
    }
    
    .copyright-year {
        font-size: 14px;
    }
}

@media (max-width: 480px) {
    .footer {
        padding: 35px 0 12px;
    }
    
    .footer-content {
        padding: 0 15px;
    }
    
    .footer h3 {
        font-size: 22px;
        margin-bottom: 20px;
    }
    
    .social {
        gap: 12px;
        margin-bottom: 25px;
    }
    
    .social-link {
        width: 40px;
        height: 40px;
        font-size: 22px;
    }
    
    .footer-nav {
        margin-bottom: 25px;
    }
    
    .nav-link {
        font-size: 15px;
        padding: 6px 18px;
    }
    
    .about-section {
        margin-bottom: 25px;
    }
    
    .about-section h4 {
        font-size: 18px;
    }
    
    .copyright {
        font-size: 14px;
    }
    
    .copyright-year {
        font-size: 13px;
    }
}

@media (max-height: 600px) and (orientation: landscape) {
    .footer {
        padding: 30px 0 15px;
    }
    
    .footer h3 {
        font-size: 20px;
        margin-bottom: 20px;
    }
    
    .social {
        margin-bottom: 25px;
    }
    
    .footer-nav {
        margin-bottom: 25px;
    }
    
    .about-section {
        margin-bottom: 25px;
    }
}

@media (hover: none) and (pointer: coarse) {
    .social-link:hover {
        transform: none;
    }
    
    .social-link:active {
        transform: scale(0.95);
    }
    
    .nav-link:hover {
        color: #1c1c1c;
    }
    
    .nav-link:active {
        color: #fae9d7;
    }
    
    .nav-link:active::before {
        opacity: 1;
    }
    
    .social-tooltip {
        display: none;
    }
}
</style>