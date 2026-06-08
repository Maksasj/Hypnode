import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// VITE_BASE is set in CI to '/Hypnode/' for GitHub Pages deployment.
// Locally it defaults to '/' so dev server works without any config.
export default defineConfig({
  plugins: [react()],
  base: process.env.VITE_BASE ?? '/',
})
