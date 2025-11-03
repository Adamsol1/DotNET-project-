// react imports
import React, { createContext, useContext, useReducer, useEffect } from 'react';
// api imports
import { auth, game, story } from '../endpoints/api';

/**
 * This context file is used to manage the game state and actions that can be taken.
 * as well used to store game states, that the backend, can use to get data. 
 * 
 */

// this is the start / initial state, of the game before the user even has
// visited the application. 
const startState = {
    // user stae is null, and they arent logged inn.
    user: null,
    authenticated: false,

    // the game states, they are emtpy.
    currentSave: null,
    currentNode: null,
    // empty array,
    availableChoices: [],
    playerState: null,
    visitedNodes: [],

    // UI state.
    loading: false,
    error: null,
    dialogueIndex: 0,
};

/**
 * Action types are strings that describe commands that can be sent by the application.
 */

const ActionTypes = {
    //auth actions.
    LOGIN_START: 'LOGIN_START',
    LOGIN_SUCCESS: 'LOGIN_SUCCESS',
    LOGIN_ERROR: 'LOGIN_ERROR',
    REGISTER_START: 'REGISTER_START',
    REGISTER_SUCCESS: 'REGISTER_SUCCESS',
    REGISTER_ERROR: 'REGISTER_ERROR',
    LOGOUT_START: 'LOGOUT_START',
    LOGOUT_SUCCESS: 'LOGOUT_SUCCESS',
    LOGOUT_ERROR: 'LOGOUT_ERROR',
    LOGOUT: 'LOGOUT',

    // game actions.
    START_GAME: 'START_GAME',
    GAME_SUCCESS: 'GAME_SUCCESS',
    GAME_ERROR: 'GAME_ERROR',
    GET_ALL_SAVES: 'GET_ALL_SAVES',
    DELETE_SAVE_START: 'DELETE_SAVE_START',
    DELETE_SAVE_SUCCESS: 'DELETE_SAVE_SUCCESS',
    DELETE_SAVE_ERROR: 'DELETE_SAVE_ERROR',
    UPDATE_SAVE: 'UPDATE_SAVE',

    // story actions.
    NODE_START: 'NODE_START',
    NODE_SUCCESS: 'NODE_SUCCESS',
    NODE_ERROR: 'NODE_ERROR',

    //choice actions.
    CHOICE_START: 'CHOICE_START',
    CHOICE_SUCCESS: 'CHOICE_SUCCESS',
    CHOICE_ERROR: 'CHOICE_ERROR',
    SET_PLAYER_STATE: 'SET_PLAYER_STATE',


    // navigation actions.
    NAVIGATE_START: 'NAVIGATE_START',
    NAVIGATE_SUCCESS: 'NAVIGATE_SUCCESS',
    NAVIGATE_ERROR: 'NAVIGATE_ERROR',

    // dialogue actions.
    DIALOGUE_START: 'DIALOGUE_START',
    DIALOGUE_SUCCESS: 'DIALOGUE_SUCCESS',
    DIALOGUE_ERROR: 'DIALOGUE_ERROR',

    // UI actions.
    SET_LOADING: 'SET_LOADING',
    SET_ERROR: 'SET_ERROR',
    CLEAR_ERROR: 'CLEAR_ERROR',
};

/**
 * game reducer is a function that takes in the current state and actions.
 * and then returns a new state based on the action.
 */

function gameReducer(state, action) {
    //switch on the action type, to return a new state.
    switch (action.type) {
        // We copy the state we are in, and move to the next state.
        case ActionTypes.LOGIN_START:
            return { ...state, loading: true, error: null };
        
        // set the once loading is sucess, we set the user and authenticated state.
        case ActionTypes.LOGIN_SUCCESS:
            return {
                ...state, loading: false, authenticated: true, 
                user: action.payload, error: null,
            };
        
        // if the login fails, we set the error state.
        case ActionTypes.LOGIN_ERROR:
            return { ...state, loading: false, error: action.payload, authenticated: false };
        
        // if the user logs out, we reset all the states to the start state.
        case ActionTypes.LOGOUT:
            return { ...state, user: null, authenticated: false,
                currentSave: null, currentNode: null, availableChoices: [],
                playerState: null, visitedNodes: [],
            };
        
        // if the game starts, we set the current save state.
        case ActionTypes.START_GAME:
            return { ...state, loading: true, error: null };
        
        // if the game starts successfully, we set the current save state.
        case ActionTypes.GAME_SUCCESS:
            return { ...state, loading: false, error: null,
                currentSave: action.payload,
            };
        // if the game starts fails, we set the error state.
        case ActionTypes.GAME_ERROR:
            return { ...state, loading: false, error: action.payload };
        
        // if the node starts, we set the current node state.
        case ActionTypes.NODE_START:
            return { ...state, loading: true, error: null };
        
        // if the node starts successfully, we set the current node state.
        case ActionTypes.NODE_SUCCESS:
            return { ...state, loading: false, error: null,
                currentNode: action.payload, availableChoices: action.payload.choices || [],
            };
        // if the node starts fails, we set the error state.
        case ActionTypes.NODE_ERROR:
            return { ...state, loading: false, error: action.payload };
        
        // if the choice starts, we set the current choice state.
        case ActionTypes.CHOICE_START:
            return { ...state, loading: true, error: null };

        // if the choice starts successfully, we set the current choice state.
        case ActionTypes.CHOICE_SUCCESS:
            return { 
                ...state, 
                loading: false, 
                error: null,
                currentNode: action.payload,
                availableChoices: action.payload.choices || []
            };

        // if the choice starts fails, we set the error state.
        case ActionTypes.CHOICE_ERROR:
            return { ...state, loading: false, error: action.payload };
        
        // if the dialogue starts, we set the current dialogue state.
        case ActionTypes.DIALOGUE_START:
            return { ...state, loading: true, error: null };
        
        // if the dialogue starts successfully, we set the current dialogue state.
        case ActionTypes.DIALOGUE_SUCCESS:
            return { ...state, loading: false, error: null,
                currentDialogue: action.payload,
                dialogueIndex: action.payload.index || 0
            };
        
        // if the dialogue starts fails, we set the error state.
        case ActionTypes.DIALOGUE_ERROR:
            return { ...state, loading: false, error: action.payload };
        
        // if the navigation starts, we set the current navigation state.
        case ActionTypes.NAVIGATE_START:
            return { ...state, loading: true, error: null };
        
        // if the application is loading, we set the loading state.
        case ActionTypes.SET_LOADING:
            return { ...state, loading: action.payload };
        
        // if the application has an error, we set the error state.
        case ActionTypes.SET_ERROR:
            return { ...state, error: action.payload };
        
        // if the error is cleared, we clear the error state.
        case ActionTypes.CLEAR_ERROR:
            return { ...state, error: null };

        case ActionTypes.SET_PLAYER_STATE:
            return { ...state, playerState: action.payload };
        
        // return a state as default.
        default:
            return state;
    }
}

// create the context.
const GameContext = createContext();

// provider compontent that wraps the application 
// and provides the context to the children components.

export function GameProvider({ children }) {
    // use the reducer to manage the state.
    const [state, dispatch] = useReducer(gameReducer, startState);

    // handles the login process.
    const login = async (credentials) => {
        // try to send the credentials to the backend.
        try {
            dispatch({ type: ActionTypes.LOGIN_START });

            // get the response from the API.
            const user = await auth.login(credentials);

            // if the login is successful, we dispatch the success action.
            // to update the state.
            dispatch({ type: ActionTypes.LOGIN_SUCCESS, payload: user });
            // return the user.
            return user;
        } catch (error) {
            // if the login fails, we dispatch the error action.
            const errorMessage = error.response?.data || 'Login failed';

            dispatch({ type: ActionTypes.LOGIN_ERROR, payload: error.message });
            // return the error.
            throw error;
        }
    };

    // register user and return the result.
    const register = async (userData) => {
        // try to register the user,
        try {
            dispatch({ type: ActionTypes.REGISTER_START });
            const user = await auth.register(userData);

            dispatch({ type: ActionTypes.REGISTER_SUCCESS, payload: user });
            // return the user.
            return user;
        }
        catch (error) {
            const errorMessage = error.response?.data || 'Registration failed';
            dispatch({ type: ActionTypes.REGISTER_ERROR, payload: errorMessage });
            throw error;
        }
    };

    // logout user and return the result.
    const logout = async () => {
        // try to logout the user,
        try {
            await auth.logout();
        } catch (error) {
            console.error('Logout error:', error);
        } finally {
            dispatch({ type: ActionTypes.LOGOUT });
        }
        return true;
    };

    // start the game and return the result.
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

    // load the game and return the result.
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

    // delete a save and return the result.
    const deleteSave = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.DELETE_SAVE_START });
            await game.deleteSave(saveId);
            dispatch({ type: ActionTypes.DELETE_SAVE_SUCCESS, payload: saveId });
            // return the result.
            return true;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to delete save';
            dispatch({ type: ActionTypes.DELETE_SAVE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };


    // update a save and return the result.
    const updateSave = async (saveId, saveData) => {
        try {
            dispatch({ type: ActionTypes.UPDATE_SAVE });

            // get the save from the API.
            const save = await game.updateSave(saveId, saveData);
            dispatch({ type: ActionTypes.GAME_SUCCESS, payload: save });
            // return the save.
            return save;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to update save';
            dispatch({ type: ActionTypes.GAME_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // get player state and return the result.
    const getPlayerState = async (playerCharacterId) => {
        try {
            dispatch({ type: ActionTypes.SET_LOADING, payload: true });
            const playerState = await story.getPlayerState(playerCharacterId);
            dispatch({ type: ActionTypes.SET_LOADING, payload: false });
            dispatch({ type: ActionTypes.SET_PLAYER_STATE, payload: playerState });
            return playerState;
            
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to get player state';
            dispatch({ type: ActionTypes.SET_ERROR, payload: errorMessage });
            throw error;
        }
    };

    // get the current node the user is on and return the result.
    const getCurrentNode = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NODE_START });

            // get the current node from the API.
            const node = await story.getCurrentNode(saveId);

            // send the get current node success action to the reducer.
            dispatch({ type: ActionTypes.NODE_SUCCESS, payload: node });
            // return the node.
            return node;
        }
        catch (error) {
            const errorMessage = error.response?.data || 'Failed to get current node';
            dispatch({ type: ActionTypes.NODE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // make a choice and return the result.
    const makeChoice = async (saveId, choiceId) => {
        try {
            dispatch({ type: ActionTypes.CHOICE_START });

            // get the choice from the API.
            const choice = await story.makeChoice(saveId, choiceId);

            // send the make choice success action to the reducer.
            dispatch({ type: ActionTypes.CHOICE_SUCCESS, payload: choice });

            // return the choice.
            return choice;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to make choice';
            dispatch({ type: ActionTypes.CHOICE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // go back to the previous node and return the result.
    const goBack = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NAVIGATE_START });

            // get the previous node from the API.
            const node = await story.goBack(saveId);

            // send the go back success action to the reducer.
            dispatch({ type: ActionTypes.NAVIGATE_SUCCESS, payload: node });
            // return the node.
            return node;

        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to go back';
            dispatch({ type: ActionTypes.NAVIGATE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // go forward to the next node and return the result.
    const nextNode = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.NAVIGATE_START });

            // get the next node from the API.
            const node = await story.nextNode(saveId);

            // send the go forward success action to the reducer.
            dispatch({ type: ActionTypes.NAVIGATE_SUCCESS, payload: node });
            // return the node.
            return node;
        } catch (error) {
            const errorMessage = error.response?.data || 'Failed to go forward';
            dispatch({ type: ActionTypes.NAVIGATE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // get the next dialogue and return the result.
    const getNextDialogue = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.DIALOGUE_START });

            // get the next dialogue from the API.
            const dialogue = await story.getNextDialogue(saveId);

            // send the get next dialogue success action to the reducer.
            dispatch({ type: ActionTypes.DIALOGUE_SUCCESS, payload: dialogue });
            // return the dialogue.
            return dialogue;
        }
        catch (error) {
            const errorMessage = error.response?.data || 'Failed to get next dialogue';
            dispatch({ type: ActionTypes.DIALOGUE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // skip to the last dialogue and return the result.
    const skipToLastDialogue = async (saveId) => {
        try {
            dispatch({ type: ActionTypes.DIALOGUE_START });

            // get the last dialogue from the API.
            const dialogue = await story.skipToLastDialogue(saveId);

            // send the skip to last dialogue success action to the reducer.
            dispatch({ type: ActionTypes.DIALOGUE_SUCCESS, payload: dialogue });
            // return the dialogue.
            return dialogue;

        } catch (error) {

            const errorMessage = error.response?.data || 'Failed to skip to last dialogue';
            dispatch({ type: ActionTypes.DIALOGUE_ERROR, payload: errorMessage });
            // return the error.
            throw error;
        }
    };

    // UI actions.
    // set the loading state.
    const setLoading = (loading) => {
        dispatch({ type: ActionTypes.SET_LOADING, payload: loading });
    };

    // set the error state.
    const setError = (error) => {
        dispatch({ type: ActionTypes.SET_ERROR, payload: error });
    };

    // clear the error state.
    const clearError = () => {
        dispatch({ type: ActionTypes.CLEAR_ERROR });
    };

    // values to return.
    const values = {
        // state.
        ...state,
        // auth actions.
        login,
        register,
        logout,
        // game actions.
        startGame,
        loadGame,
        getAllSaves,
        deleteSave,
        updateSave,

        // story actions.
        getCurrentNode,
        makeChoice,
        goBack,
        nextNode,
        getPlayerState,

        // dialogue actions.
        getNextDialogue,
        skipToLastDialogue,

        // UI actions.
        setLoading,
        setError,
        clearError,
    };

    // finally packagge the context and return it.
    return (
        <GameContext.Provider value={values}>
            {children}
        </GameContext.Provider>
    );
}

// hooks for using the context.
export function useGame() {
    const context = useContext(GameContext);
    if (!context) {
        throw new Error('useGame must be used within a GameProvider');
    }
    return context;
}

// export the context.
export default GameContext;