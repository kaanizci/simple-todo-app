<template>
  <div class="max-w-md mx-auto p-6 bg-white shadow rounded dark:bg-gray-800">
    <h2 class="text-xl font-bold mb-4 text-center dark:text-white">Giriş Yap</h2>
    <form @submit.prevent="login">
      <input
        v-model="email"
        type="email"
        placeholder="Email"
        class="w-full p-2 mb-3 border rounded dark:bg-gray-700 dark:text-white"
        required
      />
      <input
        v-model="password"
        type="password"
        placeholder="Şifre"
        class="w-full p-2 mb-3 border rounded dark:bg-gray-700 dark:text-white"
        required
      />
      <button
        type="submit"
        class="w-full p-2 bg-blue-600 text-white rounded hover:bg-blue-700"
      >
        Giriş Yap
      </button>
      <p v-if="error" class="text-red-500 mt-2">{{ error }}</p>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { getAuth, signInWithEmailAndPassword } from 'firebase/auth'

const email = ref('')
const password = ref('')
const error = ref('')

const login = async () => {
  const auth = getAuth()
  try {
    await signInWithEmailAndPassword(auth, email.value, password.value)
    alert('Giriş başarılı!')
    // Örn: router ile yönlendirme burada yapılabilir
  } catch (err) {
    error.value = err.message
  }
}
</script>