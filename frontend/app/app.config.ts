export default defineAppConfig({
  ui: {
    primary: 'aurora',       // maps to aurora teal (#3de8c8)
    gray:    'space',        // custom gray scale (defined in app/assets/css/main.css)

    button: {
      // font override — all buttons use Space Grotesk
      base: 'font-body tracking-wide',
      rounded: 'rounded-sm',     // --r-sm = 3px
    },

    input: {
      base: 'font-body font-light',
      rounded: 'rounded-sm',
    },

    badge: {
      rounded: 'rounded-none',    // AudioAtlas tags are square
    },

    card: {
      rounded: 'rounded-md',      // --r-md = 6px
      base: 'overflow-hidden',
      background: 'bg-surface-2',
      ring: 'ring-1 ring-border',
    },

    alert: {
      rounded: 'rounded-md',
    },

    breadcrumb: {
      base: 'font-mono text-[0.62rem] tracking-wider',
    }
  }
})
