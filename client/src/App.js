import React from 'react';
import Timezones from './timezones/Timezones';
import Chat from './Chat/Chat';
import Dashboard from './Dashboard'

function App() {
  return (
    <div style={{ margin: '0 30%' }}>
      <Dashboard />
      <Chat />
      {/* <Timezones /> */}
    </div>
  );
}

export default App;
