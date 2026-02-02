import React, { useState } from 'react';
import { TextField, Button } from '@mui/material';

interface UserFormProps {
  onSubmit: (name: string) => void;
}

const UserForm: React.FC<UserFormProps> = ({ onSubmit }) => {
  const [name, setName] = useState('');

  const handleSubmit = () => {
    if (!name) return;
    onSubmit(name);
    setName('');
  };

  return (
    <div style={{ margin: '20px 0' }}>
      <TextField
        label="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        variant="outlined"
      />
      <Button
        variant="contained"
        color="primary"
        onClick={handleSubmit}
        style={{ marginLeft: '10px' }}
      >
        Add User
      </Button>
    </div>
  );
};

export default UserForm;
