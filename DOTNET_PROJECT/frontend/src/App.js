import React from 'react';
import './App.css';
import { tokens } from './design/tokens';
import { SceneLayout} from './components/scene/SceneLayout';
import { Card } from './ui/Card';
import { Text } from './ui/Text';

export default function App() {
  return (
    <div
      style={{
        minHeight: '100vh',
        background: tokens.color.bg,
        color: tokens.color.text,
        padding: tokens.space.md,
      }}
      >
        <SceneLayout backgroundUrl="/assets/bg/space-tunnel.png">
        <Card
          style={{
            possition: 'absolute',
            bottom: 24,
            left: 24,
            right: 24,
            textAlign: 'center',
          }}
          >
            <Text size={18} weight={600}>
              You have to do something! You are running out of time.
            </Text>
          </Card>
        </SceneLayout>
      </div>
  )
}