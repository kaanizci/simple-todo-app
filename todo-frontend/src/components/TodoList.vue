<template>
  <div>
    <h2 class="text-xl font-bold mb-4">Görevler</h2>

    <!-- Filtreleme seçenekleri -->
    <div class="mb-4 space-x-4">
      <label><input type="radio" value="all" v-model="filter" /> Tümü</label>
      <label><input type="radio" value="completed" v-model="filter" /> Tamamlanan</label>
      <label><input type="radio" value="incomplete" v-model="filter" /> Devam Eden</label>
    </div>

    <p v-if="loading">Yükleniyor...</p>
    <p v-if="error" class="text-red-600">Hata: {{ error }}</p>

    <ul v-else>
      <li
        v-for="todo in filteredTodos"
        :key="todo.id"
        :class="{ overdue: todo.dueDate && new Date(todo.dueDate) < new Date() && !todo.isCompleted }"
        class="mb-2 p-2 border rounded"
      >
        <strong>{{ todo.title }}</strong> -
        <span>{{ todo.isCompleted ? 'Tamamlandı' : 'Devam Ediyor' }}</span> -
        <span>
          Son Tarih:
          {{ todo.dueDate ? new Date(todo.dueDate).toLocaleDateString() : 'Belirtilmemiş' }}
        </span>
        <br />
        <button @click="toggleStatus(todo)" class="mr-2 px-2 py-1 bg-green-500 text-white rounded">
          {{ todo.isCompleted ? 'Geri Al' : 'Tamamla' }}
        </button>
        <button @click="deleteTodo(todo.id)" class="px-2 py-1 bg-red-500 text-white rounded">Sil</button>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { getCurrentUserToken } from '../firebase'

const todos = ref([])
const loading = ref(true)
const error = ref('')
const filter = ref('all')

onMounted(fetchTodos)

// ✅ Görevleri kullanıcıya özel olarak al
async function fetchTodos() {
  try {
    const token = await getCurrentUserToken()
    if (!token) throw new Error('Token alınamadı')

    const response = await fetch('http://localhost:5000/api/todoitems', {
      headers: { Authorization: `Bearer ${token}` }
    })

    if (!response.ok) throw new Error('Görevleri alma başarısız')
    todos.value = await response.json()
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

// ✅ Görevi sil
async function deleteTodo(id) {
  try {
    const token = await getCurrentUserToken()
    if (!token) throw new Error('Token alınamadı')

    await fetch(`http://localhost:5000/api/todoitems/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` }
    })

    todos.value = todos.value.filter(todo => todo.id !== id)
  } catch (err) {
    console.error('Silme hatası:', err.message)
  }
}

// ✅ Görev durumunu değiştir
async function toggleStatus(todo) {
  const updated = { ...todo, isCompleted: !todo.isCompleted }

  try {
    const token = await getCurrentUserToken()
    if (!token) throw new Error('Token alınamadı')

    await fetch(`http://localhost:5000/api/todoitems/${todo.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(updated)
    })

    todo.isCompleted = !todo.isCompleted
  } catch (err) {
    console.error('Güncelleme hatası:', err.message)
  }
}

// 🔍 Filtreleme
const filteredTodos = computed(() => {
  if (filter.value === 'completed') {
    return todos.value.filter(t => t.isCompleted)
  } else if (filter.value === 'incomplete') {
    return todos.value.filter(t => !t.isCompleted)
  } else {
    return todos.value
  }
})
</script>

<style scoped>
.overdue {
  color: red;
  font-weight: bold;
}
</style>