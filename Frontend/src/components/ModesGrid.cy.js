import ModesGrid from './ModesGrid.vue'

describe('<ModesGrid />', () => {
  
  beforeEach(() => {
    cy.viewport(1200, 800)
  })

  it('renders all basic elements', () => {
    cy.mount(ModesGrid)
    
    cy.contains('.section-title', 'Játékmódok').should('be.visible')
    cy.get('#mod-area').should('be.visible')
    cy.get('.fedo').should('be.visible')
    cy.get('.doboz-area').should('be.visible')
    cy.get('.doboz').should('have.length', 3)
  })

  it('displays correct content for each mode', () => {
    cy.mount(ModesGrid)
    
    const expectedModes = [
      { title: 'DeathMatch', hasDesc: true },
      { title: 'Search and Destroy', hasDesc: false },
      { title: 'DeathMatch+', hasDesc: false }
    ]
    
    cy.get('.doboz').each(($doboz, index) => {
      const mode = expectedModes[index]
      
      cy.wrap($doboz).within(() => {
        cy.get('img').should('have.attr', 'src')
        cy.get('h3').should('contain', mode.title)
        
        if (mode.hasDesc) {
          cy.get('p').should('contain', 'A tipikus, klasszikus játékmód')
          cy.get('.p2').should('not.exist')
        } else {
          cy.get('.p2').should('contain', 'Coming Soon!')
          cy.get('p:not(.p2)').should('not.exist')
        }
      })
    })
  })

it('has hover effects on desktop', () => {
  cy.viewport(1920, 1080)
  cy.mount(ModesGrid)

  cy.get('.doboz', { timeout: 10000 }).should('have.length', 3)

  cy.get('.doboz').first().find('.overlay2')
    .should('have.css', 'opacity', '0')

  cy.get('.doboz').first().realHover()

  cy.wait(500)

  cy.get('.doboz').first().find('.overlay2')
    .should('have.css', 'opacity', '1')
})

  it('handles mobile viewport correctly', () => {
    cy.viewport('iphone-x')
    cy.mount(ModesGrid)

    cy.get('.doboz').first().within(() => {
      cy.get('.overlay2').should('be.visible')
      cy.get('h3').should('be.visible')
    })

    cy.get('.doboz').eq(1).within(() => {
      cy.get('.p2').should('be.visible')
    })
  })

  it('has all images with src attributes', () => {
    cy.mount(ModesGrid)

    cy.get('.doboz img').each(($img) => {
      cy.wrap($img)
        .should('be.visible')
        .and('have.attr', 'src')
        .and('include', '/src/assets/')  
    })
  })

  it('has correct structure', () => {
    cy.mount(ModesGrid)

    cy.get('#mod-area').should('exist')
    cy.get('.doboz-area').should('exist')
    cy.get('.overlay2').should('exist')
    cy.get('.p2').should('exist')
  })

  it('shows Coming Soon for modes without description', () => {
    cy.mount(ModesGrid)

    cy.get('.doboz').eq(1).within(() => {
      cy.get('h3').should('contain', 'Search and Destroy')
      cy.get('.p2').should('contain', 'Coming Soon!')
    })

    cy.get('.doboz').eq(2).within(() => {
      cy.get('h3').should('contain', 'DeathMatch+')
      cy.get('.p2').should('contain', 'Coming Soon!')
    })
  })
})