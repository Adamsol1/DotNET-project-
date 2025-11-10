import React, { useState, useEffect } from 'react';
import { useGame } from '../context/GameContext';
import { StartGame } from '../components/Game/NewGame';
import { PlayGame } from '../components/Game/PlayGame';
import { motion } from 'framer-motion';
import RockPaperScissors from '../components/miniGames/RockPaperScissors';

export function Game({ onNavigate }) {
    const { authenticated, user, getAllSaves } = useGame();
    const [currentSave, setCurrentSave] = useState(null);
    const [saves, setSaves] = useState([]);
    const [showGameStart, setShowGameStart] = useState(false);
    const [showMiniGame, setShowMiniGame] = useState(false);

    useEffect(() => {
        if (authenticated && user) {
            loadSaves();
        }
    }, [authenticated, user]);

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

    const handleGameStart = (gameSave) => {
        setCurrentSave(gameSave);
        setShowGameStart(false);
    };

    const handleBackToMenu = () => {
        setCurrentSave(null);
        setShowGameStart(false);
    };

    const handleLoadSave = (save) => {
        setCurrentSave(save);
    };

    const exitGame = () => {
        onNavigate('home');
    };

    const handleMiniGameWin = () => {
        console.log('Player won the mini-game!');
    };

    const handleMiniGameLose = () => {
        console.log('Player lost the mini-game!');
    };

    const handleMiniGameComplete = () => {
        console.log('Mini-game complete!');
        setShowMiniGame(false);
    };

    if (!authenticated) {
        onNavigate('home');
        return null;
    }

    if (showGameStart) {
        return <StartGame onGameStart={handleGameStart} onBack={handleBackToMenu} />;
    }

    if (currentSave && currentSave.id) {
        return (
            <PlayGame
                saveId={currentSave.id}
                onBackToMenu={handleBackToMenu}
            />
        );
    }

    // ✅ Main Menu View (fixed: removed duplicated return + duplicated layout)
    return (
        <div
            className="min-h-screen text-white font-mono relative overflow-hidden bg-cover bg-center bg-no-repeat"
            style={{
                backgroundImage: `url('/assets/bg/afterLogin.png')`,
            }}
        >
            {/* Mini-game Overlay */}
            {showMiniGame && (
                <div
                    style={{
                        position: 'fixed',
                        top: 0,
                        left: 0,
                        right: 0,
                        bottom: 0,
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        zIndex: 2000,
                    }}
                    onClick={(e) => {
                        if (e.target === e.currentTarget) {
                            setShowMiniGame(false);
                        }
                    }}
                >
                    <RockPaperScissors
                        onComplete={() => {
                            console.log('Mini-game completed, closing...');
                            setShowMiniGame(false);
                        }}
                        onWin={handleMiniGameWin}
                        onLose={handleMiniGameLose}
                    />
                </div>
            )}

            <div className="relative z-10 min-h-screen flex flex-col">
                {/* Header */}
                <header className="p-6 flex justify-between items-center">
                    <div className="w-10 h-10"></div>

                    <motion.button
                        whileHover={{ scale: 1.05 }}
                        whileTap={{ scale: 0.95 }}
                        onClick={exitGame}
                        className="px-4 py-2 bg-transparent text-white font-bold border-2 border-white hover:bg-white hover:text-black transition-colors"
                    >
                        HOME
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
                        <div className="text-center mb-12">
                            <motion.h2
                                initial={{ opacity: 0, y: -10 }}
                                animate={{ opacity: 1, y: 0 }}
                                transition={{ delay: 0.2 }}
                                className="text-4xl font-bold text-white mb-4 tracking-wider"
                                style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}
                            >
                                WELCOME {user?.username}!
                            </motion.h2>
                            <p className="text-xl text-gray-300 mb-8">
                                Ready to continue your adventure?
                            </p>
                        </div>

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

                        {!showMiniGame && (
                            <motion.div
                                initial={{ opacity: 0, y: 20 }}
                                animate={{ opacity: 1, y: 0 }}
                                transition={{ delay: 0.35 }}
                                className="mb-8"
                            >
                                <motion.button
                                    whileHover={{ scale: 1.05 }}
                                    whileTap={{ scale: 0.95 }}
                                    onClick={() => {
                                        console.log('Opening mini-game...');
                                        setShowMiniGame(true);
                                    }}
                                    className="w-full px-8 py-6 bg-blue-600 text-white font-bold text-2xl border-4 border-blue-400 hover:bg-blue-500 transition-colors"
                                    style={{ boxShadow: '8px 8px 0px rgba(0, 0, 0, 0.8)' }}
                                >
                                    TEST ROCK PAPER SCISSORS
                                </motion.button>
                            </motion.div>
                        )}

                        {saves.length > 0 && (
                            <motion.div
                                initial={{ opacity: 0, y: 20 }}
                                animate={{ opacity: 1, y: 0 }}
                                transition={{ delay: 0.4 }}
                                className="mb-8"
                            >
                                <h3
                                    className="text-2xl font-bold text-white mb-4 tracking-wide text-center"
                                    style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}
                                >
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
