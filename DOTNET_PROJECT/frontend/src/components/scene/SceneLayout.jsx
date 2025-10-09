import React from 'react';
import { tokens } from '../../design/tokens';
import { Background } from './Background';

export function SceneLayout ({ backgroundUrl, children }) {
    return (
        <div style={{
            position: 'relative',
            width: '100%', height:'78vh',
            maxWidth:1200, margin: '0 auto',
            borderRadius: tokens.radius.lg,
            overflow: 'hidden',
            border: '1px solid ' + tokens.color.surfaceBorder,
            background: tokens.color.bg,
        }}
        >
            <Background imageUrl={backgroundUrl} />
            {children}
            </div>
        );
}