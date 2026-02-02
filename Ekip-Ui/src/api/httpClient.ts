import axios from 'axios';

const httpClient = axios.create({
  baseURL: 'https://localhost:17177',
  headers: {
    'Content-Type': 'application/json'
  }
});

export default httpClient;
