/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      // Definim paleta noastra personalizata de culori
      colors: {
        mystic: {
          dark: '#0f2e22',    // Verde foarte închis (pentru navbar/footer)
          primary: '#1a4731', // Verdele principal (pădure adâncă)
          accent: '#4a7c59',  // Un verde mediu pentru butoane/elemente de accent
          light: '#a4c3b2',   // Verde salvie deschis (pentru text secundar pe fundal închis)
          pale: '#e8f3ee',    // Un alb-verzui foarte pal (pentru fundaluri de pagină)
        }
      },
      // Extindem fontul standard pentru a fi mai modern
      fontFamily: {
        sans: ['Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
      }
    },
  },
  plugins: [],
}


