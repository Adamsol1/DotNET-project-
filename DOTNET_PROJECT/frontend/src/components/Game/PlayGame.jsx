import React, { useEffect, useState } from 'react';
import { useGame } from '../../context/GameContext';
import { HUD } from '../Hudv2';
import { Button } from '../../ui/Button';
import { Text } from '../../ui/Text';
import { tokens } from '../../design/tokens';

export function PlayGame({ saveId, onBackToMenu }) {
    const { currentNode, playerCharacter, availableChoices, loading, error, getCurrentNode, makeChoice, clearError } = useGame();
    const [showChoices, setShowChoices] = useState(false);

    useEffect(() => {
        if (saveId) loadGame();
    }, [saveId]);

    const loadGame = async () => {
        try {
            clearError();
            await getCurrentNode(saveId);
            setShowChoices(false);
        } catch (err) {
            console.error(err);
        }
    };

    const handleChoice = async (choice) => {
        try {
            await makeChoice(saveId, choice.id);
            setShowChoices(false);
        } catch (err) {
            console.error(err);
        }
    };

    if (loading && !currentNode) return <Text>Loading game...</Text>;
    if (error) return <Text>Error: {error}</Text>;
    if (!currentNode) return <Text>No game data available</Text>;

    return (
        <div style={{ minHeight: '100vh', background: tokens.color.bg, paddingTop: '48px' }}>
            <HUD playerCharacter={playerCharacter} currentNode={currentNode} onBackToMenu={onBackToMenu} />

            {/* Scene Background */}
            <div style={{
                width: '100%', height: 'calc(100vh - 48px)',
                backgroundImage: `url(${currentNode.backgroundUrl || '/assets/bg/space-tunnel.png'})`,
                backgroundSize: 'cover', backgroundPosition: 'center'
            }} />

            {/* Dialogue & Choices */}
            {!playerCharacter?.health <= 0 && (
                <div style={{
                    position: 'absolute', bottom: '16px', left: '16px', right: '16px',
                    backgroundColor: '#0a0f1a', border: '2px solid #3ae6ff', padding: '12px 16px',
                    display: 'flex', gap: '12px', imageRendering: 'pixelated', color: '#d8faff'
                }}>
                    <div style={{ flex: 1 }}>
                        <p style={{ color: '#FFFFFF', fontFamily: '"visitor1", monospace', textTransform: 'uppercase' }}>
                            {currentNode.description}
                        </p>

                        {showChoices && availableChoices.map((c) => (
                            <Button key={c.id} onClick={() => handleChoice(c)}>{c.text}</Button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
}
