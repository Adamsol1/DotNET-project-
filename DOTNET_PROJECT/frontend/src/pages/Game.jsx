import React, { useState, useEffect } from 'react';
import { useGame } from '../context/GameContext';
import { StartGame } from '../components/Game/NewGame';
import { PlayGame } from '../components/Game/PlayGame';
import { motion } from 'framer-motion';

export function Game({ onNavigate }) {
  const { authenticated, user, getAllSaves } = useGame();
  const [currentSave, setCurrentSave] = useState(null);
  const [saves, setSaves] = useState([]);
  const [showGameStart, setShowGameStart] = useState(false);

  // Load saves when component mounts
  useEffect(() => {
    if (authenticated && user) {
      loadSaves();
    }
  }, [authenticated, user]);

  // get saves that belongs to the logged in user and set the saves state.
  const loadSaves = async () => {
    try {
      const userSaves = await getAllSaves(user.id);
      setSaves(userSaves);
    } catch (error) {
      console.error('Failed to load saves:', error);
    }
  };

  const handleNewGame = () => {
    setShowGameStart(true);
  };

  // handle the game start event and set the current save state.
  const handleGameStart = (gameSave) => {
    setCurrentSave(gameSave);
    setShowGameStart(false);
  };

  // handle the back to menu event and set the current save state to null.
  const handleBackToMenu = () => {
    setCurrentSave(null);
    setShowGameStart(false);
  };

  // handle the load save event and set the current save state to the selected save.
  const handleLoadSave = (save) => {
    setCurrentSave(save);
  };

  // exit game event and navigate to the home page.
  const exitGame = () => {
    onNavigate('home');
  };

  // Redirect to login if not authenticated and return null.
  if (!authenticated) {
    onNavigate('home');
    return null;
  }

  // Show StartGame component if new game is requested
  if (showGameStart) {
    return <StartGame onGameStart={handleGameStart} onBack={handleBackToMenu} />;
  }

  //return the  game play component from the current save 
  // lets the user play the game.
  if (currentSave && currentSave.id) {
    return (
      <PlayGame 
        saveId={currentSave.id} 
        onBackToMenu={handleBackToMenu}
      />
    );
  }

  // Main menu view.
  return (
    <div 
      className="min-h-screen text-white font-mono relative overflow-hidden bg-cover bg-center bg-no-repeat"
      style={{
        backgroundImage: `url('/assets/bg/afterLogin.png')`,
      }}
    >
      <div className="relative z-10 min-h-screen flex flex-col">
        {/* Header */}
        <header className="p-6 flex justify-between items-center">
          <motion.h1
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
            className="text-3xl font-bold text-white tracking-wider"
            style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}
          >
            AFTER THE JUMP
          </motion.h1>
          
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={exitGame}
            className="px-4 py-2 bg-transparent text-white font-bold border-2 border-white hover:bg-white hover:text-black transition-colors"
          >
            EXIT
          </motion.button>
        </header>

        {/* Main Content */}
        <main className="flex-1 flex items-center justify-center px-6">
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.5 }}
            className="w-full max-w-2xl"
          >
            {/* Welcome Message */}
            <div className="text-center mb-12">
              <motion.h2
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.2 }}
                className="text-4xl font-bold text-white mb-4 tracking-wider"
                style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}
              >
                WELCOME BACK
              </motion.h2>
              <p className="text-xl text-gray-300 mb-8">
                {user?.username}
              </p>
            </div>

            {/* New Game Button - Top */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.3 }}
              className="mb-8"
            >
              <motion.button
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                onClick={handleNewGame}
                className="w-full px-8 py-6 bg-white text-black font-bold text-2xl border-4 border-black hover:bg-gray-200 transition-colors"
                style={{ boxShadow: '8px 8px 0px rgba(0, 0, 0, 0.8)' }}
              >
                NEW GAME
              </motion.button>
            </motion.div>

            {/* Load Previous Saves - Below */}
            {saves.length > 0 && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.4 }}
                className="mb-8"
              >
                <h3 className="text-2xl font-bold text-white mb-4 tracking-wide text-center" style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}>
                  CONTINUE ADVENTURE
                </h3>
                
                <div className="space-y-3">
                  {saves.map((save, index) => (
                    <motion.div
                      key={save.id}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      transition={{ delay: 0.5 + index * 0.1 }}
                    >
                      <motion.button
                        whileHover={{ scale: 1.02 }}
                        whileTap={{ scale: 0.98 }}
                        onClick={() => handleLoadSave(save)}
                        className="w-full px-6 py-4 bg-gray-200 text-black font-bold text-lg border-4 border-black hover:bg-white transition-colors flex justify-between items-center"
                        style={{ boxShadow: '6px 6px 0px rgba(0, 0, 0, 0.8)' }}
                      >
                        <div className="text-left">
                          <div className="text-xl font-bold">{save.saveName}</div>
                          <div className="text-sm text-gray-600">
                            {save.characterName ? `Character: ${save.characterName}` : 'Saved Game'}
                          </div>
                        </div>
                        <div className="text-right text-sm text-gray-500">
                          {new Date(save.lastUpdate).toLocaleDateString()}
                        </div>
                      </motion.button>
                    </motion.div>
                  ))}
                </div>
              </motion.div>
            )}
          </motion.div>
        </main>
      </div>
    </div>
  );
}