import ForumCategory from './ForumCategory.vue'
import { createMemoryHistory, createRouter } from 'vue-router'

describe('<ForumCategory />', () => {
  
  const mockCategory = {
    id: 1,
    name: 'Test Category',
    threads: [
      {
        id: 101,
        title: 'First Thread',
        author: 'User1',
        timeAgo: '2 hours ago',
        tag: 'announcement',
        viewCount: 42,
        replies: 5,
        lastReply: '2024-03-03 by User2',
        isPinned: true,
        isLocked: false
      },
      {
        id: 102,
        title: 'Second Thread',
        author: 'User2',
        timeAgo: '5 hours ago',
        tag: 'discussion',
        viewCount: 23,
        replies: 3,
        lastReply: '2024-03-02 by User3',
        isPinned: false,
        isLocked: true
      },
      {
        id: 103,
        title: 'Third Thread',
        author: 'User3',
        timeAgo: '1 day ago',
        tag: 'help',
        viewCount: 10,
        replies: 0,
        lastReply: null,
        isPinned: false,
        isLocked: false
      }
    ]
  }

  const mountWithProps = (categoryOverrides = {}) => {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/thread/:id', name: 'thread', component: { template: '<div>Thread Page</div>' } }
      ]
    })

    const category = {
      ...mockCategory,
      ...categoryOverrides,
      threads: categoryOverrides.threads || mockCategory.threads
    }

    return cy.mount(ForumCategory, {
      props: { category },
      global: {
        plugins: [router]
      }
    }).then(() => {
      return router.isReady()
    }).then(() => {
      cy.wrap(router).as('router')
    })
  }

  describe('Category header', () => {
    it('displays category name', () => {
      mountWithProps({ name: 'Custom Category Name' })
      cy.get('.category-title').should('contain', 'Custom Category Name')
    })

    it('displays correct thread count', () => {
      mountWithProps({ threads: [{ id: 1 }, { id: 2 }, { id: 3 }] })
      cy.get('.thread-count').should('contain', '3 Téma')
    })

    it('handles empty thread list', () => {
      mountWithProps({ threads: [] })
      cy.get('.thread-count').should('contain', '0 Téma')
    })
  })

  describe('Thread list rendering', () => {
    it('renders all threads', () => {
      mountWithProps()
      cy.get('.thread-item').should('have.length', 3)
    })

    it('displays thread titles correctly', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.thread-title').should('contain', 'First Thread')
      cy.get('.thread-item').eq(1).find('.thread-title').should('contain', 'Second Thread')
      cy.get('.thread-item').eq(2).find('.thread-title').should('contain', 'Third Thread')
    })

    it('displays thread author and time', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.thread-author').should('contain', 'Szerző: User1')
      cy.get('.thread-item').eq(0).find('.thread-time').should('contain', '2 hours ago')
    })

    it('displays thread tags', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.thread-tag').should('contain', 'announcement')
      cy.get('.thread-item').eq(1).find('.thread-tag').should('contain', 'discussion')
      cy.get('.thread-item').eq(2).find('.thread-tag').should('contain', 'help')
    })
  })

  describe('Thread status badges', () => {
    it('shows pinned badge for pinned threads', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.pinned-badge').should('be.visible')
      cy.get('.thread-item').eq(0).find('.pinned-badge i').should('have.class', 'fa-thumbtack')
      cy.get('.thread-item').eq(1).find('.pinned-badge').should('not.exist')
    })

    it('shows locked badge for locked threads', () => {
      mountWithProps()
      cy.get('.thread-item').eq(1).find('.locked-badge').should('be.visible')
      cy.get('.thread-item').eq(1).find('.locked-badge i').should('have.class', 'fa-lock')
      cy.get('.thread-item').eq(0).find('.locked-badge').should('not.exist')
    })

    it('shows both badges when thread is pinned and locked', () => {
      const threadWithBoth = {
        ...mockCategory.threads[0],
        isPinned: true,
        isLocked: true
      }
      mountWithProps({ threads: [threadWithBoth] })
      
      cy.get('.pinned-badge').should('be.visible')
      cy.get('.locked-badge').should('be.visible')
    })

    it('shows no badges for regular threads', () => {
      mountWithProps()
      cy.get('.thread-item').eq(2).find('.pinned-badge').should('not.exist')
      cy.get('.thread-item').eq(2).find('.locked-badge').should('not.exist')
    })
  })

  describe('Thread statistics', () => {
    it('displays view count', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.stat-item .fa-eye + .stat-count').should('contain', '42')
      cy.get('.thread-item').eq(1).find('.stat-item .fa-eye + .stat-count').should('contain', '23')
    })

    it('displays reply count', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.stat-item .fa-comment + .stat-count').should('contain', '5')
      cy.get('.thread-item').eq(1).find('.stat-item .fa-comment + .stat-count').should('contain', '3')
    })

    it('displays zero for missing counts', () => {
      mountWithProps()
      cy.get('.thread-item').eq(2).find('.stat-item .fa-eye + .stat-count').should('contain', '10')
      cy.get('.thread-item').eq(2).find('.stat-item .fa-comment + .stat-count').should('contain', '0')
    })

    it('shows last reply info when available', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.thread-last-reply').should('contain', 'Legutóbbi válasz: 2024-03-03 by User2')
      cy.get('.thread-item').eq(1).find('.thread-last-reply').should('contain', 'Legutóbbi válasz: 2024-03-02 by User3')
    })

    it('hides last reply info when not available', () => {
      mountWithProps()
      cy.get('.thread-item').eq(2).find('.thread-last-reply').should('not.exist')
    })
  })

  describe('Tag styling', () => {
    it('applies correct CSS class for each tag', () => {
      mountWithProps()
      cy.get('.thread-item').eq(0).find('.thread-tag').should('have.class', 'tag-announcement')
      cy.get('.thread-item').eq(1).find('.thread-tag').should('have.class', 'tag-discussion')
      cy.get('.thread-item').eq(2).find('.thread-tag').should('have.class', 'tag-help')
    })

    it('uses default class for unknown tags', () => {
      mountWithProps({
        threads: [{
          ...mockCategory.threads[0],
          tag: 'unknown-tag'
        }]
      })
      cy.get('.thread-tag').should('have.class', 'tag-default')
    })

    it('handles all defined tag types', () => {
      const allTags = ['announcement', 'discussion', 'design', 'help', 'tools']
      const threads = allTags.map((tag, index) => ({
        ...mockCategory.threads[0],
        id: index,
        tag: tag
      }))
      
      mountWithProps({ threads })
      
      allTags.forEach(tag => {
        cy.get(`.thread-tag.tag-${tag}`).should('exist')
      })
    })
  })

  describe('Navigation', () => {
    it('navigates to thread when clicked', () => {
      mountWithProps()
      
      cy.get('@router').then((router) => {
        cy.spy(router, 'push').as('routerPush')
      })
      
      cy.get('.thread-item').eq(1).click()
      
      cy.get('@routerPush').should('have.been.calledWith', '/thread/102')
    })

    it('navigates to correct thread for each item', () => {
      mountWithProps()
      
      cy.get('@router').then((router) => {
        cy.spy(router, 'push').as('routerPush')
      })
      
      cy.get('.thread-item').eq(0).click()
      cy.get('@routerPush').should('have.been.calledWith', '/thread/101')
      
      cy.get('.thread-item').eq(2).click()
      cy.get('@routerPush').should('have.been.calledWith', '/thread/103')
    })
  })

  describe('Hover effects', () => {
    it('has hover effect on category', () => {
      mountWithProps()
      cy.get('.forum-category').trigger('mouseover')
    })

    it('has hover effect on thread items', () => {
      mountWithProps()
      cy.get('.thread-item').first().trigger('mouseover')
      cy.get('.thread-item').first().should('have.css', 'cursor', 'pointer')
    })
  })

  describe('Empty state', () => {
    it('renders category with no threads', () => {
      mountWithProps({ threads: [] })
      cy.get('.threads-list').should('exist')
      cy.get('.thread-item').should('have.length', 0)
      cy.get('.thread-count').should('contain', '0 Téma')
    })
  })

  describe('Edge cases', () => {
    it('handles missing optional thread fields', () => {
      const minimalThread = {
        id: 999,
        title: 'Minimal Thread',
        author: 'Minimal User',
        timeAgo: 'just now',
        tag: 'discussion'
      }
      
      mountWithProps({ threads: [minimalThread] })
      
      cy.get('.thread-title').should('contain', 'Minimal Thread')
      cy.get('.stat-item .fa-eye + .stat-count').should('contain', '0')
      cy.get('.stat-item .fa-comment + .stat-count').should('contain', '0')
      cy.get('.pinned-badge').should('not.exist')
      cy.get('.locked-badge').should('not.exist')
      cy.get('.thread-last-reply').should('not.exist')
    })

    it('handles very long thread titles', () => {
      const longTitle = 'A'.repeat(100)
      mountWithProps({
        threads: [{
          ...mockCategory.threads[0],
          title: longTitle
        }]
      })
      
      cy.get('.thread-title').should('contain', longTitle)
    })
  })

  describe('Responsive design', () => {
    it('adjusts layout on mobile', () => {
      cy.viewport('iphone-x')
      mountWithProps()

      cy.get('.thread-item').first().should('be.visible')
      cy.get('.thread-details').should('be.visible')
    })
  })
})