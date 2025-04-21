<template>
  <form @submit.prevent="add" class="space-y-4">
    <input
      v-model="title"
      placeholder="Yeni görev..."
      required
      class="w-full p-2 border rounded"
    />
    <input
      type="date"
      v-model="dueDate"
      class="w-full p-2 border rounded"
    />
    <button type="submit" class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded">
      Ekle
    </button>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { getCurrentUserToken } from '../firebase'
const title = ref('')
const dueDate = ref('')

// 🎯 App.vue'ya bilgi göndermek için emit sistemi
const emit = defineEmits(['todo-added']) // App.vue'deki fetchTodos'u tetikler

// ✅ Yeni görev ekle
async function add() {
  const trimmedTitle = title.value.trim()
  if (!trimmedTitle) return

  const token = await getCurrentUserToken()
  if (!token) {
    alert("Token alınamadı, giriş yapılmamış olabilir.")
    return
  }

  const newTodo = {
    title: trimmedTitle,
    isCompleted: false,
    dueDate: dueDate.value ? new Date(dueDate.value).toISOString() : null
  }

  try {
    const response = await fetch('http://localhost:5000/api/todoitems', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(newTodo)
    })

    if (!response.ok) throw new Error("Sunucudan hata geldi")

    const data = await response.json()
    console.log("Görev başarıyla eklendi:", data)

    title.value = ''
    dueDate.value = ''

    // 🧠 App.vue'ya sinyal gönder, listeyi güncelle
    emit('todo-added')
  } catch (err) {
    console.error("Hata:", err.message)
    alert("Görev eklenemedi: " + err.message)
  }
}
</script>