import React, { useEffect, useState } from 'react';
import { useGame } from '../../context/GameContext';
import { useAudio } from '../../context/AudioContext';
import { SceneLayout } from '../scene/SceneLayout';
import { DialoguePanel } from '../DialoguePanel';
import { HUD } from '../Hudv2';
import { Button } from '../../ui/Button';
import { Card } from '../../ui/Card';
import { Text } from '../../ui/Text';
import { tokens } from '../../design/tokens';

export function PlayGame({ saveId, onBackToMenu }) {
    // NOTE: keep the names your app used originally (playerState etc.)
    const {
        currentNode,
        // currentDialogue is not sent from node directly; we'll compute from currentNode.dialogues and dialogueIndex
        availableChoices,
        playerState,
        loading,
        error,
        getCurrentNode,
        // getNextDialogue is not used because node includes Dialogues[]; we keep it if other code uses it
        getNextDialogue,
        makeChoice,
        goBack,
        nextNode,
        getPlayerState,
        clearError,
        gameOver, // optional flag from GameContext
    } = useGame();

    const {
        playBackgroundMusic,
        playAmbientSound,
        playChoiceAudio
    } = useAudio();

    // dialogue index for multi-line dialogues loaded in currentNode.Dialogues
    const [dialogueIndex, setDialogueIndex] = useState(0);
    const [showChoices, setShowChoices] = useState(false);

    // Load node when saveId changes
    useEffect(() => {
        if (saveId) {
            loadGameData();
        }
    }, [saveId]);

    // Play audio when node changes
    useEffect(() => {
        if (currentNode) {
            if (currentNode.backgroundMusicUrl) {
                playBackgroundMusic(currentNode.backgroundMusicUrl);
            }
            if (currentNode.ambientSoundUrl) {
                playAmbientSound(currentNode.ambientSoundUrl, true);
            }

            // reset local dialogue state for new node
            setDialogueIndex(0);
            // if node has no dialogues, show choices immediately
            const hasDialogues = (currentNode.dialogues && currentNode.dialogues.length > 0);
            setShowChoices(!hasDialogues);
        }
    }, [currentNode?.id, currentNode?.backgroundMusicUrl, currentNode?.ambientSoundUrl]);

    // load current node from backend
    const loadGameData = async () => {
        try {
            clearError();
            await getCurrentNode(saveId);
            setShowChoices(false);
            setDialogueIndex(0);
        } catch (err) {
            console.error('Failed to load game data:', err);
        }
    };

    // Resolve the current dialogue using currentNode.dialogues and dialogueIndex
    const dialogues = currentNode?.dialogues || [];
    const currentDialogue = dialogues.length > 0 ? dialogues[dialogueIndex] : null;

    // Try to resolve character image:
    // backend may include characters in node (charactersInScene / characters), or the dialogue might include expanded character object.
    const resolveCharacterImage = () => {
        // 1) If dialogue contains character object (legacy)
        if (currentDialogue && currentDialogue.character && currentDialogue.character.imageUrl) {
            return currentDialogue.character.imageUrl;
        }

        // 2) If currentNode has characters list (several DTO variants exist)
        const chars = currentNode?.charactersInScene || currentNode?.characters || currentNode?.charactersInScene;
        if (chars && currentDialogue) {
            const found = chars.find((c) => c.id === currentDialogue.characterId);
            if (found && (found.imageUrl || found.imageURL)) {
                return found.imageUrl || found.imageURL;
            }
        }

        // 3) Fallback: try a common field on dialogue (maybe backend serializes characterImageUrl)
        if (currentDialogue && (currentDialogue.characterImageUrl || currentDialogue.characterimageurl)) {
            return currentDialogue.characterImageUrl || currentDialogue.characterimageurl;
        }

        // 4) Default avatar
        return '/assets/characters/hero.png';
    };

    const characterImageUrl = resolveCharacterImage();

    // Handle making a choice
    const handleChoice = async (choice) => {
        try {
            // Play choice audio if present on the choice
            if (choice.audioUrl) playChoiceAudio(choice.audioUrl);

            // Call makeChoice - be tolerant of different response shapes.
            // Some backends return the next node directly; others may return a wrapper.
            const res = await makeChoice(saveId, choice.id);

            // After making a choice, we want to refresh the current node and playerState
            // to ensure health and available choices are in sync.
            await loadGameData();
            
            // Reset dialogue index
            setDialogueIndex(0);
            setShowChoices(false);

        } catch (err) {
            console.error('Failed to make choice:', err);
        }
    };

    // Next dialogue (advance through currentNode.dialogues)
    const handleNextDialogue = async () => {
        try {
            // If there is a next dialogue, advance index
            if (dialogues && dialogueIndex + 1 < dialogues.length) {
                setDialogueIndex((prev) => prev + 1);
            } else {
                // end of dialogues -> show choices
                setShowChoices(true);
            }
        } catch (err) {
            console.error('Failed to advance dialogue:', err);
        }
    };

    const handleGoBack = async () => {
        try {
            await goBack(saveId);
            setDialogueIndex(0);
            setShowChoices(false);
            await loadGameData();
        } catch (err) {
            console.error('Failed to go back:', err);
        }
    };

    const handleGoForward = async () => {
        try {
            await nextNode(saveId);
            setDialogueIndex(0);
            setShowChoices(false);
            await loadGameData();
        } catch (err) {
            console.error('Failed to go forward:', err);
        }
    };

    // Loading / error / empty safeguards (keeps original UX)
    if (loading && !currentNode) {
        return (
            <div
                style={{
                    minHeight: '100vh',
                    background: tokens.color.bg,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center'
                }}
            >
                <Text size={18} style={{ color: tokens.color.textMuted }}>
                    Loading game...
                </Text>
            </div>
        );
    }

    if (error) {
        return (
            <div
                style={{
                    minHeight: '100vh',
                    background: tokens.color.bg,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    padding: tokens.space.lg
                }}
            >
                <Card
                    style={{
                        padding: tokens.space.xl,
                        background: tokens.color.surface,
                        border: `1px solid ${tokens.color.danger}`,
                        borderRadius: tokens.radius.lg
                    }}
                >
                    <Text
                        size={16}
                        style={{
                            color: tokens.color.danger,
                            marginBottom: tokens.space.lg
                        }}
                    >
                        Error loading game: {error}
                    </Text>
                    <Button onClick={loadGameData}>Retry</Button>
                </Card>
            </div>
        );
    }

    if (!currentNode) {
        return (
            <div
                style={{
                    minHeight: '100vh',
                    background: tokens.color.bg,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center'
                }}
            >
                <Text size={18} style={{ color: tokens.color.textMuted }}>
                    No game data available
                </Text>
            </div>
        );
    }

    // compute whether the player is dead / game over (be defensive)
    const hp = playerState?.health ?? playerState?.hp ?? 100;
    console.log("Playerstate.health: ", playerState?.health)
    const isGameOver = (typeof gameOver === 'boolean') ? gameOver : hp <= 0;

    return (
        <div
            style={{
                minHeight: '100vh',
                background: tokens.color.bg,
                paddingTop: '48px'
            }}
        >
            {/* HUD uses playerState and currentNode like original code */}
            <HUD
                playerState={playerState}
                currentNode={currentNode}
                onBackToMenu={onBackToMenu}
            />

            {/* Main Game Content */}
            <div
                style={{
                    width: '100%',
                    height: 'calc(100vh - 48px)',
                    position: 'relative',
                    border: '2px solid #00A2FF',
                    borderRadius: '12px',
                    overflow: 'hidden',
                    margin: '16px',
                    boxSizing: 'border-box'
                }}
            >
                {/* Scene/Background */}
                <div
                    style={{
                        width: '100%',
                        height: '100%',
                        backgroundImage: `url(${currentNode.backgroundUrl || '/assets/bg/space-tunnel.png'})`,
                        backgroundSize: 'cover',
                        backgroundPosition: 'center'
                    }}
                />

                {/* Dialogue Panel - Fixed at Bottom */}
                {!isGameOver && (
                    <div
                        style={{
                            position: 'absolute',
                            bottom: '16px',
                            left: '16px',
                            right: '16px',
                            backgroundColor: '#0a0f1a',
                            border: '2px solid #3ae6ff',
                            padding: '12px 16px',
                            display: 'flex',
                            gap: '12px',
                            imageRendering: 'pixelated',
                            color: '#d8faff',
                            boxShadow: '0 0 8px #003644, 0 0 2px #3ae6ff inset',
                            alignItems: 'flex-start'
                        }}
                    >
                        {/* Avatar */}
                        <div
                            style={{
                                flex: '0 0 96px',
                                height: '96px',
                                border: '2px solid #3ae6ff',
                                backgroundColor: '#000',
                                overflow: 'hidden',
                                imageRendering: 'pixelated'
                            }}
                        >
                            <img
                                src={characterImageUrl}
                                alt="Character"
                                style={{
                                    width: '100%',
                                    height: '100%',
                                    objectFit: 'cover',
                                    imageRendering: 'pixelated'
                                }}
                            />
                        </div>

                        {/* Text + Choices + Button wrapper as row */}
                        <div
                            style={{
                                flex: 1,
                                display: 'flex',
                                flexDirection: 'row',
                                alignItems: 'flex-start'
                            }}
                        >
                            {/* Text + Choices column */}
                            <div style={{ flex: 1, paddingRight: '12px' }}>
                                <p
                                    style={{
                                        color: '#FFFFFF',
                                        lineHeight: '1.4',
                                        margin: 0,
                                        marginBottom: showChoices ? '12px' : '0',
                                        fontFamily: '"visitor1", monospace',
                                        textTransform: 'uppercase',
                                        letterSpacing: '0.05em'
                                    }}
                                >
                                    {currentDialogue?.text || currentNode.description || 'Welcome to the adventure!'}
                                </p>

                                {/* Choices */}
                                {showChoices && (availableChoices?.length > 0) && (
                                    <div
                                        style={{
                                            marginTop: '12px',
                                            display: 'flex',
                                            flexDirection: 'column',
                                            gap: '8px'
                                        }}
                                    >
                                        {availableChoices.map((choice) => (
                                            <button
                                                key={choice.id}
                                                onClick={() => handleChoice(choice)}
                                                disabled={loading}
                                                style={{
                                                    padding: '8px 12px',
                                                    background: '#0a0f1a',
                                                    border: '2px solid #3ae6ff',
                                                    borderRadius: '0px',
                                                    color: '#d8faff',
                                                    fontSize: '20px',
                                                    lineHeight: '1.4',
                                                    fontFamily: '"visitor1", monospace',
                                                    cursor: loading ? 'not-allowed' : 'pointer',
                                                    textAlign: 'left',
                                                    boxShadow:
                                                        '0 0 6px #003644, 0 0 2px #3ae6ff inset',
                                                    imageRendering: 'pixelated'
                                                }}
                                                onMouseEnter={(e) => {
                                                    if (!loading) e.target.style.background = '#112032';
                                                }}
                                                onMouseLeave={(e) => {
                                                    e.target.style.background = '#0a0f1a';
                                                }}
                                            >
                                                {choice.text}
                                            </button>
                                            
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Next button column */}
                            {!showChoices && (
                                <div
                                    style={{
                                        flex: '0 0 auto',
                                        display: 'flex'
                                    }}
                                >
                                    <button
                                        onClick={handleNextDialogue}
                                        disabled={loading}
                                        style={{
                                            padding: '8px 12px',
                                            background: '#0a0f1a',
                                            border: '2px solid #3ae6ff',
                                            borderRadius: '0px',
                                            color: '#d8faff',
                                            fontSize: '20px',
                                            lineHeight: '1.4',
                                            fontWeight: 'bold',
                                            fontFamily: '"visitor1", monospace',
                                            cursor: loading ? 'not-allowed' : 'pointer',
                                            boxShadow:
                                                '0 0 6px #003644, 0 0 2px #3ae6ff inset',
                                            imageRendering: 'pixelated',
                                            whiteSpace: 'nowrap'
                                        }}
                                        onMouseEnter={(e) => {
                                            if (!loading) e.target.style.background = '#112032';
                                        }}
                                        onMouseLeave={(e) => {
                                            e.target.style.background = '#0a0f1a';
                                        }}
                                    >
                                        {loading ? '...' : 'NEXT'}
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                )}

                {/* Game Over overlay */}
                {isGameOver && (
                    <div style={{
                        position: 'fixed',
                        top: 0,
                        left: 0,
                        right: 0,
                        bottom: 0,
                        background: 'rgba(0,0,0,0.95)',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'center',
                        alignItems: 'center',
                        zIndex: 9999,
                        color: '#FF4D4D',
                        fontFamily: '"visitor1", monospace',
                        textAlign: 'center',
                        padding: 24
                    }}>
                        <div style={{ fontSize: 36, letterSpacing: 2 }}>████ GAME OVER ████</div>
                        <div style={{ marginTop: 16, color: '#fff', fontSize: 18 }}>
                            Your journey ends here.
                        </div>
                        <div style={{ marginTop: 24 }}>
                            <Button onClick={onBackToMenu}>Return to Menu</Button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
