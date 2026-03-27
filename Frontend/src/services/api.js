import axios from 'axios'

const api = axios.create({
  baseURL: 'https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api'
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api