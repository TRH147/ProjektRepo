import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import UpdateView from '../views/UpdateView.vue'
import StaticsView from '../views/StaticsView.vue'
import ForumView from '../views/ForumView.vue'

const routes = [
  { path: '/', component: HomeView },
  { path: '/update', component: UpdateView },
  { path: '/statics', component: StaticsView },
  { path: '/forum', component: ForumView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  },
})

export default router
