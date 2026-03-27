import EquipCards from './EquipCards.vue'

describe('<EquipCards />', () => {
  
  beforeEach(() => {
    cy.viewport(1920, 1080)
  })

  function hoverAndCheckBadge(index, shouldBeVisible = true) {
    cy.get('.box').eq(index).then(($box) => {
      cy.wrap($box).realHover({ position: 'center' })

      cy.wrap($box).find('.card-badge').should(($badge) => {
        if (shouldBeVisible) {
          expect(parseFloat($badge.css('opacity'))).to.be.greaterThan(0)
        } else {
          if ($badge.length > 0) {
            expect(parseFloat($badge.css('opacity'))).to.equal(0)
          }
        }
      })
    })
  }

  it('displays correct content for each card', () => {
  cy.mount(EquipCards)
  cy.get('.box', { timeout: 10000 }).should('have.length', 4)

  cy.get('.box').eq(0).realHover({ position: 'center' })
  cy.get('.box').eq(0).within(() => {
    cy.get('h3').should('contain', 'Fegyverek')
    cy.get('p').should('contain', 'Alap felszerelés')
    cy.get('.coming-soon').should('not.exist')
  })

  cy.get('.box').eq(1).realHover({ position: 'center' })
  cy.get('.box').eq(1).within(() => {
    cy.get('h3').should('contain', 'Fegyverek')
    cy.get('p').should('contain', 'Alap felszerelés')
    cy.get('.coming-soon').should('not.exist')
  })

  cy.get('.box').eq(2).realHover({ position: 'center' })
  cy.get('.box').eq(2).within(() => {
    cy.get('h3').should('be.empty')
    cy.get('p').should('not.exist')
    cy.get('.coming-soon').should('be.visible')
  })

  cy.get('.box').eq(3).realHover({ position: 'center' })
  cy.get('.box').eq(3).within(() => {
    cy.get('h3').should('be.empty')
    cy.get('p').should('not.exist')
    cy.get('.coming-soon').should('be.visible')
  })
})

it('shows badges on first two cards after hover', () => {
  cy.mount(EquipCards)
  cy.get('.box', { timeout: 10000 }).should('have.length', 4)

  cy.get('.box').eq(0).realHover({ position: 'center' })
  cy.get('.box').eq(0).find('.card-badge').should('be.visible')
  
  cy.get('.box').eq(1).realHover({ position: 'center' })
  cy.get('.box').eq(1).find('.card-badge').should('be.visible')

  cy.get('.box').eq(2).find('.card-badge').should('not.exist')
  cy.get('.box').eq(3).find('.card-badge').should('not.exist')
})

  it('shows Available badge only for first two cards', () => {
    cy.mount(EquipCards)
    cy.get('.box', { timeout: 10000 }).should('have.length', 4)

    cy.get('.box').eq(0).realHover({ position: 'center' })
    cy.get('.box').eq(0).find('.card-badge').should('be.visible')

    cy.get('.box').eq(1).realHover({ position: 'center' })
    cy.get('.box').eq(1).find('.card-badge').should('be.visible')

    cy.get('.box').eq(2).realHover({ position: 'center' })
    cy.get('.box').eq(2).find('.card-badge').should('not.exist')

    cy.get('.box').eq(3).realHover({ position: 'center' })
    cy.get('.box').eq(3).find('.card-badge').should('not.exist')
  })

  it('shows Coming Soon only for last two cards', () => {
    cy.mount(EquipCards)
    cy.get('.box', { timeout: 10000 }).should('have.length', 4)

    cy.get('.box').eq(0).realHover()
    cy.get('.box').eq(0).find('.coming-soon').should('not.exist')
    
    cy.get('.box').eq(1).realHover()
    cy.get('.box').eq(1).find('.coming-soon').should('not.exist')

    cy.get('.box').eq(2).realHover()
    cy.get('.box').eq(2).find('.coming-soon').should('be.visible')
    
    cy.get('.box').eq(3).realHover()
    cy.get('.box').eq(3).find('.coming-soon').should('be.visible')
  })

  it('has hover effects on desktop', () => {
    cy.mount(EquipCards)
    cy.get('.box', { timeout: 10000 }).should('have.length', 4)
    
    cy.get('.box').first().find('.overlay').should('not.be.visible')
    cy.get('.box').first().realHover({ position: 'center' })
    cy.get('.box').first().find('.overlay').should('be.visible')
  })

  it('handles mobile viewport correctly', () => {
    cy.viewport('iphone-x')
    cy.mount(EquipCards)
    cy.get('.box', { timeout: 10000 }).should('have.length', 4)

    cy.get('.box').eq(0).find('.overlay').should('be.visible')
    cy.get('.box').eq(0).find('.card-badge').should('be.visible')
    cy.get('.box').eq(2).find('.coming-soon').should('be.visible')
  })

  it('loads all images correctly', () => {
    cy.mount(EquipCards)
    cy.get('.box img', { timeout: 10000 }).should('have.length', 4)
    cy.get('.box img').each(($img) => {
      cy.wrap($img)
        .should('be.visible')
        .and('have.attr', 'src')
        .and('include', '.webp')
    })
  })
})