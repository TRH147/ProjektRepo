import HeroCover from './HeroCover.vue'

describe('<HeroCover />', () => {
  
  it('properly renders all elements', () => {
    cy.mount(HeroCover)

    cy.get('.container1').should('be.visible')

    cy.get('.my-cover')
      .should('be.visible')
      .and('have.attr', 'alt', 'banner')
      .and('have.prop', 'naturalWidth')
      .should('be.greaterThan', 0)

    cy.contains('h2', 'CombatMaster').should('be.visible')
    cy.contains('p', 'deathmatch-re épülő FPS játék').should('be.visible')

    cy.get('.download-btn')
      .should('be.visible')
      .and('contain', 'Letöltés')
  })

  it('has working download button', () => {
    const alertStub = cy.stub()
    cy.on('window:alert', alertStub)
    
    cy.mount(HeroCover)
    cy.get('.download-btn').click()

    cy.wrap(alertStub).should('be.calledWith', 'A játék letöltése hamarosan elindul!')
  })

  it('has all interactive elements', () => {
    cy.mount(HeroCover)

    cy.get('.overlay3').should('exist')

    cy.get('.btn-text').should('contain', 'Letöltés')
    cy.get('.btn-icon').should('exist')
    cy.get('.bx.bx-download').should('exist')
    cy.get('.btn-glow').should('exist')
  })

  it('responds to hover', () => {
    cy.mount(HeroCover)

    cy.get('.download-btn').trigger('mouseover')
    cy.get('.download-btn').should('have.class', 'download-btn')
  })
})