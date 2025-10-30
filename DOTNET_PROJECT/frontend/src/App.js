import React, { useState } from 'react';
import './App.css';
import { GameProvider } from './context/GameContext';
import { AudioProvider } from './context/AudioContext';
import { Home } from './pages/Home';
import { Game } from './pages/Game';
import { tokens } from './design/tokens';

//set the appContent routes
function AppContent() {
    // set the start page as the home page.
    const [currentPage, setCurrentPage] = useState('home');

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

// Setup of the app with the GameProvider, AudioProvider, and AppContent
export default function App() {
    return (
        <GameProvider>
            <AudioProvider>
                <AppContent />
            </AudioProvider>
        </GameProvider>
    );
}