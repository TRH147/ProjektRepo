import { ref, computed } from 'vue'

export function useForumStore() {

const categories = ref([
  {
    id: 1,
    name: 'General Discussion',
    threads: [
      {
        id: 1,
        title: 'Welcome to the forum',
        author: 'admin',
        timeAgo: '2 days ago',
        tag: 'announcement',
        replies: 12,
        lastReply: '5 hours ago'
      },
      {
        id: 2,
        title: 'What are you working on today?',
        author: 'jane_doe',
        timeAgo: '1 day ago',
        tag: 'discussion',
        replies: 8,
        lastReply: '2 hours ago'
      },
      {
        id: 3,
        title: 'Simple design inspiration',
        author: 'designer123',
        timeAgo: '3 days ago',
        tag: 'design',
        replies: 5,
        lastReply: '1 day ago'
      }
    ]
  },
  {
    id: 2,
    name: 'Technical Help',
    threads: [
      {
        id: 4,
        title: 'How to keep things minimal?',
        author: 'new_user',
        timeAgo: '5 days ago',
        tag: 'help',
        replies: 6,
        lastReply: '3 days ago'
      },
      {
        id: 5,
        title: 'Best tools for simple UIs',
        author: 'dev_enthusiast',
        timeAgo: '1 week ago',
        tag: 'tools',
        replies: 15,
        lastReply: 'yesterday'
      }
    ]
  }
])

  const onlineUsers = ref(14)
  const onlineGuests = ref(3)

  const updateOnlineUsers = () => {
    const randomChange = Math.floor(Math.random() * 5) - 2 // -2 to +2
    onlineUsers.value = Math.max(1, 14 + randomChange)
    onlineGuests.value = Math.max(0, 3 + randomChange)
  }

  const addThread = (threadData) => {
    const categoryMap = {
      'general': 'General Discussion',
      'technical': 'Technical Help',
      'design': 'Design & UX',
      'offtopic': 'Off-Topic'
    }

    const categoryName = categoryMap[threadData.category] || 'General Discussion'
    
    let category = categories.value.find(c => c.name === categoryName)
    
    if (!category) {
      category = {
        id: categories.value.length + 1,
        name: categoryName,
        threads: []
      }
      categories.value.push(category)
    }
    
    threadData.userReplies = []
    
    category.threads.unshift(threadData)
  }

  const addReplyToThread = (threadId, replyData) => {
    for (const category of categories.value) {
      const thread = category.threads.find(t => t.id === threadId)
      if (thread) {
        if (!thread.userReplies) {
          thread.userReplies = []
        }
        thread.userReplies.unshift(replyData)
        thread.replies++
        thread.lastReply = 'Just now'
        return true
      }
    }
    return false
  }

  return {
    categories,
    onlineUsers,
    onlineGuests,
    updateOnlineUsers,
    addThread,
    addReplyToThread
  }
}