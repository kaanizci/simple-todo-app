<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-gray-100 dark:bg-gray-800 rounded shadow">
    <h2 class="text-2xl font-bold mb-4">Kayıt Ol</h2>

    <form @submit.prevent="register">
      <input
        v-model="email"
        type="email"
        placeholder="Email"
        class="w-full p-2 mb-4 rounded border"
      />
      <input
        v-model="password"
        type="password"
        placeholder="Şifre"
        class="w-full p-2 mb-4 rounded border"
      />
      <button type="submit" class="w-full bg-blue-500 text-white p-2 rounded">
        Kayıt Ol
      </button>
    </form>

    <p v-if="error" class="mt-4 text-red-500">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { getAuth, createUserWithEmailAndPassword } from 'firebase/auth'

const email = ref('')
const password = ref('')
const error = ref('')

// ⛏️ Mantık: Firebase Auth kullanarak kullanıcı kaydı
const register = async () => {
  const auth = getAuth()
  try {
    await createUserWithEmailAndPassword(auth, email.value, password.value)
    alert('Kayıt başarılı! Artık giriş yapabilirsiniz.')
    email.value = ''
    password.value = ''
    error.value = ''
  } catch (err) {
    error.value = err.message
  }
}
</script>