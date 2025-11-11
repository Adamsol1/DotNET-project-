import React, {useState, useEffect, createContext, useContext, ReactNode} from 'react';
import { jwtDecode } from "jwt-decode";
/** @typedef {import('..types/Auth').LoginUserDto} LoginUserDto */
/** @typedef {import('..types/Auth').RegistrerUserDto} RegistrerUserDto */
import * as authservice from "./authservice";
import {pem as jwt} from "node-forge";


//This is based on the demo JWTAuthentication - Frontend

const AuthContext = createContext(undefined);
export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [token, setToken] = useState(localStorage.getItem('token'));

    useEffect(() => {
        if(token){
            try{
                const decodedUser = jwtDecode(token);
                if(decodedUser.exp * 1000 > Date.now()){
                    setUser(decodedUser);
                } else {
                    console.warn("Token is expired!");
                    localStorage.removeItem('token');
                    setUser(null);
                    setToken(null);
             }
            }
            catch(error){
                console.error("Token is not valid", error);
                localStorage.removeItem('token');
                }   
            }
        setLoading(false);
    }, [token]);

    const login = async ({ username, password }) => {
        const user = await authservice.login(username, password);
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', user.username);
        localStorage.setItem('user_id', user.id);

        const decodedUser = jwtDecode(token);
        setUser(decodedUser);
        setToken(user.token);

}

    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
        setToken(null);
    }


    return (
    <AuthContext.Provider value={{user, token, login, logout, Loading}}>
        {!Loading && children}
    </AuthContext.Provider>
    )


    export function useAuth() {
        const context = useContext(AuthContext);
        if (!context) {
            //TODO : Fix error message and handling
            throw new Error('useAuth must be used within the AuthProvider');
        }
            return context;
    }
}
