<template>
  <form @submit.prevent="sendResetEmail" class="space-y-4 max-w-md mx-auto p-4 border rounded bg-gray-100 dark:bg-gray-800">
    <h2 class="text-xl font-semibold text-gray-800 dark:text-white">🔑 Şifre Sıfırla</h2>

    <input
      type="email"
      v-model="email"
      placeholder="E-posta adresinizi girin"
      required
      class="w-full p-2 rounded border border-gray-300 dark:bg-gray-700 dark:text-white"
    />

    <button
      type="submit"
      class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded w-full"
    >
      Sıfırlama Linki Gönder
    </button>

    <p v-if="message" class="text-green-600 dark:text-green-400">{{ message }}</p>
    <p v-if="error" class="text-red-600 dark:text-red-400">{{ error }}</p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { sendPasswordResetEmail } from 'firebase/auth'
import { auth } from '../firebase'

const email = ref('')
const message = ref('')
const error = ref('')

async function sendResetEmail() {
  message.value = ''
  error.value = ''
  try {
    await sendPasswordResetEmail(auth, email.value)
    message.value = '📧 Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.'
    email.value = ''
  } catch (err) {
    error.value = 'Hata: ' + (err.message || 'Şifre sıfırlama başarısız oldu.')
  }
}
</script>