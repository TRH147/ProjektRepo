<template>
  <div class="thread-view">
    <div class="container">
      <div v-if="loading" class="loading">
        <i class="fas fa-spinner fa-spin"></i> Téma betöltése...
      </div>

      <div v-else-if="thread" class="thread-container">
        <button class="back-button" @click="goBack">
          <i class="fas fa-arrow-left"></i> Visszatérés a Fórumra
        </button>

        <div class="thread-header">
          <h1 class="thread-title">{{ thread.title }}</h1>
          <div class="thread-meta">
            <div class="meta-item">
              <i class="fas fa-user"></i>
              <span
                >Szerző:
                <strong>{{
                  thread.author?.username || "Ismeretlen"
                }}</strong></span
              >
            </div>
            <div class="meta-item">
              <i class="fas fa-clock"></i>
              <span>{{ formatDate(thread.createdAt) }}</span>
            </div>
            <div class="meta-item">
              <i class="fas fa-tags"></i>
              <span class="thread-tags">
                <span
                  v-for="tag in threadTags"
                  :key="tag.id"
                  class="thread-tag"
                  :class="getTagClass(tag)"
                  :style="{
                    borderColor: tag.color,
                    backgroundColor: tag.color + '20',
                  }"
                >
                  {{ tag.name }}
                </span>
              </span>
            </div>
            <div class="meta-item">
              <i class="fas fa-comments"></i>
              <span
                >{{ threadRepliesCount }}
                {{ threadRepliesCount === 1 ? "válasz" : "válaszok" }}</span
              >
            </div>
            <div class="meta-item">
              <i class="fas fa-layer-group"></i>
              <span>Kategória: {{ thread.category?.name }}</span>
            </div>
          </div>
        </div>

        <div class="thread-content">
          <div class="content-card">
            <div class="content-body">
              {{ thread.content }}
            </div>
          </div>
        </div>

        <div class="replies-section">
  <h2>
    <i class="fas fa-comments"></i>
    {{ threadRepliesCount === 1 ? "Válasz" : "Válaszok" }}
    {{ threadRepliesCount }}
  </h2>

  <div v-if="replies.length > 0" class="replies-list">
    <div v-for="reply in replies" :key="reply.id" class="reply-card">
      <div class="reply-header">
        <div class="reply-author">
          <div class="profile-image-container">
            <img
              :src="getProfileImageSrc(reply.author?.profileImages)"
              :alt="reply.author?.username || 'User'"
              class="author-profile-image"
              @error="handleImageError"
              @load="onImageLoad"
            />
          </div>
          <div class="author-info">
            <span class="author-name">{{
              reply.author?.username || "Ismeretlen"
            }}</span>
          </div>
        </div>
        <div class="reply-time">{{ formatDate(reply.createdAt) }}</div>
      </div>
      <div class="reply-content">{{ reply.content }}</div>
    </div>
  </div>

  <div v-else class="no-replies">
    <i class="fas fa-comment-slash"></i>
    <p>Nincs megjeleníthető válasz. Válaszolj elsőként!</p>
  </div>

  <div class="reply-form">
    <h3>Válasz Küldése</h3>
    <textarea
      v-model="replyContent"
      class="form-input"
      placeholder="Ide írd a válaszod tartalmát..."
      rows="5"
      ref="replyTextarea"
      :class="{ 'form-input-error': formError }"
      @input="clearFormError"
    ></textarea>

    <div v-if="formError" class="error-message">
      <i class="fas fa-exclamation-circle"></i> {{ formError }}
    </div>

    <div class="form-actions">
      <button
        class="forum-btn forum-btn-primary"
        @click="submitReply"
        :disabled="!replyContent.trim() || isSubmitting"
      >
        <span v-if="isSubmitting">
          <i class="fas fa-spinner fa-spin"></i> Küldés...
        </span>
        <span v-else>
          <i class="fas fa-paper-plane"></i> Válasz Küldése
        </span>
      </button>
    </div>
  </div>
</div>
      </div>

      <div v-else class="not-found">
        <i class="fas fa-exclamation-triangle"></i>
        <h2>A Téma Nem Található</h2>
        <p>Az általad keresett téma nem található vagy eltávolításra került.</p>
        <button class="back-button" @click="goBack">
          <i class="fas fa-arrow-left"></i> Visszatérés a Fórumra
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import defaultProfileImage from '/src/assets/userrrr.png'

const route = useRoute();
const router = useRouter();

const thread = ref(null);
const loading = ref(true);
const replyContent = ref("");
const formError = ref("");
const isSubmitting = ref(false);
const replies = ref([]);

const API_BASE =
  "https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api";

const onImageLoad = (event) => {
  console.log('Profile image loaded successfully:', event.target.src)
};

const handleImageError = (event) => {
  console.error('Profile image failed to load, using default:', event.target.src)
  event.target.src = defaultProfileImage
  event.target.onerror = null
};

const fetchThread = async () => {
  try {
    loading.value = true;
    const response = await axios.get(`${API_BASE}/Threads/${route.params.id}`);
    thread.value = response.data;

    await fetchReplies();
  } catch (error) {
    console.error("Error fetching thread:", error);
    thread.value = null;
  } finally {
    loading.value = false;
  }
};

const fetchReplies = async () => {
  try {
    console.log("🔍 Fetching replies via new endpoint...");

    const response = await axios.get(
      `${API_BASE}/Threads/${route.params.id}/posts`,
    );

    console.log("🔍 Replies response:", response.data);

    if (response.data && Array.isArray(response.data)) {
      replies.value = response.data;
    } else if (response.data && response.data.data) {
      replies.value = response.data.data;
    } else {
      replies.value = [];
    }

    console.log("🔍 Loaded replies:", replies.value.length);
  } catch (error) {
    console.error("Error fetching replies:", error);
    console.error("Error response:", error.response?.data);
    replies.value = [];
  }
};

onMounted(() => {
  if (!document.querySelector('link[href*="font-awesome"]')) {
    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href =
      "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css";
    document.head.appendChild(link);
  }

  fetchThread();
});

const goBack = () => {
  router.push("/forum");
};

const getTagClass = (tag) => {
  if (!tag) return "tag-default";

  const tagName = tag.name.toLowerCase();
  const tagClasses = {
    announcement: "tag-announcement",
    discussion: "tag-discussion",
    question: "tag-discussion",
    design: "tag-design",
    help: "tag-help",
    tools: "tag-tools",
    feedback: "tag-discussion",
  };
  return tagClasses[tagName] || "tag-default";
};

const clearFormError = () => {
  formError.value = "";
};

const submitReply = async () => {
  const content = replyContent.value.trim();

  if (!content) {
    formError.value = "Kérjük, írj be egy választ";
    return;
  }

  if (!thread.value?.id) {
    formError.value = "Téma ID nem található";
    return;
  }

  const getToken = () => {
    const token =
      localStorage.getItem("auth_token") ||
      localStorage.getItem("token") ||
      sessionStorage.getItem("auth_token") ||
      sessionStorage.getItem("token");

    console.log(
      "🔍 Retrieved token:",
      token ? token.substring(0, 50) + "..." : "None",
    );
    return token;
  };

  const token = getToken();

  if (!token) {
    formError.value = "Kérjük, jelentkezz be a válasz írásához";
    console.error("❌ No token found in any storage location");
    console.log("Available localStorage keys:", Object.keys(localStorage));
    router.push("/login");
    return;
  }

  isSubmitting.value = true;

  try {
    console.log(
      "🔍 Making POST request to:",
      `${API_BASE}/Threads/${thread.value.id}/posts`,
    );
    console.log("🔍 Using token (first 20 chars):", token.substring(0, 20));

    const response = await axios.post(
      `${API_BASE}/Threads/${thread.value.id}/posts`,
      {
        content: content,
        parentPostId: null,
      },
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        timeout: 10000,
      },
    );

    console.log("✅ Reply posted successfully! Status:", response.status);
    console.log("✅ Response data:", response.data);

    replies.value.push(response.data);

    if (thread.value) {
      thread.value.postCount = (thread.value.postCount || 0) + 1;
    }

    replyContent.value = "";
    formError.value = "";
  } catch (error) {
    console.error("❌ Full error object:", error);
    console.error("❌ Error config:", error.config);
    console.error("❌ Error response:", error.response);

    if (error.response) {
      console.error("❌ Status:", error.response.status);
      console.error("❌ Headers:", error.response.headers);
      console.error("❌ Data:", error.response.data);

      if (error.response.status === 401) {
        if (error.response.data?.message?.includes("token")) {
          formError.value = "A munkamenet lejárt. Kérjük, jelentkezz be újra.";
          localStorage.removeItem("auth_token");
          setTimeout(() => router.push("/login"), 1500);
        } else {
          formError.value =
            "Hitelesítés sikertelen. Kérjük, jelentkezz be újra.";
        }
      } else if (error.response.status === 400) {
        formError.value = error.response.data?.message || "Érvénytelen kérés";
      } else {
        formError.value = "Szerver hiba. Kérjük, próbáld újra.";
      }
    } else if (error.request) {
      console.error("❌ No response received:", error.request);
      formError.value =
        "Hálózati hiba. Kérjük, ellenőrizd az internetkapcsolatod.";
    } else {
      console.error("❌ Request setup error:", error.message);
      formError.value = "Hiba: " + error.message;
    }
  } finally {
    isSubmitting.value = false;
  }
};

const formatDate = (dateString) => {
  if (!dateString) return "Ismeretlen dátum";

  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now - date;
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMins < 60) {
    return `${diffMins} perce`;
  } else if (diffHours < 24) {
    return `${diffHours} órája`;
  } else if (diffDays < 7) {
    return `${diffDays} napja`;
  } else {
    return date.toLocaleDateString("hu-HU", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  }
};

const threadTags = computed(() => {
  return thread.value?.tags || [];
});

const threadRepliesCount = computed(() => {
  return thread.value?.postCount || replies.value.length || 0;
});

const getProfileImageSrc = (profileImages) => {
  if (!profileImages) {
    return defaultProfileImage
  }

  if (typeof profileImages === 'string') {
    if (profileImages.trim() === '') {
      return defaultProfileImage
    }

    return `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${profileImages}?t=${Date.now()}`
  }
  
 
  if (Array.isArray(profileImages) && profileImages.length > 0) {
    const firstImage = profileImages[0]
    if (firstImage && typeof firstImage === 'string' && firstImage.trim() !== '') {
      return `https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev${firstImage}?t=${Date.now()}`
    }
  }

  return defaultProfileImage
};
</script>

<style scoped>
@import url("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css");

.thread-view {
  min-height: 100vh;
  background: #1c1c1c;
  color: #fae9d7;
  padding: 40px 20px;
}

.container {
  max-width: 900px;
  margin: 0 auto;
}

.loading {
  text-align: center;
  padding: 60px;
  color: #aaa;
  font-size: 1.1rem;
  background: #2c2c2c;
  border-radius: 10px;
  border: 1px solid #444;
}

.loading i {
  margin-right: 10px;
  color: #e24c4f;
}

.back-button {
  background: #2c2c2c;
  border: 1px solid #444;
  color: #fae9d7;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 25px;
  transition: all 0.2s;
  font-size: 0.95rem;
  font-family: "Belleza", sans-serif;
}

.back-button:hover {
  background: #3a3a3a;
  border-color: #e24c4f;
  transform: translateY(-1px);
}

.thread-header {
  background: #2c2c2c;
  border-radius: 10px;
  padding: 30px;
  margin-bottom: 30px;
  border: 1px solid #444;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.3);
  border-left: 4px solid #e24c4f;
}

.thread-title {
  font-size: 2rem;
  font-weight: 600;
  margin-bottom: 20px;
  color: #fae9d7;
  line-height: 1.3;
  letter-spacing: 0.5px;
}

.thread-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  padding-top: 15px;
  border-top: 1px solid #444;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
  color: #aaa;
}

.meta-item i {
  color: #e24c4f;
  width: 16px;
}

.meta-item strong {
  color: #ddd;
}

.thread-tag {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
  text-transform: lowercase;
}

.tag-announcement {
  background: rgba(76, 175, 80, 0.2);
  color: #4caf50;
  border: 1px solid rgba(76, 175, 80, 0.3);
}

.tag-discussion {
  background: rgba(33, 150, 243, 0.2);
  color: #2196f3;
  border: 1px solid rgba(33, 150, 243, 0.3);
}

.tag-design {
  background: rgba(156, 39, 176, 0.2);
  color: #9c27b0;
  border: 1px solid rgba(156, 39, 176, 0.3);
}

.tag-help {
  background: rgba(255, 152, 0, 0.2);
  color: #ff9800;
  border: 1px solid rgba(255, 152, 0, 0.3);
}

.tag-tools {
  background: rgba(0, 150, 136, 0.2);
  color: #009688;
  border: 1px solid rgba(0, 150, 136, 0.3);
}

.tag-default {
  background: rgba(158, 158, 158, 0.2);
  color: #9e9e9e;
  border: 1px solid rgba(158, 158, 158, 0.3);
}

.thread-content {
  margin-bottom: 40px;
}

.content-card {
  background: #2c2c2c;
  border-radius: 10px;
  padding: 30px;
  border: 1px solid #444;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.3);
}

.author-info {
  display: flex;
  align-items: center;
  gap: 15px;
}

.author-profile-image {
  width: 100%;
  height: 100%;
  object-fit: cover; /* This ensures the image covers the container */
  border-radius: 50%;
  display: block;
}

.author-info i {
  font-size: 2.5rem;
  color: #e24c4f;
}

.author-info strong {
  color: #fae9d7;
  font-size: 1.1rem;
}

.post-time {
  font-size: 0.9rem;
  color: #888;
  margin-top: 4px;
}

.content-body {
  font-size: 1.1rem;
  line-height: 1.7;
  color: #ddd;
  white-space: pre-wrap;
}

.replies-section {
  background: #2c2c2c;
  border-radius: 10px;
  padding: 30px;
  border: 1px solid #444;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.3);
}

.replies-section h2 {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 25px;
  color: #fae9d7;
  display: flex;
  align-items: center;
  gap: 10px;
  letter-spacing: 0.5px;
}

.replies-section h2 i {
  color: #e24c4f;
}

.replies-list {
  margin-bottom: 30px;
}

.reply-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  padding: 16px;
  margin-bottom: 12px;
  border: 1px solid rgba(250, 233, 215, 0.1);
  transition: all 0.3s ease;
}

.reply-card:hover {
  background: rgba(255, 255, 255, 0.08);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.reply-card.user-reply {
  border-left-color: #e24c4f;
  background: #3a2a2a;
}

.reply-card.user-reply:hover {
  background: #443030;
}

.reply-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid rgba(250, 233, 215, 0.1);
}

.reply-author {
  display: flex;
  align-items: center;
  gap: 12px;
}

.reply-author i {
  font-size: 1.5rem;
  color: #888;
}

.author-info {
  display: flex;
  flex-direction: column;
}

.author-name {
  font-weight: 600;
  color: #fae9d7;
  font-size: 16px;
}

.reply-badge {
  background-color: #e24c4f;
  color: white;
  font-size: 0.7rem;
  padding: 2px 8px;
  border-radius: 10px;
  font-weight: 600;
  align-self: flex-start;
}

.reply-time {
  font-size: 14px;
  color: #aaa;
}

.reply-content {
  color: #fae9d7;
  line-height: 1.6;
  font-size: 15px;
  white-space: pre-wrap;
  word-break: break-word;
}

.no-replies {
  text-align: center;
  padding: 40px;
  color: #888;
  background: #2c2c2c;
  border-radius: 8px;
  border: 1px dashed #444;
  margin-bottom: 30px;
}

.no-replies i {
  font-size: 3rem;
  margin-bottom: 15px;
  opacity: 0.5;
  color: #666;
}

.no-replies p {
  font-size: 1.1rem;
  color: #aaa;
}

.reply-form {
  margin-top: 30px;
  padding-top: 30px;
  border-top: 1px solid #444;
}

.reply-form h3 {
  font-size: 1.2rem;
  margin-bottom: 15px;
  color: #fae9d7;
  font-weight: 600;
}

.form-input {
  width: 100%;
  padding: 14px;
  background: #333;
  border: 2px solid #444;
  border-radius: 6px;
  font-size: 1rem;
  transition: all 0.2s;
  line-height: 1.5;
  color: #fae9d7;
  font-family: "Belleza", sans-serif;
  resize: vertical;
  min-height: 120px;
}

.form-input:focus {
  outline: none;
  border-color: #e24c4f;
  box-shadow: 0 0 0 3px rgba(226, 76, 79, 0.1);
  background: #3a3a3a;
}

.form-input-error {
  border-color: #e74c3c !important;
  animation: shake 0.3s ease-in-out;
}

.error-message {
  color: #e74c3c;
  font-size: 0.85rem;
  margin-top: 8px;
  display: flex;
  align-items: flex-start;
  gap: 8px;
  line-height: 1.3;
}

.error-message i {
  font-size: 0.9rem;
  margin-top: 1px;
  flex-shrink: 0;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 15px;
}

.forum-btn {
  padding: 12px 30px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  border: none;
  transition: all 0.2s;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: "Belleza", sans-serif;
}

.forum-btn-primary {
  background: linear-gradient(135deg, #e24c4f, #c53c3f);
  color: white;
  box-shadow: 0 4px 12px rgba(226, 76, 79, 0.3);
}

.forum-btn-primary:hover:not(:disabled) {
  background: linear-gradient(135deg, #c53c3f, #e24c4f);
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(226, 76, 79, 0.4);
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

button:disabled:hover {
  transform: none !important;
  box-shadow: none !important;
}

.not-found {
  text-align: center;
  padding: 60px 20px;
  max-width: 500px;
  margin: 0 auto;
  background: #2c2c2c;
  border-radius: 10px;
  border: 1px solid #444;
}

.not-found i {
  font-size: 4rem;
  color: #e24c4f;
  margin-bottom: 20px;
  opacity: 0.8;
}

.not-found h2 {
  font-size: 1.8rem;
  margin-bottom: 15px;
  color: #fae9d7;
  font-weight: 600;
}

.not-found p {
  color: #aaa;
  margin-bottom: 30px;
  line-height: 1.6;
}

@keyframes shake {
  0%,
  100% {
    transform: translateX(0);
  }

  10%,
  30%,
  50%,
  70%,
  90% {
    transform: translateX(-3px);
  }

  20%,
  40%,
  60%,
  80% {
    transform: translateX(3px);
  }
}

@keyframes fa-spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.fa-spin {
  animation: fa-spin 1s infinite linear;
}

@media (max-width: 768px) {
  .thread-view {
    padding: 30px 15px;
  }

  .container {
    padding: 0;
  }

  .thread-header {
    padding: 20px;
  }

  .thread-title {
    font-size: 1.5rem;
  }

  .thread-meta {
    flex-direction: column;
    gap: 10px;
  }

  .content-card,
  .replies-section {
    padding: 20px;
  }

  .reply-header {
    flex-direction: column;
    gap: 5px;
    align-items: flex-start;
  }

  .reply-time {
    align-self: flex-start;
  }
}

@media (max-width: 480px) {
  .thread-view {
    padding: 20px 10px;
  }

  .thread-title {
    font-size: 1.3rem;
  }

  .content-body {
    font-size: 1rem;
  }

  .forum-btn {
    padding: 10px 20px;
    font-size: 0.95rem;
  }

  .replies-section h2 {
    font-size: 1.3rem;
  }

  .reply-form h3 {
    font-size: 1.1rem;
  }

  .form-input {
    padding: 12px;
    font-size: 0.95rem;
  }
}
.profile-image-container {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  overflow: visible; 
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid #d55053;
  background-color: #2c2c2c;
}

.profile-image-fallback {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fae9d7;
  font-size: 20px;
  border-radius: 50%;
  background-color: #444;
}


.profile-image-container.small {
  width: 40px;
  height: 40px;
}

.profile-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
