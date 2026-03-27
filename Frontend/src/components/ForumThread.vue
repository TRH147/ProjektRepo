<template>
  <div class="forum-thread" @click="openThread">
    <div class="thread-info">
      <div class="thread-title">
        {{ thread.title }}
        <span v-if="thread.replies > 0" class="new-indicator" title="Has replies">
          <i class="fas fa-comment"></i>
        </span>
      </div>
      <div class="thread-meta">
        Szerző: {{ thread.author }}, {{ thread.timestamp }}
        <span class="thread-tag">{{ thread.tag }}</span>
      </div>
    </div>
    <div class="thread-stats">
      <div class="replies-count">{{ thread.replies }} válaszok</div>
      <div>Legutóbbi válasz: {{ thread.lastReply }}</div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const props = defineProps({
  thread: {
    type: Object,
    required: true
  }
})

const router = useRouter()

const openThread = () => {
  router.push(`/thread/${props.thread.id}`)
}
</script>

<style scoped>
.new-indicator {
  margin-left: 8px;
  color: #4a6fa5;
  font-size: 0.8rem;
  opacity: 0.8;
}

.forum-thread {
  cursor: pointer;
  transition: background-color 0.2s;
}

.forum-thread:hover {
  background-color: #f9f9f9;
}
</style>