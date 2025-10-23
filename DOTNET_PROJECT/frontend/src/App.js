import React from 'react';
import './App.css';
import { tokens } from './design/tokens';
import { SceneLayout} from './components/scene/SceneLayout';
import { Card } from './ui/Card';
import { Text } from './ui/Text';
import { DialoguePanel } from './components/DialoguePanel';

//root react component that sets up a single scene
export default function App() {
  //placeholder scene
  var text= 'you have to do something! you are running out of time.';
  var choices = [
    { id: 'bang', text: 'bang on glass' },
    { id: 'scream', text: 'scream' },
    { id: 'look', text: 'look around' },
  ];
  //function that is called when user clicks one of the buttons
  function handleChoice(c) {
    console.log('choice: ', c);
  }
  return (
    <div
      style={{
        minHeight: '100vh',
        background: tokens.color.bg, //gets the token background color
        color: tokens.color.text, //gets the token text color
        padding: tokens.space.md, //uses token padding
      }}
      >
        {/*Gets the background froom assets/bg folder*/}
        <SceneLayout backgroundUrl="/assets/bg/space-tunnel.png">
        {/*Dialogue panel contains the avatar, text and choice buttons*/}
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