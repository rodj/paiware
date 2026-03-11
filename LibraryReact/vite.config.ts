import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const rodjBuild = process.env.RODJBUILD

export default defineConfig({
  plugins: [react(), tailwindcss()],
  base: '/library/',
  build: {
    outDir: rodjBuild ? `${rodjBuild}/paiwares/LibraryReact/dist` : 'dist',
  },
})
