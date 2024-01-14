import {defineConfig} from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "ZBXLLD",
  description: "ZBXLLD provides additional discovery features to Zabbix agent",
  head: [
    ['meta', { property: 'og:title', content: 'ZBXLLD Documentation' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:description', content: 'ZBXLLD provides additional discovery features to Zabbix agent' }],
    ['meta', { property: 'og:url', content: 'https://fgodoy.me/zbxlld/' }]
  ],

  lastUpdated: true,

  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Home', link: '/' },
      { text: 'History', link: '/history' },
      { text: 'Discussions', link: 'https://github.com/skarllot/zbxlld/discussions' }
    ],

    sidebar: [
      {
        text: 'Introduction',
        items: [
          { text: 'Getting Started', link: '/getting-started' }
        ]
      },
      {
        text: 'Keys',
        items: [
          { text: 'Features', link: '/keys/' },
          { text: 'Drive', link: '/keys/drive' },
          { text: 'Network', link: '/keys/network' },
          { text: 'Service', link: '/keys/service' }
        ]
      }
    ],

    socialLinks: [
      { icon: 'github', link: 'https://github.com/skarllot/zbxlld' }
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © Fabricio Godoy and contributors.'
    },
    editLink: {
      pattern: 'https://github.com/skarllot/zbxlld/edit/master/docs/:path',
      text: 'Edit this page on GitHub'
    }
  },

  base: '/zbxlld/',
  sitemap: {
    hostname: "https://fgodoy.me/zbxlld/"
  }
})
