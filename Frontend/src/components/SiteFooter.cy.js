import SiteFooter from './SiteFooter.vue'
import { createMemoryHistory, createRouter } from 'vue-router'

describe('<SiteFooter />', () => {
  
  const mountWithRouter = () => {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
        { path: '/update', name: 'update', component: { template: '<div>Update</div>' } },
        { path: '/statics', name: 'statics', component: { template: '<div>Statics</div>' } },
        { path: '/forum', name: 'forum', component: { template: '<div>Forum</div>' } }
      ]
    })

    return cy.mount(SiteFooter, {
      global: {
        plugins: [router]
      }
    }).then(() => {
      return router.isReady()
    })
  }

  describe('Alap megjelenés', () => {
    beforeEach(() => {
      mountWithRouter()
      cy.wait(100)
    })

    it('megjeleníti a főcímet', () => {
      cy.contains('h3', 'Kövess minket közösségi médián').should('be.visible')
    })

    it('megjeleníti a social média ikonokat', () => {
      cy.get('.social-link').should('have.length', 3)

      cy.get('.social-link').eq(0).within(() => {
        cy.get('i').should('have.class', 'bxl-facebook-circle')
        cy.get('.social-tooltip').should('contain', 'Facebook')
      })

      cy.get('.social-link').eq(1).within(() => {
        cy.get('i').should('have.class', 'bxl-instagram-alt')
        cy.get('.social-tooltip').should('contain', 'Instagram')
      })

      cy.get('.social-link').eq(2).within(() => {
        cy.get('i').should('have.class', 'bxl-twitter')
        cy.get('.social-tooltip').should('contain', 'Twitter')
      })
    })

    it('megjeleníti a footer navigációt', () => {
      cy.get('.list li').should('have.length', 4)
      
      const expectedLinks = ['Főoldal', 'Újdonságok', 'Statisztika', 'Fórum']
      
      cy.get('.nav-link').each(($link, index) => {
        cy.wrap($link).should('contain', expectedLinks[index])
      })
    })

    it('megjeleníti a "Rólunk" szekciót', () => {
      cy.contains('h4', 'Rólunk').should('be.visible')
      cy.get('.copyright').should('contain', 'Csapatunk 3 junior fejlesztőből áll')
    })

    it('megjeleníti a copyright évet', () => {
      const currentYear = new Date().getFullYear().toString()
      cy.get('.copyright-year').should('contain', currentYear)
      cy.get('.copyright-year').should('contain', 'CombatMaster. Minden jog fenntartva')
    })
  })

  describe('Router linkek', () => {
    beforeEach(() => {
      mountWithRouter()
      cy.wait(100)
    })

    it('a Főoldal link a / útvonalra mutat', () => {
      cy.contains('.nav-link', 'Főoldal').should('have.attr', 'href', '/')
    })

    it('az Újdonságok link a /update útvonalra mutat', () => {
      cy.contains('.nav-link', 'Újdonságok').should('have.attr', 'href', '/update')
    })

    it('a Statisztika link a /statics útvonalra mutat', () => {
      cy.contains('.nav-link', 'Statisztika').should('have.attr', 'href', '/statics')
    })

    it('a Fórum link a /forum útvonalra mutat', () => {
      cy.contains('.nav-link', 'Fórum').should('have.attr', 'href', '/forum')
    })
  })

  describe('Social linkek', () => {
    beforeEach(() => {
      mountWithRouter()
      cy.wait(100)
    })

    it('a social linkek külső oldalra mutatnak (#)', () => {
      cy.get('.social-link').each(($link) => {
        cy.wrap($link).should('have.attr', 'href', '#')
      })
    })

    it('a social linkek rendelkeznek aria-label attribútummal', () => {
      cy.get('.social-link').eq(0).should('have.attr', 'aria-label', 'Facebook')
      cy.get('.social-link').eq(1).should('have.attr', 'aria-label', 'Instagram')
      cy.get('.social-link').eq(2).should('have.attr', 'aria-label', 'Twitter')
    })
  })

  describe('Reszponzív viselkedés', () => {
    beforeEach(() => {
      mountWithRouter()
    })

    it('desktop nézetben megfelelően jelenik meg', () => {
      cy.viewport(1920, 1080)
      cy.wait(100)
      
      cy.get('.list').should('be.visible')
      cy.get('.list li').should('have.length', 4)

      cy.get('.list').should('have.css', 'flex-direction', 'row')
    })

    it('mobil nézetben a navigáció függőleges lesz', () => {
      cy.viewport('iphone-x') 
      cy.wait(100)
      
      cy.get('.list').should('be.visible')

      cy.get('.list').should('have.css', 'flex-direction', 'column')
    })

    it('nagyon kis mobilon a social tooltipek eltűnnek', () => {
      cy.viewport(600, 800)
      cy.wait(100)

      cy.get('.social-tooltip').each(($tooltip) => {
        cy.wrap($tooltip).should('not.be.visible')
      })
    })
  })

  describe('scrollToTop funkció', () => {
    it('a Főoldal linkre kattintva meghívódik a scrollToTop', () => {
      mountWithRouter()
      cy.wait(100)

      cy.window().then((win) => {
        cy.spy(win, 'scrollTo').as('scrollToSpy')
        
        cy.contains('.nav-link', 'Főoldal').click()
        
        cy.get('@scrollToSpy').should('have.been.calledWith', {
          top: 0,
          behavior: 'smooth'
        })
      })
    })
  })

  describe('Évszám frissítés', () => {
    it('az aktuális év jelenik meg', () => {
      mountWithRouter()
      cy.wait(100)
      
      cy.window().then(() => {
        const currentYear = new Date().getFullYear().toString()
        cy.get('.copyright-year').should('contain', currentYear)
      })
    })
  })
})