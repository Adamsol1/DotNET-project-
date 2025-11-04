import React, { useState } from 'react';
import { motion } from 'framer-motion';
import Stars from '../components/Home/Stars';
import Planet from '../components/Home/Planet';
import Spaceship from '../components/Home/Spaceship';
import { useGame } from '../context/GameContext';

export function AccountManagement({ onNavigate }) {
  const { updateUsername, updatePassword, deleteAccount } = useGame();
  const [username, setUsername] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleUpdateUsername = async (e) => {
    e.preventDefault();
    if (!username) return;

    try {
      await updateUsername({ username });
      setUsername('');
    } catch (error) {
      console.error('Update failed:', error);
    }
  };

  const handleUpdatePassword = async (e) => {
    e.preventDefault();
    if (!newPassword || newPassword !== confirmPassword) return;

    try {
      await updatePassword({ newPassword, confirmPassword });
      setNewPassword('');
      setConfirmPassword('');
    } catch (error) {
      console.error('Password update failed:', error);
    }
  };

  const handleDeleteAccount = async () => {
    try {
      await deleteAccount();
      onNavigate('home');
    } catch (error) {
      console.error('Account deletion failed:', error);
    }
  };

  return (
    <div className="relative min-h-screen overflow-auto">
      <Stars />
      <Planet />
      <Spaceship />

      <div className="relative z-10 flex flex-col items-center justify-start min-h-screen px-6 py-24">
        <motion.h1
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="text-6xl font-bold text-white tracking-wider text-center mb-12 pixel-text"
          style={{ textShadow: '3px 3px 0px rgba(0,0,0,0.8)' }}
        >
          ACCOUNT MANAGEMENT
        </motion.h1>

        <motion.button
          whileHover={{ scale: 1.05, boxShadow: '0 0 20px rgba(255, 255, 255, 0.5)' }}
          whileTap={{ scale: 0.95 }}
          onClick={() => onNavigate('home')}
          className="mb-8 px-6 py-3 bg-gray-200 text-black font-bold border-2 border-black"
        >
          BACK TO HOME
        </motion.button>

        <div className="w-full max-w-md space-y-8">
          {/* Update Username */}
          <section>
            <h2 className="font-bold text-white mb-2">Update Username</h2>
            <form onSubmit={handleUpdateUsername} className="space-y-3">
              <input
                type="text"
                placeholder="New username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full px-4 py-3 bg-gray-200 text-black text-center font-bold border-2 border-black focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <motion.button
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                type="submit"
                className="w-full px-4 py-3 bg-gray-200 text-black font-bold border-2 border-black"
              >
                SAVE USERNAME
              </motion.button>
            </form>
          </section>

          {/* Update Password */}
          <section>
            <h2 className="font-bold text-white mb-2">Update Password</h2>
            <form onSubmit={handleUpdatePassword} className="space-y-3">
              <input
                type="password"
                placeholder="New password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                className="w-full px-4 py-3 bg-gray-200 text-black text-center font-bold border-2 border-black focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <input
                type="password"
                placeholder="Confirm new password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="w-full px-4 py-3 bg-gray-200 text-black text-center font-bold border-2 border-black focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <motion.button
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                type="submit"
                className="w-full px-4 py-3 bg-gray-200 text-black font-bold border-2 border-black"
              >
                SAVE PASSWORD
              </motion.button>
            </form>
          </section>

          {/* Delete Account */}
          <section>
            <h2 className="font-bold text-red-500 mb-2">Delete Account</h2>
            <p className="text-white mb-2">This action is permanent and cannot be undone.</p>
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              onClick={handleDeleteAccount}
              className="w-full px-4 py-3 bg-red-600 text-white font-bold border-2 border-red-800"
            >
              DELETE ACCOUNT
            </motion.button>
          </section>
        </div>
      </div>
    </div>
  );
}
