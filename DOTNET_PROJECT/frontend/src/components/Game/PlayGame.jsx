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
      setShowChoices(false);
      setDialogueIndex(0);
    } catch (error) {
      console.error('Failed to make choice:', error);
    }
  };

  const handleNextDialogue = async () => {
    try {
      const dialogue = await getNextDialogue(saveId);
      if (dialogue) {
        setDialogueIndex(prev => prev + 1);
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
      <div style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}>
        <Text size={18} style={{ color: tokens.color.textMuted }}>
          Loading game...
        </Text>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: tokens.space.lg
      }}>
        <Card style={{
          padding: tokens.space.xl,
          background: tokens.color.surface,
          border: `1px solid ${tokens.color.danger}`,
          borderRadius: tokens.radius.lg
        }}>
          <Text size={16} style={{ color: tokens.color.danger, marginBottom: tokens.space.lg }}>
            Error loading game: {error}
          </Text>
          <Button onClick={loadGameData}>
            Retry
          </Button>
        </Card>
      </div>
    );
  }

  if (!currentNode) {
    return (
      <div style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center'
      }}>
        <Text size={18} style={{ color: tokens.color.textMuted }}>
          No game data available
        </Text>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', background: tokens.color.bg, paddingTop: '48px' }}>
      {/* HUD */}
      <HUD 
        playerState={playerState}
        currentNode={currentNode}
        onBackToMenu={onBackToMenu}
      />

      {/* Main Game Content */}
      <div style={{
        width: '100%',
        height: 'calc(100vh - 48px)',
        position: 'relative',
        border: '2px solid #00A2FF',
        borderRadius: '12px',
        overflow: 'hidden',
        margin: '16px',
        boxSizing: 'border-box'
      }}>
        {/* Scene/Background */}
        <div style={{
          width: '100%',
          height: '100%',
          backgroundImage: `url(${currentNode.backgroundUrl || "/assets/bg/space-tunnel.png"})`,
          backgroundSize: 'cover',
          backgroundPosition: 'center'
        }} />

        {/* Dialogue Panel - Fixed at Bottom */}
        <div style={{
          position: 'absolute',
          bottom: '20px',
          left: '20px',
          right: '20px',
          background: 'rgba(0, 0, 0, 0.9)',
          border: '2px solid #00A2FF',
          borderRadius: '12px',
          padding: '20px',
          display: 'flex',
          gap: '16px'
        }}>
          {/* Avatar */}
          <div style={{
            flex: '0 0 80px',
            height: '80px',
            borderRadius: '8px',
            overflow: 'hidden',
            border: '2px solid #00A2FF'
          }}>
            <img 
              src={currentDialogue?.character?.imageUrl || "/assets/char/hero.png"}
              alt="Character"
              style={{
                width: '100%',
                height: '100%',
                objectFit: 'cover'
              }}
            />
          </div>

          {/* Text and Choices */}
          <div style={{ flex: 1 }}>
            <p style={{
              color: '#FFFFFF',
              fontSize: '14px',
              lineHeight: '1.5',
              margin: 0,
              marginBottom: showChoices ? '12px' : '0'
            }}>
              {currentDialogue?.text || currentNode.description || "Welcome to the adventure!"}
            </p>

            {/* Choices */}
            {showChoices && availableChoices?.length > 0 && (
              <div style={{
                marginTop: '12px',
                display: 'flex',
                flexDirection: 'column',
                gap: '8px'
              }}>
                {availableChoices.map((choice, index) => (
                  <button
                    key={choice.id}
                    onClick={() => handleChoice(choice)}
                    disabled={loading}
                    style={{
                      padding: '8px 12px',
                      background: 'rgba(0, 162, 255, 0.2)',
                      border: '1px solid #00A2FF',
                      borderRadius: '8px',
                      color: '#FFFFFF',
                      fontSize: '14px',
                      cursor: loading ? 'not-allowed' : 'pointer',
                      textAlign: 'left',
                      transition: 'all 0.2s'
                    }}
                    onMouseEnter={(e) => {
                      if (!loading) {
                        e.target.style.background = 'rgba(0, 162, 255, 0.4)';
                      }
                    }}
                    onMouseLeave={(e) => {
                      e.target.style.background = 'rgba(0, 162, 255, 0.2)';
                    }}
                  >
                    {choice.text}
                  </button>
                ))}
              </div>
            )}

            {/* Next Button */}
            {!showChoices && (
              <button
                onClick={handleNextDialogue}
                disabled={loading}
                style={{
                  marginTop: '12px',
                  padding: '10px 20px',
                  background: '#00A2FF',
                  border: 'none',
                  borderRadius: '8px',
                  color: '#FFFFFF',
                  fontSize: '14px',
                  cursor: loading ? 'not-allowed' : 'pointer',
                  float: 'right'
                }}
              >
                {loading ? 'Loading...' : 'Next'}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}