import React, { useState, useEffect } from 'react';
import './App.css';
import { GameProvider } from './context/GameContext';
import { Home } from './pages/Home';
import { Game } from './pages/Game';
//import { AccountManagement } from './pages/AccountManagement';
import { tokens } from './design/tokens';

//set the appContent routes
function AppContent() {
  // set the start page as the home page.
  const [currentPage, setCurrentPage] = useState(() => {

    // save the page the user is on to the local storage.
    const savedPage = localStorage.getItem('currentPage');
    return savedPage || 'home';
  });

  // if the page changes, save the new page to the local storage.
  useEffect(() => {
    localStorage.setItem('currentPage', currentPage);
  }, [currentPage]);

  // navigate to a different page
  const navigate = (page) => {
    setCurrentPage(page);
  };

  // render the current page based on the currentPage state.
  const renderPage = () => {
    switch (currentPage) {
      case 'home':
        return <Home onNavigate={navigate} />;
      case 'game':
        return <Game onNavigate={navigate} />;

      //case 'account':
        //return <Account onNavigate={navigate} />;
      default:
        return <Home onNavigate={navigate} />;
    }
  };

  return (
    <div style={{
      minHeight: '100vh',
      background: tokens.color.bg,
      color: tokens.color.text
    }}>
      {renderPage()}
    </div>
  );
}

// setup the app with the GameProvider and AppContent
export default function App() {
  return (
    <GameProvider>
      <AppContent />
    </GameProvider>
  );
}