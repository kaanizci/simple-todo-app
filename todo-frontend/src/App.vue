<template>
  <div class="min-h-screen p-10 bg-white dark:bg-black text-black dark:text-white">
    <!-- 🌗 Dark Mode Butonu -->
    <button @click="toggleDarkMode" class="bg-gray-300 dark:bg-gray-700 px-4 py-2 rounded">
      {{ isDark ? '☀️ Light Mode' : '🌙 Dark Mode' }}
    </button>

    <!-- 🚪 Logout Butonu -->
    <button
      v-if="isAuthenticated"
      @click="logout"
      class="ml-4 bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded"
    >
      Çıkış Yap
    </button>

    <!-- ✔️ Başarı Mesajı -->
    <p v-if="successMessage" class="text-green-600 dark:text-green-400 mt-4">
      {{ successMessage }}
    </p>

    <!-- 👤 Kullanıcı Bilgisi -->
    <p v-if="isAuthenticated && userEmail" class="text-sm text-gray-600 dark:text-gray-400 mt-2">
      👤 Giriş yapan: {{ userEmail }}
    </p>

    <!-- 🔐 Giriş/Kayıt/Şifre Sıfırlama -->
    <div v-if="!isAuthenticated" class="mt-6 space-y-6">
      <!-- Form Geçiş Butonları -->
      <div class="flex justify-center space-x-4 mb-4">
        <button
          @click="currentForm = 'login'"
          :class="{'font-bold underline': currentForm === 'login'}"
          class="px-4 py-2 rounded bg-gray-200 dark:bg-gray-700"
        >
          Giriş Yap
        </button>
        <button
          @click="currentForm = 'register'"
          :class="{'font-bold underline': currentForm === 'register'}"
          class="px-4 py-2 rounded bg-gray-200 dark:bg-gray-700"
        >
          Kayıt Ol
        </button>
        <button
          @click="currentForm = 'reset'"
          :class="{'font-bold underline': currentForm === 'reset'}"
          class="px-4 py-2 rounded bg-gray-200 dark:bg-gray-700"
        >
          Şifre Sıfırla
        </button>
      </div>

      <!-- Formlar -->
      <LoginForm v-if="currentForm === 'login'" />
      <RegisterForm v-if="currentForm === 'register'" />
      <ResetPasswordForm v-if="currentForm === 'reset'" />
    </div>

    <!-- ✅ Görev Yönetimi -->
    <div v-else>
      <h1 class="text-3xl font-bold mt-6">Görev Yönetim Uygulaması</h1>

      <!-- Görev Ekleme -->
      <div class="mt-6">
        <AddTodo @todo-added="addTodoToList" />
      </div>

      <!-- Görev Listeleme -->
      <div class="mt-6">
        <TodoList :todos="todos" @todos-updated="fetchTodos" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import AddTodo from './components/AddTodo.vue'
import TodoList from './components/TodoList.vue'
import LoginForm from './components/LoginForm.vue'
import RegisterForm from './components/RegisterForm.vue'
import ResetPasswordForm from './components/ResetPasswordForm.vue'
import { listenToAuthState, logoutUser, getCurrentUserToken } from './firebase'

// 🔧 Durumlar
const isDark = ref(false)
const isAuthenticated = ref(false)
const successMessage = ref('')
const userEmail = ref('')
const todos = ref([])
const currentForm = ref('login') // İlk olarak giriş ekranı gösterilsin

// 🌗 Dark Mode toggle
function toggleDarkMode() {
  isDark.value = !isDark.value
}

// 🌗 <html> elementine "dark" class'ı ekle/kaldır
watch(isDark, (value) => {
  const html = document.documentElement
  value ? html.classList.add('dark') : html.classList.remove('dark')
})

// 🔐 Firebase ile kullanıcı giriş durumunu izle
onMounted(() => {
  listenToAuthState((user) => {
    isAuthenticated.value = !!user
    if (user) {
      userEmail.value = user.email || ''
      successMessage.value = '✔️ Başarıyla giriş yaptınız!'
      setTimeout(() => (successMessage.value = ''), 4000)
      fetchTodos()
    }
  })
})

// ✅ Token ile görevleri getir
async function fetchTodos() {
  try {
    const token = await getCurrentUserToken()
    const response = await fetch('http://localhost:5000/api/todoitems', {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`
      },
      credentials: 'include'
    })

    if (!response.ok) throw new Error('Görevler alınamadı')
    todos.value = await response.json()
  } catch (error) {
    console.error('Görev alma hatası:', error.message)
  }
}

// 🚪 Logout
async function logout() {
  try {
    await logoutUser()
    userEmail.value = ''
    todos.value = []
    currentForm.value = 'login'
    successMessage.value = '✅ Başarıyla çıkış yapıldı.'
    setTimeout(() => (successMessage.value = ''), 3000)
  } catch (error) {
    console.error('Çıkış hatası:', error.message)
  }
}

// 🔄 Yeni görev anında listeye eklensin
function addTodoToList(newTodo) {
  todos.value.unshift(newTodo)
}
</script>