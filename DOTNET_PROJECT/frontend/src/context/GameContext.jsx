// GameContext.jsx
import React, { createContext, useContext, useReducer } from 'react';
import { auth, game, story } from '../endpoints/api';

const startState = {
    user: null,
    authenticated: false,
    currentSave: null,
    currentNode: null,
    availableChoices: [],
    playerState: null,
    visitedNodes: [],
    loading: false,
    error: null,
    dialogueIndex: 0,
    gameOver: false,
    allSaves: [], // store all previous saves
};

const ActionTypes = {
    // Auth
    LOGIN_START: 'LOGIN_START',
    LOGIN_SUCCESS: 'LOGIN_SUCCESS',
    LOGIN_ERROR: 'LOGIN_ERROR',
    REGISTER_START: 'REGISTER_START',
    REGISTER_SUCCESS: 'REGISTER_SUCCESS',
    REGISTER_ERROR: 'REGISTER_ERROR',
    LOGOUT: 'LOGOUT',

    // Game
    START_GAME: 'START_GAME',
    GAME_SUCCESS: 'GAME_SUCCESS',
    GAME_ERROR: 'GAME_ERROR',
    GET_ALL_SAVES: 'GET_ALL_SAVES',
    DELETE_SAVE_START: 'DELETE_SAVE_START',
    DELETE_SAVE_SUCCESS: 'DELETE_SAVE_SUCCESS',
    DELETE_SAVE_ERROR: 'DELETE_SAVE_ERROR',
    UPDATE_SAVE: 'UPDATE_SAVE',

    // Node
    NODE_START: 'NODE_START',
    NODE_SUCCESS: 'NODE_SUCCESS',
    NODE_ERROR: 'NODE_ERROR',

    // Choice
    CHOICE_START: 'CHOICE_START',
    CHOICE_SUCCESS: 'CHOICE_SUCCESS',
    CHOICE_ERROR: 'CHOICE_ERROR',

    // Dialogue
    DIALOGUE_START: 'DIALOGUE_START',
    DIALOGUE_SUCCESS: 'DIALOGUE_SUCCESS',
    DIALOGUE_ERROR: 'DIALOGUE_ERROR',

    // Navigation
    NAVIGATE_START: 'NAVIGATE_START',
    NAVIGATE_SUCCESS: 'NAVIGATE_SUCCESS',
    NAVIGATE_ERROR: 'NAVIGATE_ERROR',

    // UI
    SET_LOADING: 'SET_LOADING',
    SET_ERROR: 'SET_ERROR',
    CLEAR_ERROR: 'CLEAR_ERROR',
    SET_GAME_OVER: 'SET_GAME_OVER',
};

function gameReducer(state, action) {
    switch (action.type) {
        // Auth
        case ActionTypes.LOGIN_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.LOGIN_SUCCESS:
            return { ...state, loading: false, authenticated: true, user: action.payload, error: null };
        case ActionTypes.LOGIN_ERROR:
            return { ...state, loading: false, authenticated: false, error: action.payload };
        case ActionTypes.REGISTER_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.REGISTER_SUCCESS:
            return { ...state, loading: false, authenticated: true, user: action.payload, error: null };
        case ActionTypes.REGISTER_ERROR:
            return { ...state, loading: false, error: action.payload };
        case ActionTypes.LOGOUT:
            return { ...startState };

        // Game
        case ActionTypes.START_GAME:
            return { ...state, loading: true, error: null };
        case ActionTypes.GAME_SUCCESS:
            return { ...state, loading: false, error: null, currentSave: action.payload };
        case ActionTypes.GAME_ERROR:
            return { ...state, loading: false, error: action.payload };
        case ActionTypes.GET_ALL_SAVES:
            return { ...state, allSaves: action.payload || [] };
        case ActionTypes.DELETE_SAVE_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.DELETE_SAVE_SUCCESS:
            return { ...state, loading: false, allSaves: state.allSaves.filter(s => s.id !== action.payload) };
        case ActionTypes.DELETE_SAVE_ERROR:
            return { ...state, loading: false, error: action.payload };
        case ActionTypes.UPDATE_SAVE:
            return {
                ...state,
                allSaves: state.allSaves.map(s => s.id === action.payload.id ? action.payload : s)
            };

        // Node
        case ActionTypes.NODE_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.NODE_SUCCESS:
            return { ...state, loading: false, error: null, currentNode: action.payload, availableChoices: action.payload.choices || [] };
        case ActionTypes.NODE_ERROR:
            return { ...state, loading: false, error: action.payload };

        // Choice
        case ActionTypes.CHOICE_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.CHOICE_SUCCESS:
            return {
                ...state,
                loading: false,
                error: null,
                currentNode: action.payload.CurrentStoryNode || state.currentNode,
                availableChoices: action.payload.AvailableChoices || [],
                playerState: action.payload.PlayerCharacter || state.playerState,
            };
        case ActionTypes.CHOICE_ERROR:
            return { ...state, loading: false, error: action.payload };

        // Dialogue
        case ActionTypes.DIALOGUE_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.DIALOGUE_SUCCESS:
            return { ...state, loading: false, error: null, dialogueIndex: action.payload.index || 0 };
        case ActionTypes.DIALOGUE_ERROR:
            return { ...state, loading: false, error: action.payload };

        // Navigation
        case ActionTypes.NAVIGATE_START:
            return { ...state, loading: true, error: null };
        case ActionTypes.NAVIGATE_SUCCESS:
            return { ...state, loading: false, error: null, currentNode: action.payload, availableChoices: action.payload.choices || [] };
        case ActionTypes.NAVIGATE_ERROR:
            return { ...state, loading: false, error: action.payload };

        // UI
        case ActionTypes.SET_LOADING:
            return { ...state, loading: action.payload };
        case ActionTypes.SET_ERROR:
            return { ...state, error: action.payload };
        case ActionTypes.CLEAR_ERROR:
            return { ...state, error: null };
        case ActionTypes.SET_GAME_OVER:
            return { ...state, gameOver: action.payload };

        default:
            return state;
    }
}

const GameContext = createContext();

export function GameProvider({ children }) {
    const [state, dispatch] = useReducer(gameReducer, startState);

    // ---------------- AUTH ----------------
    const login = async (credentials) => {
        try {
            dispatch({ type: ActionTypes.LOGIN_START });
            const user = await auth.login(credentials);
            dispatch({ type: ActionTypes.LOGIN_SUCCESS, payload: user });
            return user;
        } catch (error) {
            dispatch({ type: ActionTypes.LOGIN_ERROR, payload: error.message || 'Login failed' });
            throw error;
        }
    };

    const register = async (userData) => {
        try {
            dispatch({ type: ActionTypes.REGISTER_START });
            const user = await auth.register(userData);
            dispatch({ type: ActionTypes.REGISTER_SUCCESS, payload: user });
            return user;
        } catch (error) {
            dispatch({ type: ActionTypes.REGISTER_ERROR, payload: error.message || 'Registration failed' });
            throw error;
        }
    };

    const logout = async () => {
        try {
            await auth.logout();
        } finally {
            dispatch({ type: ActionTypes.LOGOUT });
        }
    };

    // ---------------- GAME ----------------
    const startGame = async (gameData) => {
        try {
            dispatch({ type: ActionTypes.START_GAME });

            const save = await game.startGame(gameData);

            dispatch({ type: ActionTypes.GAME_SUCCESS, payload: save });

            // return the save.
            return save;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to start game';

            dispatch({ type: ActionTypes.GAME_ERROR, payload: errorMessage });

            //return the error.
            throw error;
        }
    };

    const loadGame = async (saveId) => {
        try {
            // send the load game start action to the reducer.
            dispatch({ type: ActionTypes.START_GAME });

            // get the save from the API.
            const save = await game.loadGame(saveId);

            // send the load game success action to the reducer.

            dispatch({ type: ActionTypes.GAME_SUCCESS, payload: save });
            // return the save.
            return save;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to load game';
            dispatch({ type: ActionTypes.GAME_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // get all saves and return the result.
    const getAllSaves = async (userId) => {
        try {
            dispatch({ type: ActionTypes.GET_ALL_SAVES });
            const saves = await game.getAllSaves(userId);
            dispatch({ type: ActionTypes.GAME_SUCCESS, payload: saves });
            // return the saves.
            return saves;
        }
        catch (error) {
            const errorMessage = error.response?.data || 'Failed to get all saves';
            dispatch({ type: ActionTypes.GAME_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    const getSaveById = async (saveId) => {
        try {
            const save = state.allSaves.find(s => s.id === saveId);
            if (!save) throw new Error('Save not found');
            return save;
        } catch (error) {
            dispatch({ type: ActionTypes.SET_ERROR, payload: error.message || 'Failed to get save' });
            return null;
        }
    };

    const deleteSave = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.DELETE_SAVE_START });
            await game.deleteSave(saveId); // API call
            dispatch({ type: ActionTypes.DELETE_SAVE_SUCCESS, payload: saveId });
        } catch (error) {
            dispatch({ type: ActionTypes.DELETE_SAVE_ERROR, payload: error.message || 'Failed to delete save' });
        }
    };

    const updateSave = async (saveData) => {
        try {
            await game.updateSave(saveData); // API call
            dispatch({ type: ActionTypes.UPDATE_SAVE, payload: saveData });
        } catch (error) {
            dispatch({ type: ActionTypes.SET_ERROR, payload: error.message || 'Failed to update save' });
        }
    };

    const makeChoice = async (saveId, choiceId) => {
        try {
            dispatch({ type: ActionTypes.CHOICE_START });
            const gameState = await story.makeChoice(saveId, choiceId);
            console.log(gameState);
            console.log("GameContext - MakeChoise - gamestate - playerchar", gameState.playerCharacter);
            console.log("GameContext - MakeChoise - gamestate - availableChoices", gameState.availableChoices);
            console.log("GameContext - MakeChoise - gamestate - currentStoryNode", gameState.currentStoryNode);

            dispatch({
                type: ActionTypes.CHOICE_SUCCESS,
                payload: {
                    CurrentStoryNode: gameState.currentStoryNode,
                    AvailableChoices: gameState.availableChoices,
                    PlayerCharacter: gameState.playerCharacter
                        ? { ...gameState.playerCharacter, health: gameState.playerCharacter.health }
                        : null,
                }
            });
            
            if (gameState.IsGameOver) {
                dispatch({ type: ActionTypes.SET_GAME_OVER, payload: true });
            }
            return gameState;
        } catch (error) {
            dispatch({ type: ActionTypes.CHOICE_ERROR, payload: error.message || 'Failed to make choice' });
            throw error;
        }
    };

    const getCurrentNode = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NODE_START });
            const node = await story.getCurrentNode(saveId);
            dispatch({ type: ActionTypes.NODE_SUCCESS, payload: node });
            return node;
        } catch (error) {
            dispatch({ type: ActionTypes.NODE_ERROR, payload: error.message || 'Failed to get current node' });
            throw error;
        }
    };

    const getNextDialogue = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.DIALOGUE_START });
            const dialogue = await story.getNextDialogue(saveId);
            dispatch({ type: ActionTypes.DIALOGUE_SUCCESS, payload: dialogue });
            return dialogue;
        } catch (error) {
            dispatch({ type: ActionTypes.DIALOGUE_ERROR, payload: error.message || 'Failed to get next dialogue' });
            throw error;
        }
    };

    const goBack = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NAVIGATE_START });
            const node = await story.goBack(saveId);
            dispatch({ type: ActionTypes.NAVIGATE_SUCCESS, payload: node });
            return node;
        } catch (error) {
            dispatch({ type: ActionTypes.NAVIGATE_ERROR, payload: error.message || 'Failed to go back' });
            throw error;
        }
    };

    const nextNode = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NAVIGATE_START });
            const node = await story.nextNode(saveId);
            dispatch({ type: ActionTypes.NAVIGATE_SUCCESS, payload: node });
            return node;
        } catch (error) {
            dispatch({ type: ActionTypes.NAVIGATE_ERROR, payload: error.message || 'Failed to go forward' });
            throw error;
        }
    };

    const setGameOver = (value) => {
        dispatch({ type: ActionTypes.SET_GAME_OVER, payload: value });
    };

    const setError = (err) => dispatch({ type: ActionTypes.SET_ERROR, payload: err });
    const clearError = () => dispatch({ type: ActionTypes.CLEAR_ERROR });
    const setLoading = (value) => dispatch({ type: ActionTypes.SET_LOADING, payload: value });

    const values = {
        ...state,
        login,
        register,
        logout,
        startGame,
        loadGame,
        getAllSaves,
        getSaveById,  // old function added
        deleteSave,
        updateSave,   // old function added
        makeChoice,
        getCurrentNode,
        getNextDialogue,
        goBack,
        nextNode,
        setGameOver,
        setError,
        clearError,
        setLoading,
    };

    return <GameContext.Provider value={values}>{children}</GameContext.Provider>;
}

export function useGame() {
    const context = useContext(GameContext);
    if (!context) throw new Error('useGame must be used within GameProvider');
    return context;
}

export default GameContext;
