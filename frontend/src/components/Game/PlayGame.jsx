import React, { useEffect, useState } from 'react';
import { useGame } from '../../context/GameContext';
import { SceneLayout } from '../scene/SceneLayout';
import { DialoguePanel } from '../DialoguePanel';
import { HUD } from '../Hudv2';
import { Button } from '../../ui/Button';
import { Card } from '../../ui/Card';
import { Text } from '../../ui/Text';
import { tokens } from '../../design/tokens';
import airlockBackground from '../../assets/backgrounds/airlock.png';
import awakeningBackground from '../../assets/backgrounds/awakening.png';
import corridorBackground from '../../assets/backgrounds/corridor.png';
import encounterBackground from '../../assets/backgrounds/encounter.png';
import exposedWiresBackground from '../../assets/backgrounds/exposedWires.png';
import hallwayBackground from '../../assets/backgrounds/hallway.png';
import homePageBackground from '../../assets/backgrounds/homePage.png';
import medkitBackground from '../../assets/backgrounds/medkit.png';
import outsideCryopodBackground from '../../assets/backgrounds/outsideCryopod.png';
import releaseHatchBackground from '../../assets/backgrounds/releaseHatch.png';
import spaceTunnelBackground from '../../assets/backgrounds/space-tunnel.png';
import ventilationShaftBackground from '../../assets/backgrounds/ventilationShaft.png';
import afterLoginBackground from '../../assets/backgrounds/afterLogin.png';
import backgroundTexture from '../../assets/backgrounds/background.png';
import homePageHeroBackground from '../../assets/backgrounds/HomePageImage.png';
import loginBackground from '../../assets/backgrounds/LogInImage.png';
import planetBackground from '../../assets/backgrounds/planet.png';
import spaceshipBackground from '../../assets/backgrounds/spaceship.png';
import heroPortrait from '../../assets/characters/hero.png';
import dariusPortrait from '../../assets/characters/darius.png';
import irenePortrait from '../../assets/characters/irene.png';
import narratorPortrait from '../../assets/characters/narrator.png';
import protagonistPortrait from '../../assets/characters/protagonist.png';
import systemAiPortrait from '../../assets/characters/systemAI.png';

const backgroundAssets = {
  'assets/backgrounds/afterLogin.png': afterLoginBackground,
  'assets/backgrounds/airlock.png': airlockBackground,
  'assets/backgrounds/awakening.png': awakeningBackground,
  'assets/backgrounds/background.png': backgroundTexture,
  'assets/backgrounds/corridor.png': corridorBackground,
  'assets/backgrounds/encounter.png': encounterBackground,
  'assets/backgrounds/exposedWires.png': exposedWiresBackground,
  'assets/backgrounds/hallway.png': hallwayBackground,
  'assets/backgrounds/homePage.png': homePageBackground,
  'assets/backgrounds/HomePageImage.png': homePageHeroBackground,
  'assets/backgrounds/LogInImage.png': loginBackground,
  'assets/backgrounds/medkit.png': medkitBackground,
  'assets/backgrounds/outsideCryopod.png': outsideCryopodBackground,
  'assets/backgrounds/planet.png': planetBackground,
  'assets/backgrounds/releaseHatch.png': releaseHatchBackground,
  'assets/backgrounds/space-tunnel.png': spaceTunnelBackground,
  'assets/backgrounds/spaceship.png': spaceshipBackground,
  'assets/backgrounds/ventilationShaft.png': ventilationShaftBackground,
  'assets/bg/afterLogin.png': afterLoginBackground,
  'assets/bg/background.png': backgroundTexture,
  'assets/bg/homePage.png': homePageBackground,
  'assets/bg/planet.png': planetBackground,
  'assets/bg/space-tunnel.png': spaceTunnelBackground,
  'assets/bg/spaceship.png': spaceshipBackground,
  'images/backgrounds/airlock.png': airlockBackground,
  'images/backgrounds/awakening.png': awakeningBackground,
  'images/backgrounds/corridor.png': corridorBackground,
  'images/backgrounds/encounter.png': encounterBackground,
  'images/backgrounds/exposedWires.png': exposedWiresBackground,
  'images/backgrounds/hallway.png': hallwayBackground,
  'images/backgrounds/maintenenceCorridor.png': ventilationShaftBackground,
  'images/backgrounds/medkit.png': medkitBackground,
  'images/backgrounds/outsideCryopod.png': outsideCryopodBackground,
  'images/backgrounds/releaseHatch.png': releaseHatchBackground,
  'images/backgrounds/ventilationShaft.png': ventilationShaftBackground,
  'images/backgrounds/HomePageImage.png': homePageHeroBackground,
  'images/backgrounds/LogInImage.png': loginBackground
};

const characterAssets = {
  'assets/characters/darius.png': dariusPortrait,
  'assets/characters/hero.png': heroPortrait,
  'assets/characters/irene.png': irenePortrait,
  'assets/characters/narrator.png': narratorPortrait,
  'assets/characters/protagonist.png': protagonistPortrait,
  'assets/characters/systemAI.png': systemAiPortrait,
  'assets/char/hero.png': heroPortrait,
  'images/characters/darius.png': dariusPortrait,
  'images/characters/irene.png': irenePortrait,
  'images/characters/narrator.png': narratorPortrait,
  'images/characters/protagonist.png': protagonistPortrait,
  'images/characters/systemAI.png': systemAiPortrait
};

const fallbackBackground = spaceTunnelBackground;
const fallbackCharacter = heroPortrait;

function resolveBackgroundAsset(path) {
  if (!path) {
    return fallbackBackground;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const normalized = path.trim().replace(/\\/g, '/').replace(/^\/+/, '');
  return backgroundAssets[normalized] ?? fallbackBackground;
}

function resolveCharacterAsset(path) {
  if (!path) {
    return fallbackCharacter;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const normalized = path.trim().replace(/\\/g, '/').replace(/^\/+/, '');
  return characterAssets[normalized] ?? fallbackCharacter;
}

export function PlayGame({ saveId, onBackToMenu }) {
  const {
    currentNode,
    currentDialogue,
    availableChoices,
    playerState,
    loading,
    error,
    loadGame,
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
      const save = await loadGame(saveId);
      if (save?.playerCharacterId) {
        await getPlayerState(save.playerCharacterId);
      }
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

  const backgroundImage = resolveBackgroundAsset(
    currentNode?.backgroundUrl
  );
  const portraitImage = resolveCharacterAsset(
    currentDialogue?.character?.imageUrl
  );

  return (
    <div
      style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        paddingTop: '1px',
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
          height: 'calc(100vh - 48px)',
          position: 'relative',
          border: '2px solid #00A2FF',
          overflow: 'hidden',
          margin: '40px',
          boxSizing: 'border-box'
        }}
      >
        {/* Scene/Background */}
        <div
          style={{
            width: '100%',
            height: '100%',
            backgroundImage: `url(${backgroundImage})`,
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
              src={portraitImage}
              alt={currentDialogue?.character?.name || 'Character portrait'}
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
