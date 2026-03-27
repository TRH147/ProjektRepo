import { createApp } from "vue";
import { createPinia } from "pinia";
import router from "./router";
import App from "./App.vue";
import "./assets/style.css";


const pinia = createPinia();
const app = createApp(App);

app.use(pinia);
app.use(router);

app.mount("#app");

import { useUserStore } from "./stores/user";
const userStore = useUserStore();

setTimeout(() => {
  userStore.initialize();
}, 0);