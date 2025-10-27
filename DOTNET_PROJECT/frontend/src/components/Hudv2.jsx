import React from 'react';
import { Button } from '../ui/Button';
import { Text } from '../ui/Text';
import { tokens } from '../design/tokens';

export function HUD({ playerState, currentNode, onBackToMenu }) {
  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      zIndex: tokens.z.hud,
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      background: 'rgba(0, 0, 0, 0.9)',
      borderBottom: `2px solid #00A2FF`,
      padding: '8px 16px',
      height: '48px',
      boxShadow: '0 2px 10px rgba(0, 0, 0, 0.5)'
    }}>
      
      {/* Left: Story Title */}
      <div style={{ 
        flex: '0 0 auto',
        whiteSpace: 'nowrap',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        maxWidth: '200px'
      }}>
        {currentNode && (
          <Text size={14} weight={600} style={{ color: '#00A2FF' }}>
            {currentNode.title}
          </Text>
        )}
      </div>

      {/* Center: Menu Button */}
      <div style={{ 
        flex: '1',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center'
      }}>
        <Text size={14} weight={600} style={{ color: '#FFFFFF' }}>
          MENU
        </Text>
      </div>

      {/* Right: Navigation Buttons */}
      <div style={{ 
        flex: '0 0 auto',
        display: 'flex',
        gap: '8px',
        alignItems: 'center'
      }}>
        <Button
          onClick={onBackToMenu}
          style={{
            padding: '6px 16px',
            fontSize: 12,
            background: 'rgba(0, 162, 255, 0.2)',
            border: `1px solid #00A2FF`,
            color: '#FFFFFF',
          }}
        >
          Back
        </Button>
        <Button
          style={{
            padding: '6px 16px',
            fontSize: 12,
            background: 'rgba(0, 162, 255, 0.2)',
            border: `1px solid #00A2FF`,
            color: '#FFFFFF',
          }}
        >
          Forward
        </Button>
      </div>
    </div>
  );
}