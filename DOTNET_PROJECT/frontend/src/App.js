import React from 'react';
import './App.css';
import { tokens } from './design/tokens';
import { SceneLayout} from './components/scene/SceneLayout';
import { Card } from './ui/Card';
import { Text } from './ui/Text';
import { DialoguePanel } from './components/DialoguePanel';


export default function App() {
  //placeholder scene
  var text= 'you have to do something! you are running out of time.';
  var choices = [
    { id: 'bang', text: 'bang on glass' },
    { id: 'scream', text: 'scream' },
    { id: 'look', text: 'look around' },
  ];

  function handleChoice(c) {
    console.log('choice: ', c);
  }
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
        <DialoguePanel
        avatarUrl="/assets/char/hero.png"
        text={text}
        choices={choices}
        onSelect={handleChoice}
        />
        </SceneLayout>
      </div>
  );
}