import React, { useEffect, useState } from 'react';
import { getUsers } from '../services/userService';
import UserForm from '../components/UserForm';

interface User {
  id: number;
  name: string;
}

const Home: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    getUsers().then((data) => setUsers(data));
  }, []);

  const handleAddUser = (name: string) => {
    // اینجا می‌تونی POST بزنی به Web API
    setUsers([...users, { id: users.length + 1, name }]);
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Users from Web API</h1>
      <UserForm onSubmit={handleAddUser} />
      <ul>
        {users.map((user) => (
          <li key={user.id}>{user.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default Home;
