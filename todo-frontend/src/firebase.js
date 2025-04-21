// src/firebase.js
import { initializeApp } from "firebase/app"
import { getAuth, onAuthStateChanged, signOut } from "firebase/auth"

const firebaseConfig = {
  apiKey: "AIzaSyD-1-UzIXnEBmBn6fpybRVWzpWxmCeXjpM",
  authDomain: "todoapp-241d7.firebaseapp.com",
  projectId: "todoapp-241d7",
  storageBucket: "todoapp-241d7.appspot.com",
  messagingSenderId: "312785016210",
  appId: "1:312785016210:web:xxxxxx"
}

const app = initializeApp(firebaseConfig)
const auth = getAuth(app)

// Firebase kimlik doğrulama durumu dinleyici
export function listenToAuthState(callback) {
  return onAuthStateChanged(auth, callback)
}

// Oturumu kapatmak için
export function logoutUser() {
  return signOut(auth)
}

// 🆕 Kullanıcının kimlik doğrulama token'ını al
export async function getCurrentUserToken() {
    const user = auth.currentUser
    if (user) return await user.getIdToken()
    return null
  }

// 🆕 Şu anki kullanıcıyı al (email için)
export function getCurrentUser() {
  return auth.currentUser
}

export { auth }