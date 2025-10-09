import React from 'react';
import { tokens } from '../design/tokens';

export function Card ({ style, children }) {
    return( 
        <div style={{
            background: tokens.color.surface,
            border: '1px solid ' + tokens.color.surfaceBorder,
            borderRadius: tokens.radius.xl,
            backdropFilter: 'blur(6px)',
            boxShadow: tokens.shadow.md,
            ...style
        }}>
            {children}
        </div>
    );
}