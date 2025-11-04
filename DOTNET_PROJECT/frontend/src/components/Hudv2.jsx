import React from 'react';
import { Button } from '../ui/Button';
import { Text } from '../ui/Text';
import { tokens } from '../design/tokens';

// Pixel HP bar kept mostly as is—only cleaned up props
function PixelHPBar({ health = 0, maxHealth = 100 }) {
    const pct = Math.max(0, Math.min(100, Math.round((health / maxHealth) * 100)));

    const baseW = 64;
    const baseH = 8;
    const scale = 2.5;
    const step = 4;

    const fillColor =
        pct > 60 ? '#36FF7C' : pct > 30 ? '#FFD23F' : '#FF4D4D';
    console.log("Health passed in pixel method: ", health)

    return (
        <div
            aria-label="HP bar"
            title={`HP: ${health}/${maxHealth}`}
            style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}
        >
            <div
                style={{
                    width: baseW * scale,
                    height: baseH * scale,
                    imageRendering: 'pixelated',
                    boxShadow: '0 0 0 2px #00A2FF inset, 0 0 10px rgba(0,162,255,0.3)',
                    background: `repeating-linear-gradient(
            90deg,
            rgba(0,0,0,0.75) 0px,
            rgba(0,0,0,0.75) ${step * scale}px,
            rgba(0,0,0,0.85) ${step * scale}px,
            rgba(0,0,0,0.85) ${step * 2 * scale}px
          )`,
                    border: '1px solid #003B5C',
                    position: 'relative',
                    borderRadius: 2,
                    overflow: 'hidden',
                }}
            >
                <div
                    style={{
                        width: `${pct}%`,
                        height: '100%',
                        background: `repeating-linear-gradient(
              90deg,
              ${fillColor} 0px,
              ${fillColor} ${step * scale}px,
              ${fillColor} ${step * scale}px,
              ${fillColor} ${step * 2 * scale}px
            )`,
                        boxShadow: 'inset 0 0 6px rgba(255,255,255,0.12)',
                        transition: 'width 120ms steps(6, end)',
                    }}
                />
                <div
                    style={{
                        position: 'absolute',
                        left: 0, right: 0, top: 0,
                        height: Math.ceil(baseH * scale * 0.35),
                        background: 'linear-gradient(to bottom, rgba(255,255,255,0.15), rgba(255,255,255,0))',
                        pointerEvents: 'none',
                    }}
                />
            </div>

            <Text size={12} weight={700} style={{ color: '#9EE7FF', letterSpacing: 0.5 }}>
                {health}/{maxHealth}
            </Text>
        </div>
    );
}

export function HUD({ gameState, onBackToMenu }) {
    const health = gameState?.playerCharacter?.health ?? 100;
    const currentNodeTitle = gameState?.currentStoryNode?.title ?? '';
    const isGameOver = gameState?.isGameOver ?? health <= 0;

    
    return (
        <div
            style={{
                position: 'fixed',
                top: 0,
                left: 0,
                right: 0,
                zIndex: tokens.z.hud,
                display: 'flex',
                alignItems: 'center',
                background: 'rgba(0, 0, 0, 0.9)',
                borderBottom: `2px solid ${isGameOver ? '#FF0000' : '#00A2FF'}`,
                padding: '8px 16px',
                height: '48px',
                boxShadow: '0 2px 10px rgba(0, 0, 0, 0.5)',
            }}
        >
            {/* Left: HP Bar */}
            <div style={{ flex: '0 0 auto' }}>
                <PixelHPBar health={health} maxHealth={100} />
            </div>

            {/* Center: Node Title or GAME OVER */}
            <div
                style={{
                    position: 'absolute',
                    left: '50%',
                    transform: 'translateX(-50%)',
                    maxWidth: '60%',
                    whiteSpace: 'nowrap',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    textAlign: 'center',
                    pointerEvents: 'none',
                }}
            >
                <Text
                    size={14}
                    weight={700}
                    style={{
                        color: isGameOver ? '#FF4D4D' : '#00A2FF',
                        textShadow: isGameOver
                            ? '0 0 12px rgba(255,0,0,0.6)'
                            : '0 0 8px rgba(0,162,255,0.35)',
                    }}
                >
                    {isGameOver ? 'GAME OVER' : currentNodeTitle}
                </Text>
            </div>

            {/* Right: Menu */}
            <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 8 }}>
                <Button
                    onClick={onBackToMenu}
                    style={{
                        padding: '0px 16px',
                        fontSize: 20,
                        background: 'rgba(0, 162, 255, 0.2)',
                        border: `1px solid #00A2FF`,
                        color: '#FFFFFF',
                    }}
                >
                    Menu
                </Button>
            </div>
        </div>
    );
}