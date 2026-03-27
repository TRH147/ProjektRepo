import AuthModal from './AuthModal.vue'
import { createMemoryHistory, createRouter } from 'vue-router'
import { createTestingPinia } from '@pinia/testing'
import { useUserStore } from '../stores/user'

describe('<AuthModal />', () => {
  
  const mountWithOptions = (options = {}) => {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
        { path: '/forgot-password', name: 'forgot-password', component: { template: '<div>Forgot Password</div>' } },
        { path: '/admin', name: 'admin', component: { template: '<div>Admin</div>' } }
      ]
    })

    const pinia = createTestingPinia({
      createSpy: cy.spy,
      initialState: {
        user: { 
          isAuthModalOpen: options.isOpen ?? true,
          activeTab: options.activeTab || 'login'
        }
      }
    })

    cy.viewport(1920, 1080)

    return cy.mount(AuthModal, {
      global: {
        plugins: [router, pinia]
      }
    }).then(() => {
      return router.isReady()
    })
  }

  describe('Modal visibility', () => {
    it('is visible when store.isAuthModalOpen is true', () => {
      mountWithOptions({ isOpen: true })
      cy.get('.auth-container').should('have.class', 'active-popup')
    })

    it('is hidden when store.isAuthModalOpen is false', () => {
      mountWithOptions({ isOpen: false })
      cy.get('.auth-container').should('not.have.class', 'active-popup')
    })

    it('closes when close icon is clicked', () => {
      mountWithOptions({ isOpen: true })
      
      const store = useUserStore()
      cy.get('.icon-close').click()
      
      cy.wrap(store).its('closeAuth').should('have.been.called')
    })
  })

  describe('Tab switching', () => {
    it('shows login form by default', () => {
      mountWithOptions({ activeTab: 'login' })
      cy.get('.logreg-box').should('not.have.class', 'active')
      cy.get('.form-box.login').should('be.visible')
      cy.get('.form-box.register').should('not.be.visible')
    })

    it('switches to register form when register link is clicked', () => {
      mountWithOptions({ activeTab: 'login' })
      
      cy.contains('.register-link', 'Regisztrálj').click()
      
      const store = useUserStore()
      cy.wrap(store).its('activeTab').should('equal', 'register')
    })

    it('switches to login form when login link is clicked', () => {
      mountWithOptions({ activeTab: 'register' })
      
      cy.contains('.login-link', 'Bejelentkezés').click()
      
      const store = useUserStore()
      cy.wrap(store).its('activeTab').should('equal', 'login')
    })
  })

  describe('Login form', () => {
    beforeEach(() => {
      mountWithOptions({ activeTab: 'login' })
    })

    it('has all required fields', () => {
      cy.get('.form-box.login input[type="text"]').should('have.attr', 'required')
      cy.get('.form-box.login input[type="password"]').should('have.attr', 'required')
    })

    it('updates v-model when typing', () => {
      cy.get('.form-box.login input[type="text"]').type('testuser')
      cy.get('.form-box.login input[type="password"]').type('password123')
      
      cy.get('.form-box.login input[type="text"]').should('have.value', 'testuser')
      cy.get('.form-box.login input[type="password"]').should('have.value', 'password123')
    })

    it('calls store.login with correct credentials on submit', () => {
      const store = useUserStore()
      
      cy.get('.form-box.login input[type="text"]').type('testuser')
      cy.get('.form-box.login input[type="password"]').type('password123')
      cy.get('.form-box.login form').submit()
      
      cy.wrap(store).its('login').should('have.been.calledWith', {
        username: 'testuser',
        password: 'password123'
      })
    })

    it('clears form after successful login', () => {
      cy.window().then(() => {
        const store = useUserStore()
        store.login = cy.stub().resolves({ isAdmin: false })
      })
      
      cy.get('.form-box.login input[type="text"]').type('testuser')
      cy.get('.form-box.login input[type="password"]').type('password123')
      cy.get('.form-box.login form').submit()

      cy.wait(100)
      
      cy.get('.form-box.login input[type="text"]').should('have.value', '')
      cy.get('.form-box.login input[type="password"]').should('have.value', '')
    })
  })

  describe('Register form', () => {
    beforeEach(() => {
      mountWithOptions({ activeTab: 'register' })
    })

    it('has all required fields', () => {
      cy.get('.form-box.register input[type="text"]').should('have.attr', 'required')
      cy.get('.form-box.register input[type="email"]').should('have.attr', 'required')
      cy.get('.form-box.register input[type="password"]').should('have.attr', 'required')
    })

    it('updates v-model when typing', () => {
      cy.get('.form-box.register input[type="text"]').type('newuser')
      cy.get('.form-box.register input[type="email"]').type('test@example.com')
      cy.get('.form-box.register input[type="password"]').type('password123')
      
      cy.get('.form-box.register input[type="text"]').should('have.value', 'newuser')
      cy.get('.form-box.register input[type="email"]').should('have.value', 'test@example.com')
      cy.get('.form-box.register input[type="password"]').should('have.value', 'password123')
    })

    it('calls store.register with correct data on submit', () => {
      const store = useUserStore()
      
      cy.get('.form-box.register input[type="text"]').type('newuser')
      cy.get('.form-box.register input[type="email"]').type('test@example.com')
      cy.get('.form-box.register input[type="password"]').type('password123')
      cy.get('.form-box.register form').submit()
      
      cy.wrap(store).its('register').should('have.been.calledWith', {
        username: 'newuser',
        email: 'test@example.com',
        password: 'password123'
      })
    })

    it('clears form after successful registration', () => {
      cy.window().then(() => {
        const store = useUserStore()
        store.register = cy.stub().resolves()
      })
      
      cy.get('.form-box.register input[type="text"]').type('newuser')
      cy.get('.form-box.register input[type="email"]').type('test@example.com')
      cy.get('.form-box.register input[type="password"]').type('password123')
      cy.get('.form-box.register form').submit()
      
      cy.wait(100)
      
      cy.get('.form-box.register input[type="text"]').should('have.value', '')
      cy.get('.form-box.register input[type="email"]').should('have.value', '')
      cy.get('.form-box.register input[type="password"]').should('have.value', '')
    })
  })

  describe('Forgot password', () => {
    it('calls closeAuth when clicked', () => {
      mountWithOptions({ activeTab: 'login' })
      
      cy.contains('.remember-forgot', 'Elfelejtett Jelszó').click()
      
      const store = useUserStore()
      cy.wrap(store).its('closeAuth').should('have.been.called')
    })
  })

  describe('Logo and branding', () => {
    it('displays the logo correctly', () => {
      mountWithOptions({ isOpen: true })
      
      cy.get('.logo-be').should('contain', 'Combat')
      cy.get('.logo-be').should('contain', 'Master')
      cy.get('.logo-img')
        .should('be.visible')
        .and('have.attr', 'src')
        .and('include', 'Logo.png')
    })
  })
})