
import 'cypress-real-events/support'
import './commands'

import { mount } from 'cypress/vue'

Cypress.Commands.add('mount', mount)