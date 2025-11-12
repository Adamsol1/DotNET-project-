import React, { useState } from 'react';
import { motion } from 'framer-motion';

// component imports . gameContext has api calls and game state management.
import { useGame } from '../context/GameContext';
// planet, spacehsip, stars components. are for the background animation.
import Planet from '../components/Home/Planet';
import Spaceship from '../components/Home/Spaceship';
import Stars from '../components/Home/Stars';

import {useAuth} from "../context/Authentication";
import * as authservice from "../endpoints/AuthenticationService";


export function Home({ onNavigate }) {
  const { user, logout, login, register } = useAuth();
  //CHAT
  const  authenticated= !!user;
  const [activeTab, setActiveTab] = useState('login');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  // log inn the user, call the login function. from api.auth.login
  const handleLogin = async () => {
    try {
      // passes inn the username and password captured from the form 
      // to the login function.
      await login({ username, password });

    } catch (error) {
      console.error('Login failed:', error);
    }
  }

  // register the user, call the register function. from api.auth.register
  const handleRegister = async () => {
    try {
      // passes inn the username and password captured from the form 
      // to the register function.
      await authservice.register({ username, password });
    } catch (error) {
      console.error('Register failed:', error);
    }
  }

  // handle the logout: -> function call to logout method.
  // which in the feature just removes the session.
  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };


  // since login becomes essentially the HomePage, 
  // we check if the user is logged in, if yes, then we show play game button.
  // instead of the login form.

  // we are also using tailwind for styling the home page.
  return (
    <div 
      className="relative min-h-screen"
    >
      <Stars />
      <Planet />
      <Spaceship />
      {/* Main Content */}
      <div className="relative z-10 min-h-screen flex flex-col">
        {/* Header */}
        <header className="p-6 pt-20 flex justify-center items-center">
          <motion.h1
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
            className="text-7xl font-bold text-white pixel-text tracking-wider text-center w-full"
            style={{ textShadow: '3px 3px 0px rgba(0, 0, 0, 0.8)' }}
          >
            AFTER THE JUMP
          </motion.h1>
        </header>

        {/* Main Content - Login/Register eller Welcome */}
        <main className="flex-1 flex items-center justify-center px-6">
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.5 }}
            className="w-full max-w-md"
          >
            {authenticated ? (
              // Authenticated View
              <div className="text-center">
                <div className="mb-8">
                  <h2 className="text-2xl font-bold text-white mb-4 pixel-text">
                    Welcome back, {user?.username}!
                  </h2>
                  <p className="text-gray-300 mb-8">
                    Ready to continue your adventure?
                  </p>
                </div>
                
                <div className="space-y-4">
                  <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    onClick={() => onNavigate('game')}
                    className="w-full px-4 py-3 bg-gray-200 text-black font-bold border-2 border-black"
                  >
                    PLAY GAME
                  </motion.button>
                  
                  <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    onClick={handleLogout}
                    className="w-full px-4 py-3 bg-transparent text-white font-bold border-2 border-white"
                  >
                    LOGOUT
                  </motion.button>
                </div>
              </div>
            ) : (
              // Login/Register View
              <>
                {/* Tabs */}
                <div className="flex mb-6 border-2 border-black">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    onClick={() => setActiveTab('login')}
                    className={`flex-1 px-6 py-3 font-bold transition-colors ${
                      activeTab === 'login' 
                        ? 'bg-blue-600 text-white' 
                        : 'bg-white text-blue-600'
                    }`}
                  >
                    LOGIN
                  </motion.button>
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    onClick={() => setActiveTab('register')}
                    className={`flex-1 px-6 py-3 font-bold transition-colors ${
                      activeTab === 'register' 
                        ? 'bg-blue-600 text-white' 
                        : 'bg-white text-blue-600'
                    }`}
                  >
                    REGISTER
                  </motion.button>
                </div>

                {/* Form */}
                <div className="space-y-4">
                  {/* Username Field */}
                  <motion.div
                    initial={{ x: -20, opacity: 0 }}
                    animate={{ x: 0, opacity: 1 }}
                    transition={{ delay: 0.2 }}
                  >
                    <input
                      type="text"
                      placeholder="USERNAME"
                      value={username}
                      onChange={(e) => setUsername(e.target.value)}
                      className="w-full px-4 py-3 bg-gray-200 text-black text-center font-bold border-2 border-black focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </motion.div>
                  
                  {/* Password Field */}
                  <motion.div
                    initial={{ x: -20, opacity: 0 }}
                    animate={{ x: 0, opacity: 1 }}
                    transition={{ delay: 0.3 }}
                  >
                    <input
                      type="password"
                      placeholder="PASSWORD"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      className="w-full px-4 py-3 bg-gray-200 text-black text-center font-bold border-2 border-black focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </motion.div>
                  
                  {/* Single Dynamic Button */}
                  <motion.div
                    initial={{ y: 20, opacity: 0 }}
                    animate={{ y: 0, opacity: 1 }}
                    transition={{ delay: 0.4 }}
                  >
                    <motion.button
                      whileHover={{ scale: 1.05, boxShadow: '0 0 20px rgba(255, 255, 255, 0.5)' }}
                      whileTap={{ scale: 0.95 }}
                      onClick={activeTab === 'login' ? handleLogin : handleRegister}
                      className="w-full px-4 py-3 bg-gray-200 text-black font-bold border-2 border-black"
                    >
                      {activeTab === 'login' ? 'LOG-IN' : 'REGISTER'}
                    </motion.button>
                  </motion.div>
                </div>
              </>
            )}
          </motion.div>
        </main>
      </div>
    </div>
  );

}