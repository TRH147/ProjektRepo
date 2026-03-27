describe('Teljes felhasználói út E2E teszt', () => {

  const testUser = {
    username: `teszt_${Date.now()}`,
    email: `teszt${Date.now()}@example.com`,
    password: 'Test123456'
  }
  
  const testThread = {
    title: `Teszt téma ${Date.now()}`,
    content: 'Ez egy teszt téma tartalma az E2E tesztből.'
  }

  const newPassword = 'NewPass789!'
  const friendUsername = 'Meow'

  beforeEach(() => {
    cy.visit('/')
  })

  it('Teljes felhasználói út - regisztráció, bejelentkezés, funkciók tesztelése', () => {

    cy.log('📝 1. LÉPÉS: Regisztráció és bejelentkezés')

    cy.get('.btnLogin-popup').contains('Bejelentkezés').click()
    cy.get('.auth-container.active-popup', { timeout: 10000 }).should('be.visible')

    cy.contains('.register-link span', 'Regisztrálj').click()
    cy.get('.logreg-box.active', { timeout: 10000 }).should('exist')

    cy.get('.form-box.register input[type="text"]').type(testUser.username)
    cy.get('.form-box.register input[type="email"]').type(testUser.email)
    cy.get('.form-box.register input[type="password"]').type(testUser.password)
    cy.get('.form-box.register .btn').click()
    cy.wait(2000)

    cy.get('.form-box.login input[type="text"]').type(testUser.username)
    cy.get('.form-box.login input[type="password"]').type(testUser.password)
    cy.get('.form-box.login .btn').click()
    cy.wait(3000) 
    
    cy.get('.auth-container.active-popup', { timeout: 10000 }).should('not.exist')
    cy.get('.user-pic', { timeout: 10000 }).should('be.visible')
    cy.log('Regisztráció és bejelentkezés sikeres')

    cy.log(' 2. LÉPÉS: Update oldal tesztelése')
    cy.contains('.navbar a', 'Újdonságok').click()
    cy.url().should('include', '/update')
    cy.get('.tabs', { timeout: 15000 }).should('be.visible')

    cy.get('.tab.active', { timeout: 10000 }).should('contain', 'Hírek')
    cy.get('#news', { timeout: 10000 }).should('be.visible')
    cy.contains('.tab', 'Frissítések').click()
    cy.wait(1000)
    cy.get('.tab.active', { timeout: 10000 }).should('contain', 'Frissítések')
    cy.get('#updates', { timeout: 10000 }).should('be.visible')
    cy.contains('.tab', 'Hírek').click()
    cy.wait(1000)
    cy.log('Update oldal tab-ok működnek')

    cy.log(' 3. LÉPÉS: Statisztika oldal tesztelése')

    cy.intercept('GET', '**/api/UserStats/**').as('getStats')

    cy.contains('.navbar a', 'Statisztika').click()
    cy.url().should('include', '/statics')

    cy.wait('@getStats', { timeout: 30000 })

    cy.get('.table_body table', { timeout: 15000 }).should('be.visible')
    cy.get('table tbody tr', { timeout: 15000 }).should('have.length.at.least', 1)

    cy.get('table thead th').first().click()
    cy.wait(2000)

    cy.get('table tbody tr', { timeout: 10000 }).should('have.length.at.least', 1)

    cy.log(' Statisztika oldal betöltött')

    cy.log(' 4. LÉPÉS: Fórum és téma létrehozás')
    cy.contains('.navbar a', 'Fórum').click()
    cy.url().should('include', '/forum')
    cy.get('.forum-header', { timeout: 20000 }).should('be.visible')
    
    cy.get('.new-thread-btn', { timeout: 10000 }).click()
    cy.url().should('include', '/new-thread')
    
    cy.get('#threadTitle', { timeout: 15000 }).type(testThread.title)
    cy.get('#threadContent').type(testThread.content)

    cy.wait(2000)
    cy.get('#threadCategory option', { timeout: 10000 }).should('have.length.gt', 1)
    cy.get('#threadCategory').select(1)
    
    cy.wait(1000)
    cy.get('#threadTag option', { timeout: 10000 }).should('have.length.gt', 1)
    cy.get('#threadTag').select(1)
    
    cy.get('button[type="submit"]').click()
    
    cy.get('.success-modal', { timeout: 15000 }).should('be.visible')
    cy.wait(2000)
    cy.get('.success-modal .forum-btn-secondary').click()
    cy.wait(1000)
    cy.log(' Téma létrehozva')

    cy.log('5. LÉPÉS: Vissza a főoldalra')
    cy.visit('/')
    cy.get('.user-pic', { timeout: 10000 }).should('be.visible')
    cy.wait(1000)

    cy.log('6. LÉPÉS: Profil oldal')
    cy.get('.user-pic', { timeout: 10000 }).click()
    cy.get('.user-menu.active', { timeout: 10000 }).should('be.visible')
    cy.contains('.user-menu a', 'Profilom', { timeout: 10000 }).click()
    cy.url().should('include', '/profile')
    cy.get('.profile-card', { timeout: 15000 }).should('be.visible')
    cy.wait(2000)

cy.log(' 7. LÉPÉS: Adatváltoztatás tesztelése')

cy.contains('.tab', 'Adatváltoztatás', { timeout: 10000 }).click()
cy.get('#adatvaltoztatas', { timeout: 10000 }).should('be.visible')
cy.wait(1000)

cy.log('   → Név változtatás tesztelése')
const newName = `${testUser.username}_updated`
cy.get('input#oldName', { timeout: 10000 }).type(testUser.username)
cy.get('input#newName', { timeout: 10000 }).type(newName)

cy.get('#adatvaltoztatas form').first().contains('button', 'Mentés').click()

cy.get('.notification-toast.success', { timeout: 10000 }).should('be.visible')
cy.wait(3000)

cy.log('   → Jelszó változtatás tesztelése')

cy.get('input#oldPass', { timeout: 10000 }).should('be.visible')
cy.get('input#newPass', { timeout: 10000 }).should('be.visible')

cy.get('.password-field .show-pass-btn').first().click()
cy.get('input#oldPass').should('have.attr', 'type', 'text')
cy.get('.password-field .show-pass-btn').first().click()
cy.get('input#oldPass').should('have.attr', 'type', 'password')

cy.get('input#oldPass').type(testUser.password)
cy.get('input#newPass').type(newPassword)

cy.get('#adatvaltoztatas form').eq(1).contains('button', 'Mentés').click()

cy.get('.notification-toast.success', { timeout: 10000 }).should('be.visible')
cy.wait(3000)

cy.log(' Név és jelszó változtatás sikeres')

    cy.log(' 8. LÉPÉS: Profilkép feltöltés')
    cy.contains('.tab', 'Profilkép', { timeout: 10000 }).click()
    cy.get('#profilkep', { timeout: 10000 }).should('be.visible')
    cy.get('input[type="file"]', { timeout: 10000 }).should('exist')
    cy.contains('button', 'Feltöltés', { timeout: 10000 }).should('exist')
    cy.get('.profile-pic', { timeout: 10000 }).should('be.visible')
    cy.wait(1000)
    cy.log(' Profilkép feltöltés funkció elérhető')

    cy.log(' 9. LÉPÉS: Barátkérés küldés tesztelése')

    cy.contains('.tab', 'Barátok', { timeout: 10000 }).click()
    cy.get('#baratok', { timeout: 10000 }).should('be.visible')
    cy.wait(2000)

    cy.get('.friends-list', { timeout: 15000 }).should('exist')

    cy.log(`   → Barátkérés küldése ${friendUsername} részére`)
    cy.get('.add-friend input[type="text"]', { timeout: 10000 }).should('be.visible')
    cy.get('.add-friend input[type="text"]').clear().type(friendUsername)
    cy.get('.add-friend button').click()

    cy.wait(5000)

    cy.get('body').then($body => {
      if ($body.find('.notification-toast').length > 0) {

        cy.get('.notification-toast').first().then($toast => {
          const toastClass = $toast.attr('class')
          cy.log(`   → Notification típus: ${toastClass}`)
          
          if ($toast.hasClass('success')) {
            cy.log(' Barátkérés sikeresen elküldve')
          } else if ($toast.hasClass('error')) {

            cy.get('.notification-message').first().then($msg => {
              const errorMsg = $msg.text()
              cy.log(`   → Hibaüzenet: ${errorMsg}`)
              
              if (errorMsg.includes('már barátok') || errorMsg.includes('már létezik')) {
                cy.log(' Már barátok vagy már küldtek kérést - ez elfogadható')
              } else {
                cy.log(` Váratlan hiba: ${errorMsg}`)
              }
            })
          } else {
            cy.log(` Más típusú notification: ${toastClass}`)
          }
        })
      } else {
        cy.log(' Nem jelent meg notification')

      }
    })

    cy.wait(3000) 
    cy.log(' 10. LÉPÉS: Kijelentkezés')

    cy.get('.logout-btn', { timeout: 10000 }).click()
    cy.wait(2000)

    cy.url().should('eq', 'http://localhost:5173/')
    cy.get('.btnLogin-popup', { timeout: 10000 }).should('be.visible')
    
    cy.log(' Kijelentkezés sikeres')
  })
})