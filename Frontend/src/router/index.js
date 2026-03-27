import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import UpdateView from '../views/UpdateView.vue'
import StaticsView from '../views/StaticsView.vue'
import ForumView from '../views/ForumView.vue' 
import UjJelszo from '../views/UjJelszo.vue'
import Admin from '../views/Admin.vue'
import Profile from '../views/Profile.vue'
import HelyreJelszo from '../views/HelyreJelszo.vue'
import ThreadView from '../views/ThreadView.vue'
import CreateThread from '../views/CreateThread.vue'

const routes = [
  { 
    path: '/', 
    name: 'Home',
    component: HomeView 
  },
  { 
    path: '/update', 
    name: 'Update',
    component: UpdateView 
  },
  { 
    path: '/statics', 
    name: 'Statics',
    component: StaticsView 
  },
  { 
    path: '/forum', 
    name: 'Forum',
    component: ForumView 
  },
  { 
    path: '/thread/:id', 
    name: 'ThreadView',
    component: ThreadView,
    props: true
  },
  { 
    path: '/new-thread', 
    name: 'CreateThread',
    component: CreateThread 
  },
  { 
    path: '/forgot-password', 
    name: 'ForgotPassword',
    component: UjJelszo 
  },
  { 
    path: '/admin', 
    name: 'Admin',
    component: Admin 
  },
  { 
    path: '/profile', 
    name: 'Profile',
    component: Profile 
  },
  { 
    path: '/helyre-jelszo', 
    name: 'HelyreJelszo',
    component: HelyreJelszo 
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  },
})

export default router