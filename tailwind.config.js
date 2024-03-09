/** @type {import('tailwindcss').Config} */
module.exports = {
  mode: "jit",
  content: [
    "./src/index.html",
    "./src/**/*.{fs,js,ts,jsx,tsx}"
  ],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
  daisyui: {
      themes: [
          "light", "dark", "lofi",
          {
              MyTheme: {
                  "primary": "#87642C"
              }
          }
      ],
  },
}
