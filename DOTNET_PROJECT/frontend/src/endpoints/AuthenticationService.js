/** @typedef {import('..types/Auth').LoginUserDto} LoginUserDto */
/** @typedef {import('..types/Auth').RegistrerUserDto} RegistrerUserDto */
import {auth} from "./api";

export async function login(loginCredentials) {
    const payload = {username: loginCredentials.username, password: loginCredentials.password}
    return auth.login(payload);
}

export async function register(userCredentials) {
    const payload = {username: userCredentials.username, password: userCredentials.password}
    return auth.register(payload);
}


