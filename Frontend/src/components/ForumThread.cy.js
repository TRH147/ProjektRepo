import ForumThread from './ForumThread.vue'
import { createMemoryHistory, createRouter } from 'vue-router'

describe('<ForumThread />', () => {
  
  const mountWithProps = (threadProps = {}) => {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/thread/:id', name: 'thread', component: { template: '<div>Thread Page</div>' } }
      ]
    })

    const defaultThread = {
      id: 1,
      title: 'Test Thread Title',
      author: 'TestUser',
      timestamp: '2024-03-03',
      tag: 'General',
      replies: 5,
      lastReply: '2024-03-03 by OtherUser'
    }

    return cy.mount(ForumThread, {
      props: {
        thread: { ...defaultThread, ...threadProps }
      },
      global: {
        plugins: [router]
      }
    }).then(() => {
      return router.isReady()
    }).then(() => {
      cy.wrap(router).as('router')
    })
  }

  describe('Basic rendering', () => {
    it('displays thread title correctly', () => {
      mountWithProps({ title: 'Custom Title' })
      cy.get('.thread-title').should('contain', 'Custom Title')
    })

    it('displays thread author and timestamp', () => {
      mountWithProps({ 
        author: 'JohnDoe',
        timestamp: '2024-03-03 10:30'
      })
      cy.get('.thread-meta').should('contain', 'Szerző: JohnDoe, 2024-03-03 10:30')
    })

    it('displays thread tag', () => {
      mountWithProps({ tag: 'Important' })
      cy.get('.thread-tag').should('contain', 'Important')
    })

    it('displays replies count correctly', () => {
      mountWithProps({ replies: 42 })
      cy.get('.replies-count').should('contain', '42 válaszok')
    })

    it('displays last reply info', () => {
      mountWithProps({ lastReply: '2024-03-03 by Jane' })
      cy.get('.thread-stats div').last().should('contain', 'Legutóbbi válasz: 2024-03-03 by Jane')
    })
  })

  describe('Replies indicator', () => {
    it('shows comment icon when replies > 0', () => {
      mountWithProps({ replies: 5 })
      cy.get('.new-indicator').should('exist')
      cy.get('.new-indicator i').should('have.class', 'fa-comment')
    })

    it('does not show comment icon when replies = 0', () => {
      mountWithProps({ replies: 0 })
      cy.get('.new-indicator').should('not.exist')
    })

    it('shows tooltip title on icon', () => {
      mountWithProps({ replies: 3 })
      cy.get('.new-indicator').should('have.attr', 'title', 'Has replies')
    })
  })

  describe('Click interaction', () => {
    it('navigates to correct thread page when clicked', () => {
      mountWithProps({ id: 123 })

      cy.get('@router').then((router) => {
        cy.spy(router, 'push').as('routerPush')
      })
      
      cy.get('.forum-thread').click()
      
      cy.get('@routerPush').should('have.been.calledWith', '/thread/123')
    })
  })

  describe('Edge cases', () => {
    it('handles missing optional fields gracefully', () => {
      mountWithProps({
        replies: undefined,
        lastReply: undefined
      })

      cy.get('.replies-count').should('contain', ' válaszok')
      cy.get('.thread-stats div').last().should('contain', 'Legutóbbi válasz:')
    })

    it('handles long thread titles', () => {
      const longTitle = 'A'.repeat(100)
      mountWithProps({ title: longTitle })
      
      cy.get('.thread-title').should('contain', longTitle)
    })

    it('handles special characters in author name', () => {
      mountWithProps({ author: 'User_123!@#' })
      cy.get('.thread-meta').should('contain', 'User_123!@#')
    })

    it('handles very large reply counts', () => {
      mountWithProps({ replies: 999999 })
      cy.get('.replies-count').should('contain', '999999 válaszok')
    })
  })

  describe('Thread tag styling', () => {
    it('displays tag with correct styling', () => {
      mountWithProps({ tag: 'Announcement' })
      cy.get('.thread-tag').should('be.visible')
    })
  })

  describe('Accessibility', () => {
    it('can be triggered with click (same as mouse interaction)', () => {
      mountWithProps({ id: 456 })
      
      cy.get('@router').then((router) => {
        cy.spy(router, 'push').as('routerPush')
      })

      cy.get('.forum-thread').click()
      
      cy.get('@routerPush').should('have.been.calledWith', '/thread/456')
    })
  })
})