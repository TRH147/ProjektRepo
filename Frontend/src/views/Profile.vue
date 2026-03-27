<template>
  <div>
    <header class="header">
      <img :src="logoImg" alt="Logo" class="logo" />

      <div class="hamburger" @click="toggleMenu">
        <span></span>
        <span></span>
        <span></span>
      </div>

      <nav :class="['navbar', { active: isMenuOpen }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update">Újdonságok</router-link>
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum">Fórum</router-link>
        <button class="logout-btn" @click="logout">Kijelentkezés</button>
      </nav>
    </header>

    <section class="profile-container">
      <div class="profile-card">
        <img :src="profile.image" class="profile-pic" />

        <ul class="profile-info">
          <li><strong>Üdvözlünk,</strong> {{ profile.name }}</li>
          <li><strong>Email:</strong> {{ profile.email }}</li>
          <li><strong>Regisztráció:</strong> {{ profile.registered }}</li>
        </ul>
      </div>
    </section>

    <div class="tabs">
      <button
        class="tab"
        :class="{ active: activeContent === 'altalanos' }"
        @click="showTab('altalanos')"
      >
        Általános
      </button>
      <button
        class="tab"
        :class="{ active: activeContent === 'adatvaltoztatas' }"
        @click="showTab('adatvaltoztatas')"
      >
        Adatváltoztatás
      </button>
      <button
        class="tab"
        :class="{ active: activeContent === 'profilkep' }"
        @click="showTab('profilkep')"
      >
        Profilkép
      </button>
      <button
        class="tab"
        :class="{ active: activeContent === 'baratok' }"
        @click="showTab('baratok')"
      >
        Barátok
      </button>
    </div>

    <section
      id="altalanos"
      class="profile-edit-container"
      v-if="activeContent === 'altalanos'"
    >
      <div class="chart-container">
        <h3>Játékos statisztika</h3>
        <div class="chart-wrapper">
          <canvas ref="playerChart" :key="'chart-' + activeContent"></canvas>
        </div>
      </div>
    </section>

    <section
      id="adatvaltoztatas"
      class="profile-edit-container"
      v-show="activeContent === 'adatvaltoztatas'"
    >
      <div class="profile-edit-left">
        <h3>Név változtatás</h3>
        <form @submit.prevent="updateName">
          <label for="oldName">Régi név:</label>
          <input
            type="text"
            id="oldName"
            v-model="oldName"
            placeholder="Régi név"
          />

          <label for="newName">Új név:</label>
          <input
            type="text"
            id="newName"
            v-model="newName"
            placeholder="Új név"
          />

          <button type="submit">Mentés</button>
        </form>
        <h3>Jelszó változtatás</h3>
<form @submit.prevent="updatePass">
  <label for="oldPass">Régi jelszó:</label>
  <div class="password-field">
    <input
      :type="showOldPass ? 'text' : 'password'"
      id="oldPass"
      v-model="oldPass"
      placeholder="Régi jelszó"
    />
    <button
      type="button"
      class="show-pass-btn"
      @click="showOldPass = !showOldPass"
    >
      <i :class="showOldPass ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
    </button>
  </div>

  <label for="newPass">Új jelszó:</label>
  <div class="password-field">
    <input
      :type="showNewPass ? 'text' : 'password'"
      id="newPass"
      v-model="newPass"
      placeholder="Új jelszó"
    />
    <button
      type="button"
      class="show-pass-btn"
      @click="showNewPass = !showNewPass"
    >
      <i :class="showNewPass ? 'pi pi-eye-slash' : 'pi pi-eye'"></i>
    </button>
  </div>
  <button type="submit">Mentés</button>
</form>
      </div>
    </section>

    <section
      id="profilkep"
      class="profile-edit-container"
      v-show="activeContent === 'profilkep'"
    >
      <div class="profile-edit-right">
        <h3>Profilkép cseréje</h3>
        <form @submit.prevent="updateProfilePic">
          <input
            type="file"
            @change="onFileChange"
            accept="image/*"
            style="color: #fae9d7"
          />
          <button type="submit">Feltöltés</button>
        </form>
      </div>
    </section>

    <section
      id="baratok"
      class="profile-edit-container"
      v-show="activeContent === 'baratok'"
    >
      <div class="profile-edit-left">
        <h3>Barátaim</h3>

        <ul class="friends-list">
          <li
            v-for="friend in friends"
            :key="friend.username"
            class="friend-item"
          >
            <img :src="friend.profileImage" class="friend-pic" />
            <span>{{ friend.username }}</span>
            <button @click="removeFriend(friend.username)" class="remove-btn">
              Eltávolítás
            </button>
          </li>
        </ul>

        <div class="add-friend">
          <input
            type="text"
            v-model="newFriendUsername"
            placeholder="Adj meg egy felhasználónevet"
          />
          <button @click="sendFriendRequest">Küldés</button>
        </div>

        <div class="incoming-requests" v-if="pendingRequests.length">
          <h4>Beérkező barátfelkérések</h4>
          <ul>
            <li v-for="request in pendingRequests" :key="request.requestId">
              <img :src="request.senderProfileImage" class="friend-pic" />
              <span>{{ request.senderUsername }}</span>
              <button @click="acceptRequest(request.requestId)">Elfogad</button>
              <button @click="rejectRequest(request.requestId)">
                Elutasít
              </button>
            </li>
          </ul>
        </div>
      </div>
    </section>

    <FooterComponent />
  </div>
</template>

<script setup>
import FooterComponent from "../components/SiteFooter.vue";
import { ref, reactive, onMounted, watch, nextTick, onUnmounted } from "vue";
import axios from "axios";
import Chart from "chart.js/auto";
import ChartDataLabels from "chartjs-plugin-datalabels";
import logoImg from "/src/assets/Logo.png";
import { notificationService } from "../services/notificationService";

const activeContent = ref("altalanos");
let pollingInterval = null;

function showTab(tab) {
  activeContent.value = tab;

  if (pollingInterval) {
    clearInterval(pollingInterval);
    pollingInterval = null;
  }

  if (tab === "baratok") {
    fetchFriends();
    fetchPendingRequests();

    pollingInterval = setInterval(() => {
      fetchPendingRequests();
      fetchFriends();
    }, 5000);
  }
}

Chart.register(ChartDataLabels);

const playerChart = ref(null);
let chartInstance = null;
let isChartRendering = false;

const userStats = ref(null);
const kills = ref(0);
const deaths = ref(0);

async function fetchUserStats() {
  if (!userId.value) return;
  
  try {
    const response = await api.get(`/UserStats/${userId.value}`);
    userStats.value = response.data;
    kills.value = response.data.kills || 0;
    deaths.value = response.data.death || 0;
    
    if (activeContent.value === "altalanos") {
      renderChart();
    }
  } catch (error) {
    console.error("Error fetching user stats:", error);
    notificationService.error("Hiba", "Nem sikerült betölteni a statisztikákat.", 3000);
  }
}

function renderChart() {
  if (isChartRendering) return;

  if (!playerChart.value) {
    console.warn("Canvas element not found");
    return;
  }

  if (activeContent.value !== "altalanos") {
    return;
  }

  isChartRendering = true;

  requestAnimationFrame(() => {
    try {
      if (chartInstance) {
        chartInstance.destroy();
        chartInstance = null;
      }

      const total = kills.value + deaths.value;

      chartInstance = new Chart(playerChart.value, {
        type: "pie",
        data: {
          labels: ["Kill-ek", "Halálok"],
          datasets: [
            {
              data: [kills.value, deaths.value],
              backgroundColor: ["#bf2a4a", "#3660eb"],
              borderColor: "#000",
              borderWidth: 2,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          aspectRatio: 1.5,
          animation: {
            animateScale: true,
            animateRotate: true,
            duration: 1000,
          },
          plugins: {
            legend: {
              position: "bottom",
              labels: {
                color: "#fae9d7",
                padding: 20,
                font: {
                  size: 14,
                },
              },
            },
            datalabels: {
              color: "#fff",
              font: {
                weight: "bold",
                size: 14,
              },
              formatter: (value) => {
                if (total === 0) return "0%";
                return ((value / total) * 100).toFixed(1) + "%";
              },
            },
          },
        },
      });
    } catch (error) {
      console.error("Error rendering chart:", error);
    } finally {
      isChartRendering = false;
    }
  });
}

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.destroy();
    chartInstance = null;
  }
});

const api = axios.create({
  baseURL:
    "https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

const userId = ref(null);
const profile = reactive({
  name: "",
  image: "/src/assets/userrrr.png",
  email: "",
  registered: "",
  lastAction: "",
});

const isMenuOpen = ref(false);
function toggleMenu() {
  isMenuOpen.value = !isMenuOpen.value;
  if (isMenuOpen.value) {
    document.body.classList.add("nav-open");
  } else {
    document.body.classList.remove("nav-open");
  }
}

function logout() {
  localStorage.removeItem("token");
  window.location.href = "/";
}

const oldName = ref("");
const newName = ref("");

async function updateName() {
  if (!newName.value) {
    notificationService.error("Hiba", "Kérlek adj meg egy új nevet!", 3000);
    return;
  }

  try {
    const response = await api.put(`/Users/update-username`, {
      currentUsername: profile.name,
      newUsername: newName.value,
    });

    profile.name = newName.value;
    newName.value = "";
    oldName.value = "";

    if (response.data && response.data.message) {
      notificationService.success("Siker", response.data.message, 3000);
    } else {
      notificationService.success("Siker", "Felhasználónév frissítve!", 3000);
    }
  } catch (error) {
    console.error("Username update error:", error);
    notificationService.error(
      "Hiba",
      error.response?.data?.message || "Hiba történt a név frissítésekor!",
      5000
    );
  }
}

const oldPass = ref("");
const newPass = ref("");
const showOldPass = ref(false);
const showNewPass = ref(false);

async function updatePass() {
  if (!oldPass.value || !newPass.value) {
    notificationService.error("Hiba", "Kérlek töltsd ki minden mezőt!", 3000);
    return;
  }

  if (newPass.value.length < 6) {
    notificationService.error(
      "Hiba",
      "Az új jelszónak legalább 6 karakter hosszúnak kell lennie!",
      3000
    );
    return;
  }

  try {
    const response = await api.put(`/Users/change-password`, {
      currentPassword: oldPass.value,
      newPassword: newPass.value
    });

    oldPass.value = '';
    newPass.value = '';
    
    if (response.data && response.data.message) {
      notificationService.success("Siker", response.data.message, 3000);
    } else {
      notificationService.success("Siker", "Jelszó sikeresen frissítve!", 3000);
    }
    
  } catch (error) {
    console.error("Password update error:", error);

    if (error.response) {
      console.error("Error response:", error.response.data);
      notificationService.error(
        "Hiba",
        error.response.data?.message || "Hiba történt a jelszó frissítésekor!",
        5000
      );
    } else if (error.request) {
      console.error("No response received:", error.request);
      notificationService.error(
        "Hálózati hiba",
        "Nem érkezett válasz a szervertől. Ellenőrizd az internetkapcsolatot.",
        5000
      );
    } else {
      console.error("Error:", error.message);
      notificationService.error(
        "Hiba",
        "Ismeretlen hiba történt: " + error.message,
        5000
      );
    }
  }
}

const fileInput = ref(null);
let selectedFile = null;
function onFileChange(e) {
  selectedFile = e.target.files[0];
}

async function updateProfilePic() {
  if (!selectedFile) {
    notificationService.error("Hiba", "Válassz egy képet!", 3000);
    return;
  }

  const formData = new FormData();
  formData.append("file", selectedFile);

  try {
    const response = await api.post(`/Users/upload-profile-image`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    if (response.data && response.data.profileImageUrl) {
      const imagePath = response.data.profileImageUrl;
      profile.image = `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${imagePath}?t=${Date.now()}`;
      notificationService.success(
        "Siker",
        response.data.message || "Profilkép sikeresen frissítve!",
        3000
      );
    } else {
      notificationService.warning(
        "Figyelmeztetés",
        "Profilkép feltöltve, de a kép elérési útja nem érhető el.",
        3000
      );
    }

    selectedFile = null;
    if (fileInput.value) {
      fileInput.value.value = "";
    }
  } catch (error) {
    console.error("Upload error:", error);
    notificationService.error(
      "Hiba",
      error.response?.data?.message ||
        "Hiba történt a profilkép frissítésekor!",
      5000
    );
  }
}

const friends = ref([]);
const newFriendUsername = ref("");
const pendingRequests = ref([]);

async function fetchFriends() {
  try {
    const res = await api.get(`/FriendRequest/friends`);

    console.log("Friends API response:", res.data);

    if (res.data && res.data.friends && Array.isArray(res.data.friends)) {
      friends.value = res.data.friends.map((f) => ({
        username: f.friendUsername,
        profileImage: f.friendProfileImage
          ? `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${f.friendProfileImage}?t=${Date.now()}`
          : "/src/assets/userrrr.png",
      }));

      console.log("Mapped friends with images:", friends.value);
    } else {
      console.warn("Friends array not found:", res.data);
      friends.value = [];
    }
  } catch (err) {
    console.error("Hiba a barátok lekérésekor:", err);
    friends.value = [];
    notificationService.error(
      "Hiba",
      "Nem sikerült betölteni a barátlistát.",
      3000
    );
  }
}

async function fetchPendingRequests() {
  if (!userId.value) return;

  try {
    const res = await api.get(`/FriendRequest/pending`);

    console.log("Pending API response:", res.data);

    if (!res.data || !res.data.pendingRequests) {
      console.warn("No pending requests data:", res.data);
      pendingRequests.value = [];
      return;
    }

    pendingRequests.value = res.data.pendingRequests.map((r) => ({
      requestId: r.id,
      senderUsername: r.senderUsername,
      senderProfileImage: r.senderProfileImage
        ? `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${r.senderProfileImage}`
        : "/src/assets/userrrr.png",
      createdAt: r.createdAt,
    }));

    console.log("Mapped pending requests:", pendingRequests.value);
  } catch (err) {
    console.error("Hiba a barátfelkérések lekérésekor:", err);
    notificationService.error(
      "Hiba",
      "Nem sikerült betölteni a barátfelkéréseket.",
      3000
    );
  }
}

async function sendFriendRequest() {
  if (!newFriendUsername.value) {
    notificationService.error("Hiba", "Adj meg egy felhasználónevet!", 3000);
    return;
  }

  try {
    const userResponse = await api.get(
      `/Users/by-username/${newFriendUsername.value}`,
    );

    if (!userResponse.data || !userResponse.data.id) {
      notificationService.error("Hiba", "Felhasználó nem található!", 3000);
      return;
    }

    const receiverId = userResponse.data.id;

    await api.post(`/FriendRequest/send/${receiverId}`, {});

    notificationService.success(
      "Siker",
      `Barátfelkérés elküldve ${newFriendUsername.value}-nak!`,
      3000
    );
    newFriendUsername.value = "";
  } catch (err) {
    console.error(err);
    if (err.response?.status === 404) {
      notificationService.error("Hiba", "Felhasználó nem található!", 3000);
    } else {
      notificationService.error(
        "Hiba",
        err.response?.data?.message || "Hiba a barátfelkérés küldésekor!",
        5000
      );
    }
  }
}

async function acceptRequest(requestId) {
  try {
    await api.post(`/FriendRequest/accept/${requestId}`);

    pendingRequests.value = pendingRequests.value.filter(
      (r) => r.requestId !== requestId,
    );

    await fetchFriends();

    notificationService.success("Siker", "Barátfelkérés elfogadva!", 3000);
  } catch (err) {
    notificationService.error(
      "Hiba",
      err.response?.data?.message || "Hiba az elfogadáskor",
      5000
    );
  }
}

async function rejectRequest(requestId) {
  try {
    await api.post(`/FriendRequest/reject/${requestId}`);

    pendingRequests.value = pendingRequests.value.filter(
      (r) => r.requestId !== requestId,
    );
    notificationService.success("Siker", "Barátfelkérés elutasítva!", 3000);
  } catch (err) {
    notificationService.error(
      "Hiba",
      err.response?.data?.message || "Hiba az elutasításkor",
      5000
    );
  }
}

async function removeFriend(friendUsername) {
  if (!friendUsername) return;

  try {
    const userResponse = await api.get(`/Users/by-username/${friendUsername}`);

    if (!userResponse.data || !userResponse.data.id) {
      notificationService.error("Hiba", "Felhasználó nem található!", 3000);
      return;
    }

    const friendId = userResponse.data.id;

    const res = await api.delete(`/FriendRequest/remove/${friendId}`);

    if (res.data.success) {
      notificationService.success("Siker", res.data.message, 3000);
      friends.value = friends.value.filter(
        (f) => f.username !== friendUsername,
      );
    } else {
      notificationService.warning("Figyelmeztetés", res.data.message, 3000);
    }
  } catch (err) {
    console.error(err);

    if (err.response?.status === 404) {
      notificationService.error("Hiba", "Felhasználó nem található!", 3000);
    } else {
      notificationService.error(
        "Hiba",
        err.response?.data?.message || "Hiba a barát törlésekor!",
        5000
      );
    }
  }
}

async function deleteAccount() {
  const confirmed = confirm(
    "Biztosan törölni szeretnéd a fiókod? Ez a művelet nem visszavonható!"
  );
  
  if (!confirmed) return;

  try {
    const response = await api.delete("/Users/delete-account");
    if (response.data.success) {
      localStorage.removeItem("token");
      notificationService.success("Siker", "Fiók sikeresen törölve!", 3000);
      setTimeout(() => {
        window.location.href = "/";
      }, 1000);
    }
  } catch (error) {
    console.error("Account deletion error:", error);
    notificationService.error(
      "Hiba",
      error.response?.data?.message || "Hiba történt a fiók törlésekor!",
      5000
    );
  }
}

onMounted(async () => {
  console.log("Profile component mounted");

  try {
    const response = await api.get("/Users/profile");

    userId.value = response.data.id;
    profile.name = response.data.username;
    profile.email = response.data.email;
    profile.image = response.data.profileImages
      ? `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${response.data.profileImages}`
      : "/src/assets/userrrr.png";

    if (response.data.createdAt) {
      const date = new Date(response.data.createdAt);
      profile.registered = date.toLocaleDateString("hu-HU", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    }

    profile.lastAction = response.data.lastAction || "";

    // Fetch user stats after we have the userId
    await fetchUserStats();

    await nextTick();

    setTimeout(() => {
      if (activeContent.value === "altalanos") {
        renderChart();
      }
    }, 100);
  } catch (error) {
    console.error("Nem sikerült betölteni a felhasználót:", error);
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    } else {
      notificationService.error(
        "Hiba",
        "Nem sikerült betölteni a profil adatokat.",
        3000
      );
    }
  }
});

let renderTimeout = null;
watch(activeContent, async (newVal) => {
  console.log("Tab changed to:", newVal);

  if (renderTimeout) {
    clearTimeout(renderTimeout);
  }

  if (newVal === "altalanos") {
    renderTimeout = setTimeout(() => {
      nextTick().then(() => {
        renderChart();
      });
    }, 50);
  } else if (newVal === "baratok") {
    fetchFriends();
    fetchPendingRequests();
  } else {
    if (chartInstance) {
      chartInstance.destroy();
      chartInstance = null;
    }
  }
});

// Refresh stats every 30 seconds when on the stats tab
let statsPollingInterval = null;
watch(activeContent, (newVal) => {
  if (statsPollingInterval) {
    clearInterval(statsPollingInterval);
    statsPollingInterval = null;
  }
  
  if (newVal === "altalanos" && userId.value) {
    statsPollingInterval = setInterval(() => {
      fetchUserStats();
    }, 30000);
  }
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);
</script>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Belleza&display=swap");
@import "primeicons/primeicons.css";

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: "Belleza", sans-serif;
}

body {
  background: #1c1c1c;
  min-height: 100vh;
}

.header {
  position: absolute;
  width: 100%;
  padding: 0px 3%;
  background: transparent;
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 1000;
}

.logo {
  width: clamp(50px, 8vw, 80px);
  height: auto;
  user-select: none;
  transition: transform 0.3s ease;
}

.logo:hover {
  transform: scale(1.05);
}

.navbar {
  display: flex;
  align-items: center;
  gap: clamp(15px, 2vw, 25px);
}

.navbar a {
  position: relative;
  font-size: clamp(14px, 1.2vw, 16px);
  color: #fae9d7;
  text-decoration: none;
  font-weight: 600;
  text-transform: uppercase;
  text-shadow: 2px 2px 5px rgba(0, 0, 0, 0.7);
  transition: color 0.3s ease;
  white-space: nowrap;
}

.navbar a:hover,
.navbar a.active {
  color: #e24c4f;
}

.navbar a::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -6px;
  width: 100%;
  height: 2px;
  background: #e24c4f;
  border-radius: 5px;
  transform-origin: right;
  transform: scaleX(0);
  transition: transform 0.3s ease;
}

.navbar a:hover::after,
.navbar a.active::after {
  transform-origin: left;
  transform: scaleX(1);
}

.logout-btn {
  min-width: clamp(100px, 10vw, 140px);
  height: clamp(40px, 4vw, 45px);
  background: transparent;
  border: 2px solid #fae9d7;
  outline: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: clamp(13px, 1.1vw, 15px);
  color: #fae9d7;
  font-weight: 600;
  text-transform: uppercase;
  text-shadow: 2px 2px 5px rgba(0, 0, 0, 0.7);
  transition: all 0.3s ease;
  padding: 0 clamp(10px, 1vw, 15px);
  white-space: nowrap;
  margin-left: clamp(5px, 1vw, 10px);
}

.logout-btn:hover {
  background: #d55053;
  border-color: #d55053;
  color: #1c1c1c;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(213, 80, 83, 0.3);
}

.hamburger {
  display: none;
  flex-direction: column;
  justify-content: space-between;
  width: 30px;
  height: 21px;
  cursor: pointer;
  z-index: 1001;
  padding: 0;
  background: transparent;
  border: none;
  transition: transform 0.3s ease;
}

.hamburger:hover {
  transform: scale(1.1);
}

.hamburger span {
  height: 3px;
  width: 100%;
  background: #fae9d7;
  border-radius: 2px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  transform-origin: center;
}

.hamburger.active span:nth-child(1) {
  transform: translateY(9px) rotate(45deg);
}

.hamburger.active span:nth-child(2) {
  opacity: 0;
  transform: scaleX(0);
}

.hamburger.active span:nth-child(3) {
  transform: translateY(-9px) rotate(-45deg);
}

.main-content {
  padding-top: 80px;
  min-height: calc(100vh - 80px);
}

.profile-container {
  padding: 40px 5% 20px;
  width: 100%;
  display: flex;
  justify-content: center;
  animation: fadeIn 0.5s ease;
}

.profile-card {
  width: 100%;
  margin-top: 60px;
  max-width: 850px;
  background: rgba(255, 255, 255, 0.07);
  border-radius: 15px;
  padding: clamp(20px, 3vw, 30px);
  display: flex;
  align-items: center;
  gap: clamp(20px, 3vw, 30px);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(250, 233, 215, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
  transition:
    transform 0.3s ease,
    box-shadow 0.3s ease;
}

.profile-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.3);
}

.profile-pic {
  width: clamp(100px, 15vw, 140px);
  height: clamp(100px, 15vw, 140px);
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid #fae9d7;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
}

.profile-info {
  list-style: none;
  font-size: clamp(15px, 1.5vw, 18px);
  color: #fae9d7;
  line-height: 1.8;
  flex: 1;
}

.profile-info li {
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.profile-info strong {
  color: #e24c4f;
  font-weight: 700;
}

.profile-info .delete {
  color: #e24c4f;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 5px 10px;
  border-radius: 5px;
  margin-top: 10px;
  display: inline-block;
}

.profile-info .delete:hover {
  color: #ff0000;
  background: rgba(255, 0, 0, 0.1);
  transform: translateX(5px);
}

.tabs {
  margin: 30px 5% 20px;
  display: flex;
  border-bottom: 3px solid #d55053;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
  scrollbar-width: none;
  padding-bottom: 5px;
}

.tabs::-webkit-scrollbar {
  display: none;
}

.tab {
  margin: 0 clamp(5px, 1vw, 10px);
  padding: clamp(8px, 1.5vw, 12px) clamp(15px, 2vw, 25px);
  min-height: 50px;
  min-width: clamp(100px, 15vw, 150px);
  cursor: pointer;
  border: none;
  background: rgba(0, 0, 0, 0.3);
  font-size: clamp(14px, 1.2vw, 16px);
  transition: all 0.3s ease;
  color: #fae9d7;
  border-radius: 8px 8px 0 0;
  white-space: nowrap;
  flex-shrink: 0;
}

.tab:hover {
  background: rgba(213, 80, 83, 0.1);
  color: #d55053;
}

.tab.active {
  background: rgba(213, 80, 83, 0.2);
  color: #d55053;
  font-weight: bold;
  border-top: 3px solid #d55053;
  border-left: 1px solid rgba(213, 80, 83, 0.3);
  border-right: 1px solid rgba(213, 80, 83, 0.3);
}

.profile-edit-container {
  width: 100%;
  max-width: 850px;
  margin: 30px auto;
  padding: 0 5%;
  animation: slideUp 0.5s ease;
}

.chart-container {
  width: 100%;
  height: auto;
  aspect-ratio: 1.5;
  max-width: 500px;
  background: #2c2c2c;
  padding: clamp(15px, 2vw, 20px);
  border-radius: 15px;
  text-align: center;
  margin: auto;
  border: 1px solid rgba(250, 233, 215, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
}

.chart-container h3 {
  color: #fae9d7;
  margin-bottom: 15px;
  font-size: clamp(18px, 2vw, 22px);
  font-weight: 600;
}

.chart-container canvas {
  width: 100% !important;
  height: 100% !important;
  display: block;
}

.profile-edit-left,
.profile-edit-right {
  width: 100%;
  padding: clamp(20px, 3vw, 25px);
  border-radius: 15px;
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(250, 233, 215, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
}

.profile-edit-left h3,
.profile-edit-right h3 {
  color: #fae9d7;
  margin-bottom: clamp(15px, 2vw, 20px);
  border-bottom: 2px solid #d55053;
  padding-bottom: clamp(8px, 1vw, 12px);
  font-size: clamp(18px, 2vw, 22px);
}

.profile-edit-left label,
.profile-edit-right label {
  color: #fae9d7;
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  font-size: clamp(14px, 1.2vw, 16px);
}

.profile-edit-left form,
.profile-edit-right form {
  display: flex;
  flex-direction: column;
  gap: clamp(12px, 1.5vw, 18px);
  margin-bottom: 30px;
}

.profile-edit-left input[type="text"],
.profile-edit-left input[type="password"],
.profile-edit-left input[type="email"],
.profile-edit-right input[type="file"] {
  width: 100%;
  padding: clamp(10px, 1.2vw, 14px);
  background: rgba(17, 17, 17, 0.8);
  color: #fae9d7;
  border: 1px solid rgba(68, 68, 68, 0.5);
  border-radius: 8px;
  font-size: clamp(14px, 1.2vw, 16px);
  transition: all 0.3s ease;
  outline: none;
}

.profile-edit-left input[type="text"]:focus,
.profile-edit-left input[type="password"]:focus,
.profile-edit-left input[type="email"]:focus,
.profile-edit-right input[type="file"]:focus {
  border-color: #d55053;
  box-shadow: 0 0 0 2px rgba(213, 80, 83, 0.2);
}

.password-field {
  position: relative;
  width: 100%;
}

.show-pass-btn {
  position: absolute;
  right: 12px;
  top: 42%;
  transform: translateY(-50%);
  background: transparent !important;
  border: none;
  color: #fae9d7;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  height: 30px;
  opacity: 0.7;
  transition: opacity 0.3s ease;
}

.show-pass-btn:hover {
  opacity: 1;
  box-shadow: none !important;
}

.show-pass-btn i {
  font-size: 18px;
}

.password-field input[type="text"],
.password-field input[type="password"] {
  padding-right: 45px;
}

.profile-edit-right input[type="file"]::file-selector-button {
  background: rgba(213, 80, 83, 0.2);
  color: #fae9d7;
  border: 1px solid rgba(213, 80, 83, 0.5);
  border-radius: 6px;
  padding: 8px 12px;
  cursor: pointer;
  font-weight: 600;
  margin-right: 10px;
  transition: all 0.3s ease;
  font-size: clamp(13px, 1.1vw, 15px);
}

.profile-edit-right input[type="file"]::file-selector-button:hover {
  background: rgba(213, 80, 83, 0.3);
  border-color: #d55053;
}

.profile-edit-left button,
.profile-edit-right button {
  padding: clamp(10px, 1.2vw, 14px) clamp(15px, 2vw, 25px);
  background: #d55053;
  border: none;
  border-radius: 8px;
  color: #fae9d7;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: clamp(10px, 1.2vw, 14px);
  margin-top: 5px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.profile-edit-left button:hover,
.profile-edit-right button:hover {
  background: #a52d31;
  box-shadow: 0 4px 12px rgba(213, 80, 83, 0.3);
}

.friends-list {
  list-style: none;
  padding: 0;
  margin: 0 0 25px 0;
}

.friend-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: clamp(12px, 1.5vw, 15px);
  margin-bottom: 10px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  border: 1px solid rgba(250, 233, 215, 0.1);
  transition: all 0.3s ease;
}

.friend-item:hover {
  background: rgba(255, 255, 255, 0.08);
  transform: translateX(5px);
}

.friend-pic {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 12px;
  border: 2px solid #d55053;
  flex-shrink: 0;
}

.friend-item span {
  color: #fae9d7;
  font-size: clamp(14px, 1.2vw, 16px);
  flex: 1;
}

.remove-btn {
  background: rgba(226, 76, 79, 0.2);
  border: 1px solid rgba(226, 76, 79, 0.5);
  padding: 6px 12px;
  border-radius: 6px;
  color: #fae9d7;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: clamp(13px, 1.1vw, 14px);
  white-space: nowrap;
}

.remove-btn:hover {
  background: rgba(226, 76, 79, 0.3);
  border-color: #e24c4f;
}

.add-friend {
  display: flex;
  gap: 10px;
  margin-bottom: 25px;
  flex-wrap: wrap;
}

.add-friend input {
  flex: 1;
  min-width: 200px;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid rgba(68, 68, 68, 0.5);
  background: rgba(17, 17, 17, 0.8);
  color: #fae9d7;
  font-size: clamp(14px, 1.2vw, 16px);
  outline: none;
  transition: all 0.3s ease;
}

.add-friend input:focus {
  border-color: #d55053;
  box-shadow: 0 0 0 2px rgba(213, 80, 83, 0.2);
}

.add-friend button {
  padding: 12px 20px;
  background: #d55053;
  border: none;
  border-radius: 8px;
  color: #fae9d7;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: clamp(14px, 1.2vw, 16px);
  white-space: nowrap;
}

.add-friend button:hover {
  background: #a52d31;
  transform: translateY(-2px);
}

.incoming-requests {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid rgba(250, 233, 215, 0.1);
}

.incoming-requests h4 {
  color: #fae9d7;
  margin-bottom: 15px;
  font-size: clamp(16px, 1.5vw, 18px);
  font-weight: 600;
}

.incoming-requests ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.incoming-requests li {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px;
  margin-bottom: 10px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  border: 1px solid rgba(250, 233, 215, 0.1);
  flex-wrap: wrap;
  gap: 10px;
}

.incoming-requests li span {
  color: #fae9d7;
  font-size: clamp(14px, 1.2vw, 16px);
  flex: 1;
  min-width: 150px;
}

.incoming-requests button {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  background: #d55053;
  color: #fae9d7;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  font-size: clamp(13px, 1.1vw, 14px);
  white-space: nowrap;
}

.incoming-requests button:hover {
  background: #a52d31;
  transform: translateY(-2px);
}

.incoming-requests button:first-of-type {
  background: rgba(76, 175, 80, 0.2);
  border: 1px solid rgba(76, 175, 80, 0.5);
  color: #4caf50;
}

.incoming-requests button:first-of-type:hover {
  background: rgba(76, 175, 80, 0.3);
  border-color: #4caf50;
}

.incoming-requests button:last-of-type {
  background: rgba(244, 67, 54, 0.2);
  border: 1px solid rgba(244, 67, 54, 0.5);
  color: #f44336;
}

.incoming-requests button:last-of-type:hover {
  background: rgba(244, 67, 54, 0.3);
  border-color: #f44336;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
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

@media only screen and (max-width: 1024px) {
  .header {
    padding: 12px 4%;
  }

  .navbar {
    gap: 12px;
  }

  .profile-container {
    padding: 30px 4% 15px;
  }

  .tabs {
    margin: 25px 4% 15px;
  }

  .profile-edit-container {
    padding: 0 4%;
  }
}

@media only screen and (max-width: 768px) {
  .header {
    padding: 10px 3%;
  }

  .logo {
    width: 50px;
  }

  .hamburger {
    display: flex;
  }

  .navbar {
    position: fixed;
    top: 0;
    right: -100%;
    width: 100%;
    height: 100vh;
    flex-direction: column;
    justify-content: flex-start;
    padding-top: 100px;
    backdrop-filter: blur(20px);
    text-align: center;
    gap: 25px;
    transition: right 0.4s cubic-bezier(0.4, 0, 0.2, 1);
    z-index: 999;
    overflow-y: auto;
  }

  .navbar.active {
    right: 0;
  }

  .navbar a {
    font-size: 18px;
    padding: 15px;
    width: 80%;
    max-width: 250px;
  }

  .logout-btn {
    width: 80%;
    max-width: 250px;
    margin-left: 0;
    margin-top: 20px;
    height: 50px;
    font-size: 16px;
  }

  .profile-card {
    flex-direction: column;
    text-align: center;
    gap: 20px;
  }

  .profile-info li {
    justify-content: center;
    text-align: center;
  }

  .tabs {
    margin: 20px 3% 15px;
  }

  .tab {
    min-width: 120px;
    font-size: 14px;
    padding: 10px 15px;
  }

  .friend-item {
    flex-direction: column;
    text-align: center;
    gap: 10px;
  }

  .friend-item span {
    text-align: center;
  }

  .add-friend {
    flex-direction: column;
  }

  .add-friend input {
    min-width: 100%;
  }

  .incoming-requests li {
    flex-direction: column;
    text-align: center;
    gap: 10px;
  }

  .incoming-requests li span {
    min-width: 100%;
    text-align: center;
  }

  body.nav-open {
    overflow: hidden;
  }
}

@media only screen and (max-width: 480px) {
  .header {
    padding: 8px 15px;
  }

  .logo {
    width: 45px;
  }

  .hamburger {
    width: 26px;
    height: 18px;
  }

  .navbar {
    padding-top: 80px;
    gap: 20px;
  }

  .navbar a {
    font-size: 16px;
    padding: 12px;
    width: 90%;
  }

  .logout-btn {
    width: 90%;
    height: 45px;
    font-size: 15px;
    margin-top: 15px;
  }

  .profile-container {
    padding: 25px 15px 10px;
  }

  .profile-card {
    padding: 20px;
  }

  .profile-pic {
    width: 100px;
    height: 100px;
  }

  .profile-info {
    font-size: 15px;
  }

  .tabs {
    margin: 15px 15px 10px;
  }

  .tab {
    min-width: 100px;
    font-size: 13px;
    padding: 8px 12px;
  }

  .profile-edit-container {
    padding: 0 15px;
  }

  .chart-container {
    padding: 15px;
  }

  .profile-edit-left,
  .profile-edit-right {
    padding: 15px;
  }

  .friend-pic {
    width: 35px;
    height: 35px;
  }
}

@media only screen and (max-width: 320px) {
  .header {
    padding: 5px 10px;
  }

  .logo {
    width: 40px;
  }

  .navbar a {
    font-size: 15px;
    padding: 10px;
  }

  .logout-btn {
    height: 40px;
    font-size: 14px;
  }

  .tab {
    min-width: 80px;
    font-size: 12px;
    padding: 6px 10px;
  }

  .profile-pic {
    width: 80px;
    height: 80px;
  }

  .profile-info {
    font-size: 14px;
  }
}

@media (hover: none) and (pointer: coarse) {
  .navbar a,
  .logout-btn,
  .tab,
  button {
    min-height: 44px;
  }

  .profile-edit-left input[type="text"],
  .profile-edit-left input[type="password"],
  .add-friend input {
    font-size: 16px;
    min-height: 44px;
  }

  .hamburger {
    min-width: 44px;
    min-height: 44px;
  }
}
</style>