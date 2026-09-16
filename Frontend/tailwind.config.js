/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Fredoka', 'sans-serif'], 
      },
      colors: {
        brand: {
          gold: '#f1c40f',
          dark: '#0a0a0a',
          black: '#050505'
        }
      }
    },
  },
  plugins: [],
}