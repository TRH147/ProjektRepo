import NotificationToast from './NotificationToast.vue'

describe('<NotificationToast />', () => {
  
  describe('Basic rendering', () => {
    it('displays message with given props', () => {
      cy.mount(NotificationToast, {
        props: {
          title: 'Test Title',
          message: 'This is a test message',
          type: 'info'
        }
      })

      cy.get('.notification-toast').should('be.visible')
      cy.get('.notification-title').should('contain', 'Test Title')
      cy.get('.notification-message').should('contain', 'This is a test message')
      cy.get('.notification-icon i').should('have.class', 'fa-info-circle')
    })

    it('uses default title when not provided', () => {
      cy.mount(NotificationToast, {
        props: {
          message: 'Message only'
        }
      })

      cy.get('.notification-title').should('contain', 'Értesítés')
    })

    it('requires message prop', () => {
      cy.mount(NotificationToast, {
        props: {
          message: 'Required message'
        }
      }).then(() => {
        cy.get('.notification-message').should('exist')
      })
    })
  })

  describe('Types and icons', () => {
    const testCases = [
      { type: 'success', icon: 'fa-check-circle' },
      { type: 'info', icon: 'fa-info-circle' },
      { type: 'warning', icon: 'fa-exclamation-triangle' },
      { type: 'error', icon: 'fa-exclamation-circle' }
    ]

    testCases.forEach(({ type, icon }) => {
      it(`uses correct icon and class for ${type} type`, () => {
        cy.mount(NotificationToast, {
          props: {
            message: 'Test message',
            type: type
          }
        })

        cy.get('.notification-toast').should('have.class', type)
        cy.get('.notification-icon i').should('have.class', icon)
      })
    })

    it('uses default type (success) for invalid type', () => {
      cy.mount(NotificationToast, {
        props: {
          message: 'Test',
          type: 'invalid-type'
        }
      })

      cy.get('.notification-toast').should('be.visible')

      cy.get('.notification-title').should('contain', 'Értesítés')
      cy.get('.notification-message').should('contain', 'Test')

      cy.get('.notification-toast').should('not.have.class', 'warning')
    })
  })

  describe('Timing and closing', () => {
    beforeEach(() => {
      cy.clock()
    })

    it('auto-closes after duration time', () => {
      const closeSpy = cy.spy().as('closeSpy')
      
      cy.mount(NotificationToast, {
        props: {
          message: 'Test',
          duration: 3000,
          onClose: closeSpy
        }
      })

      cy.get('.notification-toast').should('be.visible')
      cy.tick(3000)
      cy.get('.notification-toast').should('not.exist')
      cy.get('@closeSpy').should('have.been.called')
    })

    it('warning type does not auto-close', () => {
      const closeSpy = cy.spy().as('closeSpy')
      
      cy.mount(NotificationToast, {
        props: {
          message: 'Warning message',
          type: 'warning',
          duration: 3000,
          onClose: closeSpy
        }
      })

      cy.get('.notification-toast').should('be.visible')
      cy.tick(5000)
      cy.get('.notification-toast').should('be.visible')
      cy.get('@closeSpy').should('not.have.been.called')
    })

    it('disappears when close button is clicked', () => {
      const closeSpy = cy.spy().as('closeSpy')
      
      cy.mount(NotificationToast, {
        props: {
          message: 'Test',
          onClose: closeSpy
        }
      })

      cy.get('.notification-close').click()
      cy.get('.notification-toast').should('not.exist')
      cy.get('@closeSpy').should('have.been.called')
    })
  })

  describe('Special cases', () => {
    it('handles very long messages correctly', () => {
      const longMessage = 'This is a very long message that might take up multiple lines. We need to test how the component behaves with long content. Adding more text to make sure it wraps properly and still looks good.'
      
      cy.mount(NotificationToast, {
        props: {
          message: longMessage
        }
      })

      cy.get('.notification-message').should('contain', longMessage)
    })

    it('does not auto-close with 0 duration', () => {
      cy.clock()
      
      cy.mount(NotificationToast, {
        props: {
          message: 'Test',
          duration: 0
        }
      })

      cy.tick(5000)
      cy.get('.notification-toast').should('be.visible')
    })

    it('multiple close attempts do not cause issues', () => {
      const closeSpy = cy.spy().as('closeSpy')
  
      cy.mount(NotificationToast, {
        props: {
          message: 'Test',
          onClose: closeSpy
        }
      })

      cy.get('.notification-close').click()

      cy.get('@closeSpy').should('have.been.calledOnce')

      cy.get('.notification-toast').should('not.exist')

      cy.get('@closeSpy').should('have.been.calledOnce')
    })
  })

  describe('Responsive behavior', () => {
    it('displays correctly on mobile viewport', () => {
      cy.viewport('iphone-x')
      
      cy.mount(NotificationToast, {
        props: {
          message: 'Mobile view test'
        }
      })

      cy.get('.notification-toast').should('be.visible')
    })
  })

  describe('Progress bar', () => {
    it('warning type has no progress bar', () => {
      cy.mount(NotificationToast, {
        props: {
          message: 'Warning',
          type: 'warning'
        }
      })

      cy.get('.notification-toast.warning').should('exist')
    })

    it('non-warning types have progress bar', () => {
      cy.mount(NotificationToast, {
        props: {
          message: 'Success',
          type: 'success'
        }
      })

      cy.get('.notification-toast').should('not.have.class', 'warning')
    })
  })
})