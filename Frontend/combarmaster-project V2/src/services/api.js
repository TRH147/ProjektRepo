import axios from 'axios'

const api = axios.create({
  baseURL: '/api', // proxy-t használunk
  headers: {
    'Content-Type': 'application/json'
  }
})

export default api