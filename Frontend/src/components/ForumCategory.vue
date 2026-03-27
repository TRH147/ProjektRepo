<template>
  <div class="forum-category">
    <div class="category-header">
      <h2 class="category-title">{{ category.name }}</h2>
      <div class="category-meta">
        <span class="thread-count">{{ category.threads.length }} Téma</span>
      </div>
    </div>
    
    <div class="threads-list">
      <div 
        v-for="thread in category.threads" 
        :key="thread.id" 
        class="thread-item"
        @click="navigateToThread(thread.id)"
      >
        <div class="thread-main">
          <div class="thread-header">
            <h3 class="thread-title">{{ thread.title }}</h3>
            <div v-if="thread.isPinned || thread.isLocked" class="thread-status">
              <span v-if="thread.isPinned" class="pinned-badge">
                <i class="fas fa-thumbtack"></i> Kitűzve
              </span>
              <span v-if="thread.isLocked" class="locked-badge">
                <i class="fas fa-lock"></i> Lezárva
              </span>
            </div>
          </div>
          <div class="thread-meta">
            <span class="thread-author">Szerző: {{ thread.author }}</span>
            <span class="thread-time">, {{ thread.timeAgo }}</span>
          </div>
        </div>
        
        <div class="thread-details">
          <span class="thread-tag" :class="getTagClass(thread.tag)">{{ thread.tag }}</span>
          
          <div class="thread-stats">
            <div class="stat-group">
              <div class="stat-item" title="Megtekintések">
                <i class="fas fa-eye"></i>
                <span class="stat-count">{{ thread.viewCount || 0 }}</span>
              </div>
              <div class="stat-item" title="Válaszok">
                <i class="fas fa-comment"></i>
                <span class="stat-count">{{ thread.replies || 0 }}</span>
              </div>
            </div>
            
            <span v-if="thread.lastReply" class="thread-last-reply">
              Legutóbbi válasz: {{ thread.lastReply }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const props = defineProps({
  category: {
    type: Object,
    required: true
  }
})

const router = useRouter()

const navigateToThread = (threadId) => {
  router.push(`/thread/${threadId}`)
}

const getTagClass = (tag) => {
  const tagClasses = {
    'announcement': 'tag-announcement',
    'discussion': 'tag-discussion',
    'design': 'tag-design',
    'help': 'tag-help',
    'tools': 'tag-tools'
  }
  return tagClasses[tag] || 'tag-default'
}
</script>

<style scoped>
.forum-category {
  background: #2c2c2c;
  border-radius: 10px;
  border: 1px solid #444;
  overflow: hidden;
  margin-bottom: 20px;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.forum-category:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.category-header {
  background: #333;
  padding: 20px;
  border-bottom: 1px solid #444;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.category-title {
  font-size: 1.5rem;
  color: #fae9d7;
  font-weight: 600;
  margin: 0;
  letter-spacing: 0.5px;
}

.category-meta {
  font-size: 0.9rem;
  color: #aaa;
}

.thread-count {
  background: #444;
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 500;
}

.threads-list {
  padding: 0;
}

.thread-item {
  padding: 20px;
  border-bottom: 1px solid #3a3a3a;
  cursor: pointer;
  transition: background-color 0.2s ease;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.thread-item:last-child {
  border-bottom: none;
}

.thread-item:hover {
  background-color: #333;
}

.thread-main {
  flex: 1;
}

.thread-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 8px;
}

.thread-title {
  font-size: 1.1rem;
  color: #fae9d7;
  margin: 0;
  font-weight: 500;
  line-height: 1.4;
}

.thread-title::before {
  content: "•";
  color: #e24c4f;
  margin-right: 8px;
  font-size: 1.2rem;
}

.thread-status {
  display: flex;
  gap: 8px;
}

.pinned-badge {
  background: rgba(76, 175, 80, 0.2);
  color: #4CAF50;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.7rem;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.locked-badge {
  background: rgba(244, 67, 54, 0.2);
  color: #f44336;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.7rem;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.thread-meta {
  font-size: 0.9rem;
  color: #aaa;
}

.thread-author {
  color: #ddd;
}

.thread-time {
  color: #888;
}

.thread-details {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 12px;
  margin-left: 20px;
  min-width: 180px;
}

.thread-tag {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

.thread-stats {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 8px;
}

.stat-group {
  display: flex;
  gap: 15px;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 6px;
  color: #aaa;
  font-size: 0.85rem;
}

.stat-item i {
  color: #888;
  font-size: 0.8rem;
}

.stat-count {
  color: #fae9d7;
  font-weight: 500;
  min-width: 20px;
  text-align: center;
}

.stat-item:hover {
  color: #e24c4f;
}

.stat-item:hover i {
  color: #e24c4f;
}

.thread-last-reply {
  font-size: 0.8rem;
  color: #888;
}

.tag-announcement {
  background: rgba(76, 175, 80, 0.2);
  color: #4CAF50;
  border: 1px solid rgba(76, 175, 80, 0.3);
}

.tag-discussion {
  background: rgba(33, 150, 243, 0.2);
  color: #2196F3;
  border: 1px solid rgba(33, 150, 243, 0.3);
}

.tag-design {
  background: rgba(156, 39, 176, 0.2);
  color: #9C27B0;
  border: 1px solid rgba(156, 39, 176, 0.3);
}

.tag-help {
  background: rgba(255, 152, 0, 0.2);
  color: #FF9800;
  border: 1px solid rgba(255, 152, 0, 0.3);
}

.tag-tools {
  background: rgba(0, 150, 136, 0.2);
  color: #009688;
  border: 1px solid rgba(0, 150, 136, 0.3);
}

.tag-default {
  background: rgba(158, 158, 158, 0.2);
  color: #9E9E9E;
  border: 1px solid rgba(158, 158, 158, 0.3);
}

@media (max-width: 768px) {
  .forum-category {
    margin-bottom: 15px;
  }
  
  .category-header {
    padding: 15px;
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .category-title {
    font-size: 1.3rem;
  }
  
  .thread-item {
    flex-direction: column;
    align-items: flex-start;
    padding: 15px;
    gap: 15px;
  }
  
  .thread-details {
    margin-left: 0;
    align-items: flex-start;
    min-width: auto;
    width: 100%;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
  }
  
  .thread-stats {
    align-items: flex-end;
  }
  
  .stat-group {
    gap: 10px;
  }
}

@media (max-width: 480px) {
  .category-header {
    padding: 12px;
  }
  
  .category-title {
    font-size: 1.2rem;
  }
  
  .thread-item {
    padding: 12px;
  }
  
  .thread-title {
    font-size: 1rem;
  }
  
  .thread-details {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .thread-stats {
    width: 100%;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
  }
  
  .stat-group {
    flex-direction: column;
    gap: 4px;
  }
  
  .stat-item {
    font-size: 0.8rem;
  }
}
</style>