<template>
  <div>
    <header class="header">
      <img :src="logoImg" alt="Logo" class="logo" />
      <div class="hamburger" @click="toggleMenu">
        <span></span>
        <span></span>
        <span></span>
      </div>
      <nav :class="['navbar', { active: menuActive }]">
        <router-link to="/">Főoldal</router-link>
        <router-link to="/update">Újdonságok</router-link>
        <router-link to="/statics">Statisztika</router-link>
        <router-link to="/forum" class="active">Fórum</router-link>
      </nav>
    </header>

    <div class="banner">
      <img src="/src/assets/bigpicture.webp" alt="banner">
    </div>

    <div class="home">
      <div class="container">
        <div class="forum-header">
          <div class="search-filters">
            <div class="filter-dropdown" v-click-outside="closeTagDropdown">
              <button class="filter-btn" @click="toggleTagDropdown">
                <i class="fas fa-tag"></i>
                <span>{{ selectedTag || "Összes Címke" }}</span>
                <i class="fas fa-chevron-down dropdown-icon"></i>
              </button>
              <div v-if="showTagDropdown" class="dropdown-menu">
                <div class="dropdown-header">
                  <i class="fas fa-filter"></i>
                  Szűrés cimke szerint
                </div>
                <div class="dropdown-item" @click="selectTag(null)">
                  <i class="fas fa-layer-group"></i>
                  Összes cimke
                </div>
                <div v-for="tag in allTags" :key="tag" class="dropdown-item" @click="selectTag(tag)"
                  :class="{ active: selectedTag === tag }">
                  <i class="fas fa-hashtag"></i>
                  {{ tag }}
                  <span v-if="tagsFromDB.find(t => t.name === tag)?.threadCount" class="tag-count">
                    {{tagsFromDB.find(t => t.name === tag).threadCount}}
                  </span>
                </div>
              </div>
            </div>

            <div class="filter-dropdown" v-click-outside="closeCategoryDropdown">
              <button class="filter-btn" @click="toggleCategoryDropdown">
                <i class="fas fa-folder"></i>
                <span>{{ selectedCategory || "Összes Kategória" }}</span>
                <i class="fas fa-chevron-down dropdown-icon"></i>
              </button>
              <div v-if="showCategoryDropdown" class="dropdown-menu">
                <div class="dropdown-header">
                  <i class="fas fa-filter"></i>
                  Szűrés kategória szerint
                </div>
                <div class="dropdown-item" @click="selectCategory(null)">
                  <i class="fas fa-layer-group"></i>
                  Összes kategória
                </div>
                <div v-for="category in allCategories" :key="category.id" class="dropdown-item"
                  @click="selectCategory(category.name)" :class="{ active: selectedCategory === category.name }">
                  <i class="fas fa-folder-open"></i>
                  {{ category.name }}
                  <span class="thread-count-small">
                    {{categoriesFromDB.find(c => c.id === category.id)?.threadCount || 0}}
                  </span>
                </div>
              </div>
            </div>

            <button v-if="hasActiveFilters" class="clear-filters-btn" @click="clearFilters">
              <i class="fas fa-times"></i>
              Szűrés Törlése
            </button>
          </div>

          <div class="header-actions">
            <button class="new-thread-btn" @click="openNewThread">
              <i class="fas fa-plus"></i> Új Téma
            </button>
          </div>
        </div>

        <div v-if="loading && pagination.page === 1" class="loading">
          <i class="fas fa-spinner fa-spin"></i> Témák betöltése...
        </div>

        <div v-else>
          <div v-if="hasAnyThreads">
            <ForumCategory v-for="category in filteredCategories" :key="category.id" :category="category" />
            
            <div v-if="pagination.totalPages > 1" class="pagination-controls">
              <div class="pagination">
                <button @click="goToPage(pagination.page - 1)" :disabled="pagination.page === 1 || loading"
                  class="pagination-btn prev-btn">
                  <i class="fas fa-chevron-left"></i> Előző oldal
                </button>

                <div class="page-numbers">
                  <span v-for="pageNum in visiblePages" :key="pageNum" @click="goToPage(pageNum)" class="page-number"
                    :class="{ active: pageNum === pagination.page }" :disabled="loading">
                    {{ pageNum }}
                  </span>
                  <span v-if="showEllipsis" class="ellipsis">...</span>
                </div>

                <button @click="goToPage(pagination.page + 1)"
                  :disabled="pagination.page === pagination.totalPages || loading" class="pagination-btn next-btn">
                  Következő oldal <i class="fas fa-chevron-right"></i>
                </button>

                <div class="page-info">
                  Oldal {{ pagination.page }} / {{ pagination.totalPages }}
                  <span v-if="pagination.totalCount"> ({{ pagination.totalCount }} összes téma)</span>
                  <span v-if="loading" class="loading-indicator">
                    <i class="fas fa-spinner fa-spin"></i>
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div v-else-if="!loading" class="no-results">
            <i class="fas fa-search"></i>
            <h3>Nem található téma</h3>
            <p>
              Nincs szűrésnek megfelelő téma. Próbálkozz más szűrési feltételekkel.
            </p>
          </div>
        </div>
      </div>
    </div>

    <FooterComponent />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import FooterComponent from '../components/SiteFooter.vue'
import ForumCategory from '../components/ForumCategory.vue'
import { useForumStore } from '../composables/useForumStore.js'
import logoImg from '/src/assets/Logo.png'
import api from '../services/api'

const router = useRouter()

const menuActive = ref(false)
function toggleMenu() {
  menuActive.value = !menuActive.value
}

const { categories, onlineUsers, onlineGuests } = useForumStore();

const showTagDropdown = ref(false);
const showCategoryDropdown = ref(false);
const selectedTag = ref(null);
const selectedCategory = ref(null);

const tagsFromDB = ref([]);
const categoriesFromDB = ref([]);
const threadsFromDB = ref([]);
const loading = ref(false);
const pagination = ref({
  page: 1,
  pageSize: 5,
  totalPages: 1,
  totalCount: 0
});

const vClickOutside = {
  mounted(el, binding) {
    el.clickOutsideEvent = (event) => {
      if (!(el === event.target || el.contains(event.target))) {
        binding.value();
      }
    };
    document.addEventListener("click", el.clickOutsideEvent);
  },
  unmounted(el) {
    document.removeEventListener("click", el.clickOutsideEvent);
  },
};

const fetchForumData = async () => {
  try {
    loading.value = true;

    const tagsResponse = await api.get('/threads/tags');
    tagsFromDB.value = tagsResponse.data;
    console.log('Tags loaded:', tagsFromDB.value.map(t => t.name));

    const categoriesResponse = await api.get('/threads/categories');
    categoriesFromDB.value = categoriesResponse.data;
    console.log('Categories loaded:', categoriesFromDB.value.map(c => c.name));

    await fetchThreads();

    loading.value = false;
  } catch (error) {
    console.error('Error fetching forum data:', error);
    loading.value = false;
  }
};

const fetchThreads = async (page = 1, categoryId = null, tagName = null) => {
  try {
    loading.value = true;
    
    const params = {
      page: page,
      pageSize: pagination.value.pageSize,
      sortBy: 'newest'
    };

    if (categoryId) {
      params.categoryId = categoryId;
    }
    
    if (tagName) {
      params.tag = tagName;
    }
    
    console.log('Fetching threads with params:', params);

    const response = await api.get('/threads', { params });
    
    threadsFromDB.value = response.data.data || [];
    console.log(`Fetched ${threadsFromDB.value.length} threads`);
    
    if (tagName) {
      const tagThreads = threadsFromDB.value.filter(t => 
        t.tags?.some(tag => tag.name === tagName)
      );
      console.log(`Threads with "${tagName}" tag:`, tagThreads.map(t => ({ id: t.id, title: t.title })));
    }
    
    pagination.value = {
      page: response.data.page || page,
      pageSize: response.data.pageSize || pagination.value.pageSize,
      totalCount: response.data.totalCount || 0,
      totalPages: response.data.totalPages || 1
    };
    
    console.log('Pagination:', pagination.value);

  } catch (error) {
    console.error('Error fetching threads:', error);
    threadsFromDB.value = [];
  } finally {
    loading.value = false;
  }
};

const allTags = computed(() => {
  if (tagsFromDB.value.length > 0) {
    return tagsFromDB.value.map(tag => tag.name).sort();
  }

  const tags = new Set();
  threadsFromDB.value.forEach((thread) => {
    if (thread.tags && Array.isArray(thread.tags)) {
      thread.tags.forEach(tag => {
        tags.add(tag.name);
      });
    }
  });
  return Array.from(tags).sort();
});

const allCategories = computed(() => {
  if (categoriesFromDB.value.length > 0) {
    return categoriesFromDB.value.map(cat => ({
      id: cat.id,
      name: cat.name,
      description: cat.description,
      order: cat.order,
      icon: cat.icon,
      threadCount: cat.threadCount || 0
    })).sort((a, b) => a.order - b.order);
  }

  const uniqueCategories = {};
  threadsFromDB.value.forEach(thread => {
    if (thread.category) {
      uniqueCategories[thread.category.id] = {
        id: thread.category.id,
        name: thread.category.name,
        description: thread.category.description,
        order: thread.category.order,
        icon: thread.category.icon,
        threadCount: 0 
      };
    }
  });

  threadsFromDB.value.forEach(thread => {
    if (thread.category && uniqueCategories[thread.category.id]) {
      uniqueCategories[thread.category.id].threadCount++;
    }
  });

  return Object.values(uniqueCategories).sort((a, b) => a.order - b.order);
});

const hasAnyThreads = computed(() => {
  const totalThreadsInCategories = filteredCategories.value.reduce(
    (total, category) => total + category.threads.length, 
    0
  );
  
  return totalThreadsInCategories > 0 || threadsFromDB.value.length > 0;
});

const toggleTagDropdown = () => {
  showTagDropdown.value = !showTagDropdown.value;
  showCategoryDropdown.value = false;
};

const toggleCategoryDropdown = () => {
  showCategoryDropdown.value = !showCategoryDropdown.value;
  showTagDropdown.value = false;
};

const closeTagDropdown = () => {
  showTagDropdown.value = false;
};

const closeCategoryDropdown = () => {
  showCategoryDropdown.value = false;
};

const selectTag = (tagName) => {
  console.log('Selected tag:', tagName);
  selectedTag.value = tagName;
  selectedCategory.value = null;
  showTagDropdown.value = false;

  applyFilters();
};

const selectCategory = (categoryName) => {
  console.log('Selected category:', categoryName);
  selectedCategory.value = categoryName;
  selectedTag.value = null; 
  showCategoryDropdown.value = false;

  applyFilters();
};

const clearFilters = () => {
  console.log('Clearing all filters');
  selectedTag.value = null;
  selectedCategory.value = null;
  showTagDropdown.value = false;
  showCategoryDropdown.value = false;

  pagination.value.page = 1;
  fetchThreads(1);
};

const applyFilters = () => {
  console.log('Applying filters:', {
    tag: selectedTag.value,
    category: selectedCategory.value
  });
  
  const page = 1;

  if (selectedCategory.value) {
    const selectedCat = allCategories.value.find(c => c.name === selectedCategory.value);
    if (selectedCat) {
      fetchThreads(page, selectedCat.id);
    }
  } 
  else if (selectedTag.value) {
    fetchThreads(page, null, selectedTag.value); 
  } 
  else {
    fetchThreads(page);
  }
};

const hasActiveFilters = computed(() => {
  return selectedTag.value !== null || selectedCategory.value !== null;
});

const filteredCategories = computed(() => {
  console.log('=== filteredCategories computed called ===');
  console.log('selectedTag:', selectedTag.value);
  console.log('selectedCategory:', selectedCategory.value);
  console.log('Total threads:', threadsFromDB.value.length);
  
  let filteredThreads = threadsFromDB.value;

  if (selectedTag.value) {
    const beforeCount = filteredThreads.length;
    filteredThreads = filteredThreads.filter(thread => {
      if (!thread.tags || !Array.isArray(thread.tags)) {
        console.log(`Thread ${thread.id} has no tags array`);
        return false;
      }
      
      const hasTag = thread.tags.some(tag => tag.name === selectedTag.value);
      console.log(`Thread ${thread.id} "${thread.title}" has tag "${selectedTag.value}":`, hasTag, 
                  'Tags:', thread.tags.map(t => t.name));
      return hasTag;
    });
    console.log(`Tag filter: ${beforeCount} → ${filteredThreads.length} threads`);
  }

  if (selectedCategory.value) {
    const beforeCount = filteredThreads.length;
    filteredThreads = filteredThreads.filter(thread => {
      if (!thread.category) {
        console.log(`Thread ${thread.id} has no category`);
        return false;
      }
      
      const matchesCategory = thread.category.name === selectedCategory.value;
      console.log(`Thread ${thread.id} matches category "${selectedCategory.value}":`, matchesCategory);
      return matchesCategory;
    });
    console.log(`Category filter: ${beforeCount} → ${filteredThreads.length} threads`);
  }

  const categoriesMap = {};

  filteredThreads.forEach(thread => {
    if (thread.category) {
      const categoryId = thread.category.id;
      const categoryName = thread.category.name;

      if (!categoriesMap[categoryId]) {
        categoriesMap[categoryId] = {
          id: categoryId,
          name: categoryName,
          description: thread.category.description || '',
          threads: []
        };
        console.log(`Created category: ${categoryName} (ID: ${categoryId})`);
      }

      const formattedThread = {
        id: thread.id,
        title: thread.title,
        author: thread.author?.username || 'Unknown',
        timeAgo: formatTimeAgo(thread.createdAt),
        tag: thread.tags && thread.tags.length > 0 ? thread.tags[0].name : 'general',
        replies: thread.postCount || 0,
        lastReply: thread.lastPost ? formatTimeAgo(thread.lastPost.createdAt || thread.lastPost) : 'Még nincs válasz',
        content: thread.content,
        viewCount: thread.viewCount || 0,
        isLocked: thread.isLocked || false,
        isPinned: thread.isPinned || false,
        authorProfileImage: thread.author?.profileImages,
        categoryName: categoryName,
        tags: thread.tags || []
      };

      categoriesMap[categoryId].threads.push(formattedThread);
      console.log(`Added thread ${thread.id} to category ${categoryName}`);
    } else {
      console.log(`Thread ${thread.id} has no category object, skipping`);
    }
  });

  const result = Object.values(categoriesMap)
    .sort((a, b) => {
      const catA = allCategories.value.find(c => c.id === a.id);
      const catB = allCategories.value.find(c => c.id === b.id);
      return (catA?.order || 999) - (catB?.order || 999);
    });
  
  console.log('Final result:', {
    categoryCount: result.length,
    threadsPerCategory: result.map(c => `${c.name}: ${c.threads.length} threads`),
    allThreadIds: result.flatMap(c => c.threads.map(t => t.id))
  });
  
  return result;
});

const formatTimeAgo = (dateString) => {
  if (!dateString) return 'Recently';

  try {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Épp most';
    if (diffMins < 60) return `${diffMins} perccel ezelőtt`;
    if (diffHours < 24) return `${diffHours} órával ezelőtt`;
    if (diffDays < 7) return `${diffDays} napja`;

    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: diffDays > 365 ? 'numeric' : undefined
    });
  } catch (error) {
    return 'Recently';
  }
};


const goToPage = async (page) => {
  if (page < 1 || page > pagination.value.totalPages || loading.value) return;

  loading.value = true;
  
  const categoryId = selectedCategory.value
    ? allCategories.value.find(c => c.name === selectedCategory.value)?.id
    : null;
    
  const tagName = selectedTag.value;

  await fetchThreads(page, categoryId, tagName);
  
  loading.value = false;
  
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

const visiblePages = computed(() => {
  const maxVisible = 5;
  const pages = [];

  let start = Math.max(1, pagination.value.page - Math.floor(maxVisible / 2));
  let end = Math.min(pagination.value.totalPages, start + maxVisible - 1);

  start = Math.max(1, end - maxVisible + 1);

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

const showEllipsis = computed(() => {
  return pagination.value.totalPages > visiblePages.value.length;
});

const openNewThread = () => {
  router.push("/new-thread");
};

onMounted(() => {
  const link = document.createElement('link');
  link.rel = 'stylesheet';
  link.href = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css';
  document.head.appendChild(link);

  fetchForumData();
});
</script>

<style scoped>
@import "/src/assets/forum.css";
</style>