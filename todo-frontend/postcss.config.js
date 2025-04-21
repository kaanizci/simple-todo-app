import tailwindcss from '@tailwindcss/postcss'
import autoprefixer from 'autoprefixer'

export default {
  plugins: [
    tailwindcss(), // @tailwindcss/postcss kullanılıyor
    autoprefixer()
  ]
}