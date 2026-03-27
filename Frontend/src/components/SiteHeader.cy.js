import SiteHeader from './SiteHeader.vue'
import { createMemoryHistory, createRouter } from 'vue-router'
import { createTestingPinia } from '@pinia/testing'
import { useUserStore } from '../stores/user'

describe('<SiteHeader />', () => {
  
  const mountWithOptions = (options = {}) => {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
        { path: '/update', name: 'update', component: { template: '<div>Update</div>' } },
        { path: '/statics', name: 'statics', component: { template: '<div>Statics</div>' } },
        { path: '/forum', name: 'forum', component: { template: '<div>Forum</div>' } },
        { path: '/profile', name: 'profile', component: { template: '<div>Profile</div>' } }
      ]
    })

    const pinia = createTestingPinia({
      createSpy: cy.spy, 
      initialState: {
        user: { 
          user: options.isLoggedIn ? { 
            name: 'Teszt User',
            profileImages: null 
          } : null 
        }
      }
    })

    cy.viewport(1920, 1080)

    return cy.mount(SiteHeader, {
      global: {
        plugins: [router, pinia]
      }
    }).then(() => {
      return router.isReady()
    })
  }

  describe('Not logged in state', () => {
    beforeEach(() => {
      mountWithOptions({ isLoggedIn: false })
      cy.wait(100)
    })

    it('displays basic navigation links', () => {
      cy.contains('Főoldal').should('be.visible')
      cy.contains('Újdonságok').should('be.visible')
      cy.contains('Statisztika').should('not.exist')
      cy.contains('Fórum').should('not.exist')
    })

    it('displays login button', () => {
      cy.get('.btnLogin-popup')
        .should('be.visible')
        .and('contain', 'Bejelentkezés')
    })

    it('does not show user menu', () => {
      cy.get('.user-wrapper').should('not.exist')
      cy.get('.user-pic').should('not.exist')
    })

    it('logo is visible', () => {
      cy.get('.logo')
        .should('be.visible')
        .and('have.attr', 'src')
        .and('include', 'Logo.png')
    })
  })

  describe('Logged in state', () => {
    beforeEach(() => {
      mountWithOptions({ isLoggedIn: true })
      cy.wait(100)
    })

    it('displays protected links', () => {
      cy.contains('Statisztika').should('be.visible')
      cy.contains('Fórum').should('be.visible')
    })

    it('does not show login button', () => {
      cy.get('.btnLogin-popup').should('not.exist')
    })

    it('shows user profile picture', () => {
      cy.get('.user-pic')
        .should('be.visible')
        .and('have.attr', 'src') 
    })

    it('user menu is initially hidden', () => {
      cy.get('.user-menu').should('not.have.class', 'active')
    })

    it('opens user menu when clicking on profile picture', () => {
      cy.get('.user-pic').click()
      cy.get('.user-menu').should('have.class', 'active')
      cy.contains('Profilom').should('be.visible')
      cy.contains('Kijelentkezés').should('be.visible')
    })
  })

  describe('Router links', () => {
    beforeEach(() => {
      mountWithOptions({ isLoggedIn: false })
      cy.wait(100)
    })

    it('Főoldal link points to /', () => {
      cy.contains('Főoldal').should('have.attr', 'href', '/')
    })

    it('Újdonságok link points to /update', () => {
      cy.contains('Újdonságok').should('have.attr', 'href', '/update')
    })
  })

  describe('Hamburger menu (mobile view)', () => {
    beforeEach(() => {
      mountWithOptions({ isLoggedIn: false })
      cy.viewport('iphone-x')
      cy.wait(100)
    })

    it('hamburger button is visible on mobile', () => {
      cy.get('.hamburger').should('be.visible')
    })

    it('navbar is initially hidden on mobile', () => {
      cy.get('.navbar').should('not.have.class', 'active')
    })

    it('opens menu when clicking hamburger', () => {
      cy.get('.hamburger').click()
      cy.get('.hamburger').should('have.class', 'active')
      cy.get('.navbar').should('have.class', 'active')
      
      cy.contains('Főoldal').should('be.visible')
      cy.contains('Újdonságok').should('be.visible')
      cy.get('.btnLogin-popup').should('be.visible')
    })

    it('closes menu when clicking hamburger again', () => {
      cy.get('.hamburger').click()
      cy.get('.navbar').should('have.class', 'active')
      
      cy.get('.hamburger').click()
      cy.get('.navbar').should('not.have.class', 'active')
    })

    it('closes menu when clicking a link', () => {
      cy.get('.hamburger').click()
      cy.get('.navbar').should('have.class', 'active')
      
      cy.contains('Főoldal').click({ force: true })
      cy.get('.navbar').should('not.have.class', 'active')
    })
  })

  describe('Logged in state on mobile', () => {
    beforeEach(() => {
      mountWithOptions({ isLoggedIn: true })
      cy.viewport('iphone-x')
      cy.wait(100)
    })

    it('user menu works correctly on mobile', () => {
      cy.get('.hamburger').click()
      cy.get('.navbar').should('have.class', 'active')
      
      cy.get('.user-pic').should('be.visible')
      cy.get('.user-pic').click()
      
      cy.get('.user-menu').should('have.class', 'active')
      cy.get('.user-menu').should('have.class', 'mobile')
      cy.contains('Profilom').should('be.visible')
      cy.contains('Kijelentkezés').should('be.visible')
    })
  })

  describe('Store interactions', () => {
    it('login button calls store.openAuth function', () => {
      mountWithOptions({ isLoggedIn: false })
      cy.wait(100)

      cy.get('.btnLogin-popup').click()

      cy.window().then(() => {
        const store = useUserStore()
        expect(store.openAuth).to.have.been.calledWith('login')
      })
    })

    it('logout calls store.logout function', () => {
      mountWithOptions({ isLoggedIn: true })
      cy.wait(100)
      
      cy.get('.user-pic').click()
      cy.contains('Kijelentkezés').click()
      
      cy.window().then(() => {
        const store = useUserStore()
        expect(store.logout).to.have.been.called
      })
    })
  })
})