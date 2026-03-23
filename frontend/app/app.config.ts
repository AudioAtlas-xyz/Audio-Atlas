export default defineAppConfig({
  ui: {
    primary: 'aurora', // maps to aurora teal (#3de8c8)
    gray: 'space', // custom gray scale (defined in app/assets/css/main.css)

    button: {
      slots: {
        base: 'font-body tracking-wide rounded-sm', // --r-sm = 3px
      },
    },

    input: {
      slots: {
        base: 'font-body font-light rounded-sm',
      },
    },

    badge: {
      slots: {
        base: 'rounded-none', // AudioAtlas tags are square
      },
    },

    card: {
      slots: {
        root: 'rounded-md overflow-hidden bg-surface-2 ring-1 ring-border', // --r-md = 6px
      },
    },

    alert: {
      slots: {
        root: 'rounded-md',
      },
    },

    breadcrumb: {
      slots: {
        link: 'font-mono text-[0.62rem] tracking-wider',
      },
    },
  },
})
