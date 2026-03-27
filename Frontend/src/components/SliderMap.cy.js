import SliderMap from './SliderMap.vue'

describe('<SliderMap />', () => {
  
  it('properly renders all basic elements', () => {
    cy.mount(SliderMap)

    cy.contains('.section-title', 'Pályáink').should('be.visible')

    cy.get('.slider-wrapper').should('be.visible')

    cy.get('.slide-track').should('exist')

    cy.get('.slide-item').should('have.length', 6)

    cy.get('.prev').should('be.visible')
    cy.get('.next').should('be.visible')
    cy.get('.slide-dots').should('be.visible')
    cy.get('.dot').should('have.length', 6)
  })
})

it('displays correct content for first slide', () => {
  cy.mount(SliderMap)

  cy.get('.slide-item.active').within(() => {
    cy.get('.name').should('contain', 'Sivatagi')
    cy.get('.des').should('contain', 'Taktikai a harc a sivatagi falu')
  })

  cy.get('.slide-item.active')
    .should('have.css', 'background-image')
    .and('include', '1.webp')
})

it('navigates to next slide when next button is clicked', () => {
  cy.mount(SliderMap)

  cy.get('.slide-item.active').find('.name').should('contain', 'Sivatagi')

  cy.get('.next').click()

  cy.get('.slide-item.active').find('.name').should('contain', 'Raktár')

  cy.get('.dot.active').should('have.length', 1)
})

it('navigates to previous slide when prev button is clicked', () => {
  cy.mount(SliderMap)

  cy.get('.next').click()
  cy.get('.slide-item.active').find('.name').should('contain', 'Raktár')

  cy.get('.prev').click()
  cy.get('.slide-item.active').find('.name').should('contain', 'Sivatagi')
})

it('navigates to specific slide when dot is clicked', () => {
  cy.mount(SliderMap)

  cy.get('.dot').eq(2).click()

  cy.get('.slide-item.active').find('.name').should('contain', 'Városi')

  cy.get('.dot.active').should('have.length', 1)
  cy.get('.dot').eq(2).should('have.class', 'active')
})

it('wraps around from last to first slide', () => {
  cy.mount(SliderMap)

  cy.get('.next').click() 
  cy.get('.next').click() 
  cy.get('.next').click() 
  cy.get('.next').click() 
  cy.get('.next').click() 
  
  cy.get('.slide-item.active').find('.name').should('contain', 'Építkezés')

  cy.get('.next').click()
  cy.get('.slide-item.active').find('.name').should('contain', 'Sivatagi')
})

it('wraps around from first to last slide when prev is clicked', () => {
  cy.mount(SliderMap)

  cy.get('.prev').click()
  cy.get('.slide-item.active').find('.name').should('contain', 'Építkezés')
})

it('automatically advances slides every 5 seconds', () => {
  cy.mount(SliderMap)

  cy.get('.slide-item.active').find('.name').should('contain', 'Sivatagi')

  cy.wait(5500)

  cy.get('.slide-item.active').find('.name').should('contain', 'Raktár')
})

it('pauses auto-slide on unmount', () => {
  cy.mount(SliderMap).then(({ wrapper }) => {
  })
})

it('highlights active dot correctly', () => {
  cy.mount(SliderMap)

  cy.get('.dot').first().should('have.class', 'active')
  cy.get('.dot').last().should('not.have.class', 'active')

  cy.get('.next').click()

  cy.get('.dot').eq(1).should('have.class', 'active')
  cy.get('.dot').first().should('not.have.class', 'active')
})

it('buttons are clickable and not disabled', () => {
  cy.mount(SliderMap)
  
  cy.get('.prev').should('not.be.disabled')
  cy.get('.next').should('not.be.disabled')
  cy.get('.dot').each(($dot) => {
    cy.wrap($dot).should('not.be.disabled')
  })
})

it('completes a full user journey through all slides', () => {
  cy.mount(SliderMap)
  
  const expectedSlides = ['Sivatagi', 'Raktár', 'Városi', 'Pályaudvar', 'Erdei', 'Építkezés']

  expectedSlides.forEach((slideName, index) => {
    cy.get('.slide-item.active').find('.name').should('contain', slideName)

    cy.get('.dot').eq(index).should('have.class', 'active')

    if (index < expectedSlides.length - 1) {
      cy.get('.next').click()
    }
  })
})