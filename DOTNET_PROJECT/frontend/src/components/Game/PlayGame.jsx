import React, { useEffect, useState } from 'react';
import { useGame } from '../../context/GameContext';
import { SceneLayout } from '../scene/SceneLayout';
import { DialoguePanel } from '../DialoguePanel';
import { HUD } from '../Hudv2';
import { Button } from '../../ui/Button';
import { Card } from '../../ui/Card';
import { Text } from '../../ui/Text';
import { tokens } from '../../design/tokens';

export function PlayGame({ saveId, onBackToMenu }) {
  const {
    currentNode,
    currentDialogue,
    availableChoices,
    playerState,
    loading,
    error,
    getCurrentNode,
    getNextDialogue,
    makeChoice,
    goBack,
    nextNode,
    getPlayerState,
    clearError
  } = useGame();

  const [dialogueIndex, setDialogueIndex] = useState(0);
  const [showChoices, setShowChoices] = useState(false);

  useEffect(() => {
    if (saveId) {
      loadGameData();
    }
  }, [saveId]);

  const loadGameData = async () => {
    try {
      clearError();
      // Only load the current node for now
      // Player state will be loaded when needed
      await getCurrentNode(saveId);
    } catch (error) {
      console.error('Failed to load game data:', error);
    }
  };

  const handleChoice = async (choice) => {
    try {
      await makeChoice(saveId, choice.id);
      console.log(choice);
      setShowChoices(false);
      setDialogueIndex(0);
    } catch (error) {
        console.error('choices is: ', choice)
      console.error('Failed to make choice:', error);
    }
  };

  const handleNextDialogue = async () => {
    try {
      const dialogue = await getNextDialogue(saveId);
      if (dialogue) {
        setDialogueIndex((prev) => prev + 1);
      } else {
        setShowChoices(true);
      }
    } catch (error) {
      console.error('Failed to get next dialogue:', error);
    }
  };

  const handleGoBack = async () => {
    try {
      await goBack(saveId);
      setDialogueIndex(0);
      setShowChoices(false);
    } catch (error) {
      console.error('Failed to go back:', error);
    }
  };

  const handleGoForward = async () => {
    try {
      await nextNode(saveId);
      setDialogueIndex(0);
      setShowChoices(false);
    } catch (error) {
      console.error('Failed to go forward:', error);
    }
  };

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

  return (
    <div
      style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        paddingTop: '48px'
      }}
    >
      {/* HUD */}
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
            backgroundImage: `url(${
              currentNode.backgroundUrl || '/assets/bg/space-tunnel.png'
            })`,
            backgroundSize: 'cover',
            backgroundPosition: 'center'
          }}
        />

        {/* Dialogue Panel - Fixed at Bottom */}
        <div
          style={{
            position: 'absolute',
            bottom: '16px',
            left: '16px',
            right: '16px',
            backgroundColor: '#0a0f1a', // deep blue/teal, not rgba blur
            border: '2px solid #3ae6ff', // neon-ish cyan border
            padding: '12px 16px',
            display: 'flex',
            gap: '12px',
            imageRendering: 'pixelated',
            color: '#d8faff',
            boxShadow: '0 0 8px #003644, 0 0 2px #3ae6ff inset',
            alignItems: 'flex-start' // top-align portrait + text + button
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
              src={
                currentDialogue?.character?.imageUrl ||
                '/assets/char/hero.png'
              }
              alt="Character"
              style={{
                width: '100%',
                height: '100%',
                objectFit: 'cover',
                imageRendering: 'pixelated'
              }}
            />
          </div>

          {/* Text + Choices + Button wrapper as row: text grows, button stays to the right */}
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
                {currentDialogue?.text ||
                  currentNode.description ||
                  'Welcome to the adventure!'}
              </p>

              {/* Choices */}
              {showChoices && availableChoices?.length > 0 && (
                <div
                  style={{
                    marginTop: '12px',
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '8px'
                  }}
                >
                  {availableChoices.map((choice, index) => (
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
                        if (!loading) {
                          e.target.style.background = '#112032';
                        }
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
                    if (!loading) {
                      e.target.style.background = '#112032';
                    }
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
      </div>
    </div>
  );
}