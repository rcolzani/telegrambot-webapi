import React from 'react';
import Routes from './routes';
import Chat from './pages/Chat';
import Dashboard from './pages/Dashboard'
import GlobalStyle from './styles/global'
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
  return (
    <>
      <Routes />
      <GlobalStyle />
      <ToastContainer />
    </>
  );
}

export default App;
