import React, { createContext, useContext, useRef, useState } from 'react';

const AudioContext = createContext();

export function AudioProvider({ children }) {
    const backgroundMusicRef = useRef(null);
    const ambientSoundRef = useRef(null);
    const dialogueAudioRef = useRef(null);
    const choiceAudioRef = useRef(null);

    const [currentBackgroundUrl, setCurrentBackgroundUrl] = useState(null);
    const [currentAmbientUrl, setCurrentAmbientUrl] = useState(null);
    const [isMuted, setIsMuted] = useState(false);

    const playBackgroundMusic = (url) => {
        // Don't change if same URL or no URL provided
        if (!url || url === currentBackgroundUrl) return;

        // Stop existing background music
        if (backgroundMusicRef.current) {
            backgroundMusicRef.current.pause();
            backgroundMusicRef.current = null;
        }

        // Play new background music
        backgroundMusicRef.current = new Audio(url);
        backgroundMusicRef.current.loop = true;
        backgroundMusicRef.current.volume = isMuted ? 0 : 0.3; // Lower volume for background
        backgroundMusicRef.current.play().catch(err => {
            console.error('Error playing background music:', err);
        });

        setCurrentBackgroundUrl(url);
    };

    const playAmbientSound = (url, loop = true) => {
        // Stop existing ambient sound
        if (ambientSoundRef.current) {
            ambientSoundRef.current.pause();
            ambientSoundRef.current = null;
        }

        if (!url) {
            setCurrentAmbientUrl(null);
            return;
        }

        // Don't replay if same URL and it's already playing
        if (url === currentAmbientUrl && ambientSoundRef.current && !ambientSoundRef.current.paused) {
            return;
        }

        // Play new ambient sound
        ambientSoundRef.current = new Audio(url);
        ambientSoundRef.current.loop = loop;
        ambientSoundRef.current.volume = isMuted ? 0 : 0.7;
        ambientSoundRef.current.play().catch(err => {
            console.error('Error playing ambient sound:', err);
        });

        setCurrentAmbientUrl(url);
    };

    const playDialogueAudio = (url) => {
        if (!url) return;

        // Stop existing dialogue audio
        if (dialogueAudioRef.current) {
            dialogueAudioRef.current.pause();
            dialogueAudioRef.current = null;
        }

        // Play dialogue audio (doesn't loop)
        dialogueAudioRef.current = new Audio(url);
        dialogueAudioRef.current.volume = isMuted ? 0 : 1.0;
        dialogueAudioRef.current.play().catch(err => {
            console.error('Error playing dialogue audio:', err);
        });
    };

    const playChoiceAudio = (url) => {
        if (!url) return;

        // Play choice audio (doesn't loop, can overlap)
        const audio = new Audio(url);
        audio.volume = isMuted ? 0 : 0.8;
        audio.play().catch(err => {
            console.error('Error playing choice audio:', err);
        });

        choiceAudioRef.current = audio;
    };

    const stopAllAudio = () => {
        if (backgroundMusicRef.current) {
            backgroundMusicRef.current.pause();
            backgroundMusicRef.current = null;
        }
        if (ambientSoundRef.current) {
            ambientSoundRef.current.pause();
            ambientSoundRef.current = null;
        }
        if (dialogueAudioRef.current) {
            dialogueAudioRef.current.pause();
            dialogueAudioRef.current = null;
        }
        if (choiceAudioRef.current) {
            choiceAudioRef.current.pause();
            choiceAudioRef.current = null;
        }

        setCurrentBackgroundUrl(null);
        setCurrentAmbientUrl(null);
    };

    const toggleMute = () => {
        const newMutedState = !isMuted;
        setIsMuted(newMutedState);

        // Update volume for all active audio
        if (backgroundMusicRef.current) {
            backgroundMusicRef.current.volume = newMutedState ? 0 : 0.3;
        }
        if (ambientSoundRef.current) {
            ambientSoundRef.current.volume = newMutedState ? 0 : 0.7;
        }
        if (dialogueAudioRef.current) {
            dialogueAudioRef.current.volume = newMutedState ? 0 : 1.0;
        }
    };

    const setBackgroundVolume = (volume) => {
        if (backgroundMusicRef.current && !isMuted) {
            backgroundMusicRef.current.volume = Math.max(0, Math.min(1, volume));
        }
    };

    const setAmbientVolume = (volume) => {
        if (ambientSoundRef.current && !isMuted) {
            ambientSoundRef.current.volume = Math.max(0, Math.min(1, volume));
        }
    };

    const values = {
        playBackgroundMusic,
        playAmbientSound,
        playDialogueAudio,
        playChoiceAudio,
        stopAllAudio,
        toggleMute,
        setBackgroundVolume,
        setAmbientVolume,
        currentBackgroundUrl,
        currentAmbientUrl,
        isMuted,
    };

    return (
        <AudioContext.Provider value={values}>
            {children}
        </AudioContext.Provider>
    );
}

export function useAudio() {
    const context = useContext(AudioContext);
    if (!context) {
        throw new Error('useAudio must be used within an AudioProvider');
    }
    return context;
}

export default AudioContext;