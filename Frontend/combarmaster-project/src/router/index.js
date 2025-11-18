import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import UpdateView from '../views/UpdateView.vue'
import StaticsView from '../views/StaticsView.vue'
import ForumView from '../views/ForumView.vue'
import UjJelszo from '../views/UjJelszo.vue'
import Admin from '../views/Admin.vue'
import Profile from '../views/Profile.vue'
import HelyreJelszo from '../views/HelyreJelszo.vue'

const routes = [
  { path: '/', component: HomeView },
  { path: '/update', component: UpdateView },
  { path: '/statics', component: StaticsView },
  { path: '/forum', component: ForumView },
  { path: '/forgot-password', component: UjJelszo },
  { path: '/admin', component: Admin },
  { path: '/profile', component: Profile },
  { path: '/helyre-jelszo', component: HelyreJelszo },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  },
})

export default router
