import axios from 'axios';

/**
 * The purpose and plan for this file is to create a single channel? / place where
 * all api requests are made or tunneled through to the backend. 
 * if your ever are missing something, check the backend controller.
 *and add the api call your missing here. 
 -Ah
 */

 /* this is an instace that holds the base configuration for the api requests.
 so you dont have to repeat them on every request.
 contains the baseUrl, limit on how long the request should take to avoid blocking the thread.
 and the header. normally we send json, but if you need to send something else then json,
 then u can override in the header where you are doing the request.
 */
const api = axios.create({
    baseURL: 'http://localhost:5169/api',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// should we enforce usage of JWT token for the api requests?
// for know il not include it.

// interceptor for error handling.
api.interceptors.response.use(
    (response) => response,
    (error) => {
        // log the error message or the error data to the console.
        if (error.response?.data) {
            console.error('API Error:', JSON.stringify(error.response.data, null, 2));
        } else {
            console.error('API Error:', error.message);
        }
        // return the error.
        return Promise.reject(error);
    }
);

/**
 * Authentication endpoints. for login, register, logout.
 */
export const auth = {
    // register takes in a data objectm which is the user data to be registered.
    register: async (data) => {
        const response = await api.post('/auth/register', data);
        return response.data;
    },

    // login endpoint. takes in the users credentials to be logged in
    // and returns that back. if successful.
    login: async (cred) => {
        const response = await api.post('/auth/login', cred);
        return response.data;
    },

    // logout endpoint does not take in any data. and just logs out
    // to logout the user, we send post to logout endpoint which deletes the session.
    logout: async () => {
        const response = await api.post('/auth/logout');
        return response.data;
    },

    // get currentUser, to know who is logged in.
    getCurrentUser: async () => {
        const response = await api.get('/auth/user');
        return response.data;
    },
};

// game API endpoints.

export const game = {
    //endpoint to start a new game.
    startGame: async (data) => {
        const response = await api.post('/game/start', data);
        return response.data;
    },

    // get or load a game save.
    loadGame: async (saveId) => {
        const response = await api.get(`/game/load/${saveId}`);
        return response.data;
    },

    // get all saves that belong to the user.
    getAllSaves: async (userId) => {
        const response = await api.get(`/game/saves/${userId}`);
        return response.data;
    },

    // delete a game save.
    deleteSave: async (saveId) => {
        const response = await api.delete(`/game/saves/${saveId}`);
        return response.data;
    },

    // save or update a game save.
    saveGame: async ( data ) => {
        const response = await api.put(`/game/save`, data);
        return response.data;
    },

    // update a game save.
    updateSave: async (saveId, data) => {
        const response = await api.put(`/game/saves/${saveId}`, data);
        return response.data;
    },
};

// story API endpoints.

export const story = {
    // get the current story node for a given save id.
    getCurrentNode: async (saveId) => {
        const response = await api.get(`/story/current/${saveId}`);
        return response.data;
    },

    // get the next story node for a given save id.
    navigateToNode: async (saveId, nodeId) => {
        const response = await api.post(`/story/nav/${nodeId}`, null, {
          params: { saveId }
        });
        return response.data;
    },

    // go back to the previous story node for a given save id.
    goBack: async (saveId) => {
        const response = await api.post(`/story/back/${saveId}`);
        return response.data;
    },

    // move to the nextNode for a given save id.
    nextNode: async (saveId) => {
        const response = await api.post(`/story/next/${saveId}`);
        return response.data;
    },

    // make a choice takes the session/ save id, and the choice id to make.
    makeChoice: async (saveId, choiceId) => {
        const response = await api.post(`/story/choice`, { saveId, choiceId });
        return response.data;
    },

    // get available choices for the current story node.
    getAvailableChoices: async (saveId) => {
        const response = await api.get(`/story/choices/${saveId}`);
        return response.data;
    },

    // get the next dialogue for the current story node.
    getNextDialogue: async (saveId) => {
        const response = await api.get(`/story/dialogue/next/${saveId}`);
        return response.data;
    },

    // skip to the last dialogue for the current story node.
    skipToLastDialogue: async (saveId) => {
        const response = await api.post(`/story/dialogue/skip/${saveId}`);
        return response.data;
    },

    isDialogueComplete: async (saveId) => {
        const response = await api.get(`/story/dialogue/complete/${saveId}`);
        return response.data;
    },

    // modify health from choice.
    modifyHealth: async (playerCharacterId, healthValue) => {
        const response = await api.post('/story/health', { playerCharacterId, healthValue });
        return response.data;
    },

    // get the nodes the player has visited.
    getVisitedNodes: async (saveId) => {
        const response = await api.get(`/story/history/${saveId}`);
        return response.data;
    },

    // check if the player has visited a specific node.
    hasVisitedNode: async (saveId, nodeId) => {
        const response = await api.get(`/story/history/${saveId}/${nodeId}`);
        return response.data;
    },

    // get player state.
    getPlayerState: async (playerCharacterId) => {
        const response = await api.get(`/story/player/${playerCharacterId}`);
        return response.data;
    },

};


// export the api endpoints
export default api;