<template>
  <div class="create-thread-page">
    <div class="container">
      <div class="page-header">
        <button class="back-button" @click="goBack">
          <i class="fas fa-arrow-left"></i> Visszatérés a fórumra
        </button>
        <h1>Téma Létrehozása</h1>
        <p class="page-subtitle">Új beszélgetés kezdeményezése</p>
      </div>

      <div class="form-container">
        <form @submit.prevent="handleSubmit" id="threadForm">
          <div class="form-group">
            <label for="threadTitle">Téma Címe</label>
            <input type="text" id="threadTitle" class="form-input" :class="{ 'form-input-error': errors.title }"
              placeholder="Add meg a téma címét" v-model="form.title" @input="clearError('title')">
            <div v-if="errors.title" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              <span>{{ errors.title }}</span>
            </div>
          </div>

          <div class="form-group">
            <label for="threadContent">Tartalom</label>
            <textarea id="threadContent" class="form-input" :class="{ 'form-input-error': errors.content }"
              placeholder="Ide írd a témád tartalmát..." v-model="form.content" @input="clearError('content')"
              rows="8"></textarea>
            <div v-if="errors.content" class="error-message">
              <i class="fas fa-exclamation-circle"></i>
              <span>{{ errors.content }}</span>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group half">
              <label for="threadTag">Cimke</label>
              <div class="dropdown-wrapper">
                <select id="threadTag" class="form-select" :class="{ 'form-input-error': errors.tag }"
                  v-model="form.tagId" @change="clearError('tag')">
                  <option value="" disabled>Válassz egy cimkét</option>
                  <option v-for="tag in availableTags" :key="tag.id" :value="tag.id">
                    {{ tag.name }}
                  </option>
                </select>
              </div>
              <div v-if="loadingTags" class="loading-indicator">
                <i class="fas fa-spinner fa-spin"></i> Cimkék betöltése...
              </div>
              <div v-if="tagsError" class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Hiba a cimkék betöltésekor
              </div>
              <div v-if="errors.tag" class="error-message">
                <i class="fas fa-exclamation-circle"></i>
                <span>{{ errors.tag }}</span>
              </div>
            </div>

            <div class="form-group half">
              <label for="threadCategory">Kategória</label>
              <div class="dropdown-wrapper">
                <select id="threadCategory" class="form-select" :class="{ 'form-input-error': errors.category }"
                  v-model="form.categoryId" @change="clearError('category')">
                  <option value="" disabled>Válassz egy kategóriát</option>
                  <option v-for="category in availableCategories" :key="category.id" :value="category.id">
                    {{ category.name }}
                  </option>
                </select>
              </div>
              <div v-if="loadingCategories" class="loading-indicator">
                <i class="fas fa-spinner fa-spin"></i> Kategóriák betöltése...
              </div>
              <div v-if="categoriesError" class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Hiba a kategóriák betöltésekor
              </div>
              <div v-if="errors.category" class="error-message">
                <i class="fas fa-exclamation-circle"></i>
                <span>{{ errors.category }}</span>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" class="forum-btn forum-btn-secondary" @click="goBack" :disabled="isSubmitting">
              Mégse
            </button>
            <button type="submit" class="forum-btn forum-btn-primary"
              :disabled="isSubmitting || loadingTags || loadingCategories">
              <span v-if="isSubmitting">
                <i class="fas fa-spinner fa-spin"></i> Téma létrehozása...
              </span>
              <span v-else>
                <i class="fas fa-paper-plane"></i> Téma Létrehozása
              </span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="showSuccessModal" class="success-modal-overlay" @click.self="closeSuccessModal">
      <div class="success-modal">
        <div class="success-icon">
          <i class="fas fa-check-circle"></i>
        </div>
        <h2>Téma Sikeresen Létrehozva!</h2>
        <p>A témád "<strong>{{ createdThreadTitle }}</strong>" létrehozásra került.</p>
        <div class="success-actions">
          <button class="forum-btn forum-btn-secondary" @click="closeSuccessModal">
            <i class="fas fa-times"></i> Bezárás
          </button>
          <button class="forum-btn forum-btn-primary" @click="viewThread">
            <i class="fas fa-eye"></i> Téma Megtekintése
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()

const form = ref({
  title: '',
  content: '',
  tagId: '',
  categoryId: ''
})

const errors = ref({
  title: '',
  content: '',
  tag: '',
  category: ''
})

const isSubmitting = ref(false)
const showSuccessModal = ref(false)
const createdThreadTitle = ref('')
const createdThreadId = ref(null)

const availableTags = ref([])
const availableCategories = ref([])
const loadingTags = ref(true)
const loadingCategories = ref(true)
const tagsError = ref(false)
const categoriesError = ref(false)

onMounted(async () => {
  if (!document.querySelector('link[href*="font-awesome"]')) {
    const link = document.createElement('link')
    link.rel = 'stylesheet'
    link.href = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css'
    document.head.appendChild(link)
  }

  await Promise.all([fetchTags(), fetchCategories()])

  document.getElementById('threadTitle')?.focus()
})

const fetchTags = async () => {
  try {
    loadingTags.value = true
    tagsError.value = false

    console.log('Fetching tags from API...')
    const response = await api.get('/Threads/tags')

    if (response.data && Array.isArray(response.data)) {
      availableTags.value = response.data.map(tag => ({
        id: tag.id,
        name: tag.name
      }))
      console.log(`Loaded ${availableTags.value.length} tags:`, availableTags.value)
    } else {
      console.warn('Unexpected tags response format:', response.data)
      availableTags.value = []
    }
  } catch (error) {
    console.error('Error fetching tags:', error)
    tagsError.value = true
    availableTags.value = []
  } finally {
    loadingTags.value = false
  }
}

const fetchCategories = async () => {
  try {
    loadingCategories.value = true
    categoriesError.value = false

    console.log('Fetching categories from API...')
    const response = await api.get('/Threads/categories')

    if (response.data && Array.isArray(response.data)) {
      availableCategories.value = response.data.map(category => ({
        id: category.id,
        name: category.name
      }))
      console.log(`Loaded ${availableCategories.value.length} categories:`, availableCategories.value)
    } else {
      console.warn('Unexpected categories response format:', response.data)
      availableCategories.value = []
    }
  } catch (error) {
    console.error('Error fetching categories:', error)
    categoriesError.value = true
    availableCategories.value = []
  } finally {
    loadingCategories.value = false
  }
}

const goBack = () => {
  router.push('/forum')
}

const clearError = (field) => {
  if (errors.value[field]) {
    errors.value[field] = ''
  }
}

const validateForm = () => {
  let isValid = true

  errors.value = {
    title: '',
    content: '',
    tag: '',
    category: ''
  }

  if (!form.value.title.trim()) {
    errors.value.title = 'Please enter a thread title'
    isValid = false
  } else if (form.value.title.trim().length < 3) {
    errors.value.title = 'Title must be at least 3 characters'
    isValid = false
  }

  if (!form.value.content.trim()) {
    errors.value.content = 'Please enter thread content'
    isValid = false
  } else if (form.value.content.trim().length < 10) {
    errors.value.content = 'Content must be at least 10 characters'
    isValid = false
  }

  if (!form.value.categoryId) {
    errors.value.category = 'Please select a category'
    isValid = false
  }

  if (!form.value.tagId) {
    errors.value.tag = 'Please select a tag'
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) {
    return
  }
  
  isSubmitting.value = true
  
  try {
    const threadData = {
      title: form.value.title.trim(),
      content: form.value.content.trim(),
      categoryId: parseInt(form.value.categoryId),
      tagIds: [parseInt(form.value.tagId)] 
    }
    
    console.log('Submitting thread data:', threadData)
    
    const response = await api.post('/Threads', threadData)
    
    createdThreadTitle.value = threadData.title
    createdThreadId.value = response.data.id || Date.now()
    
    form.value = {
      title: '',
      content: '',
      tagId: '',
      categoryId: ''
    }
    
    isSubmitting.value = false
    showSuccessModal.value = true
    
  } catch (error) {
    console.error('Error creating thread:', error)
    
    isSubmitting.value = false
  }
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  goBack()
}

const viewThread = () => {
  if (createdThreadId.value) {
    router.push(`/thread/${createdThreadId.value}`)
  } else {
    goBack()
  }
}
</script>

<style scoped>
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css');

.create-thread-page {
  min-height: 100vh;
  background: #1c1c1c;
  color: #fae9d7;
  padding: 40px 20px;
}

.container {
  max-width: 800px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 40px;
  text-align: center;
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

.page-header h1 {
  font-size: 2.2rem;
  font-weight: 600;
  color: #fae9d7;
  margin-bottom: 10px;
  letter-spacing: 0.5px;
}

.page-subtitle {
  color: #aaa;
  font-size: 1.1rem;
}

.form-container {
  background: #2c2c2c;
  border-radius: 10px;
  padding: 40px;
  border: 1px solid #444;
  box-shadow: 0 5px 20px rgba(0, 0, 0, 0.3);
}

.form-group {
  margin-bottom: 25px;
}

.form-group label {
  display: block;
  margin-bottom: 10px;
  font-weight: 600;
  color: #fae9d7;
  font-size: 1rem;
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
}

.form-input:focus {
  outline: none;
  border-color: #e24c4f;
  box-shadow: 0 0 0 3px rgba(226, 76, 79, 0.1);
  background: #3a3a3a;
}

textarea.form-input {
  min-height: 200px;
  resize: vertical;
  line-height: 1.6;
}

.form-select {
  width: 100%;
  padding: 14px;
  background: #333;
  border: 2px solid #444;
  border-radius: 6px;
  font-size: 1rem;
  color: #fae9d7;
  transition: all 0.2s;
  line-height: 1.5;
  cursor: pointer;
  font-family: "Belleza", sans-serif;
}

.form-select option {
  font-family: "Belleza", sans-serif;
  color: #fae9d7;
  background: #333;
  padding: 10px;
}

.form-select:focus {
  outline: none;
  border-color: #e24c4f;
  box-shadow: 0 0 0 3px rgba(226, 76, 79, 0.1);
  background: #3a3a3a;
}

.form-row {
  display: flex;
  gap: 20px;
}

.form-row .half {
  flex: 1;
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
  gap: 15px;
  margin-top: 40px;
  padding-top: 30px;
  border-top: 1px solid #444;
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

.forum-btn-secondary {
  background-color: #333;
  color: #fae9d7;
  border: 1px solid #444;
}

.forum-btn-secondary:hover {
  background-color: #3a3a3a;
  border-color: #555;
  transform: translateY(-1px);
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

button:disabled:hover {
  transform: none !important;
  box-shadow: none !important;
}

.success-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.8);
  z-index: 2000;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.success-modal {
  background-color: #2c2c2c;
  border-radius: 12px;
  padding: 40px;
  max-width: 500px;
  width: 100%;
  text-align: center;
  border: 1px solid #444;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  animation: modalAppear 0.3s ease-out;
}

.success-icon {
  font-size: 4rem;
  color: #4CAF50;
  margin-bottom: 20px;
}

.success-modal h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #fae9d7;
  margin-bottom: 15px;
}

.success-modal p {
  color: #ddd;
  font-size: 1.1rem;
  line-height: 1.6;
  margin-bottom: 30px;
}

.success-actions {
  display: flex;
  justify-content: center;
  gap: 15px;
  margin-top: 30px;
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

@keyframes modalAppear {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.95);
  }

  to {
    opacity: 1;
    transform: translateY(0) scale(1);
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
  .create-thread-page {
    padding: 30px 15px;
  }

  .form-container {
    padding: 25px;
  }

  .page-header h1 {
    font-size: 1.8rem;
  }

  .form-row {
    flex-direction: column;
    gap: 0;
  }

  .form-actions {
    flex-direction: column;
  }

  .forum-btn {
    width: 100%;
    justify-content: center;
  }

  .success-modal {
    padding: 25px;
  }

  .success-icon {
    font-size: 3rem;
  }

  .success-modal h2 {
    font-size: 1.5rem;
  }
}

@media (max-width: 480px) {
  .create-thread-page {
    padding: 20px 10px;
  }

  .form-container {
    padding: 20px;
  }

  .page-header h1 {
    font-size: 1.5rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .form-input,
  .form-select {
    padding: 12px;
    font-size: 0.95rem;
  }

  .forum-btn {
    padding: 10px 20px;
    font-size: 0.95rem;
  }

  .success-modal {
    padding: 20px;
  }

  .success-modal h2 {
    font-size: 1.3rem;
  }
}
</style>