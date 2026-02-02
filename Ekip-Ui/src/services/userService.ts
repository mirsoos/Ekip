import httpClient from '../api/httpClient';

export const getUsers = async () => {
  try {
    const response = await httpClient.get('/api/users'); // مسیر Web API
    return response.data;
  } catch (error) {
    console.error('Error fetching users:', error);
    return [];
  }
};
