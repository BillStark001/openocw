import { defineConfig, PluginOption, splitVendorChunkPlugin, UserConfig, UserConfigExport } from 'vite';
import { resolve, dirname } from 'node:path';
import { fileURLToPath } from 'url';
import { VitePWA } from 'vite-plugin-pwa';
import vue from '@vitejs/plugin-vue'
import vueI18n from '@intlify/vite-plugin-vue-i18n';
// import basicSsl from '@vitejs/plugin-basic-ssl';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {

  const config: UserConfig = {

    server: {
      open: true,
      https: false,
      port: 5002
    },
    plugins: [
      vue(),
      vueI18n({
        // if you want to use Vue I18n Legacy API, you need to set `compositionOnly: false`
        // compositionOnly: false,
        // you need to set i18n resource including paths !
        include: resolve(dirname(fileURLToPath(import.meta.url)), './src/locales/**'),
      }),
      // basicSsl(),
      splitVendorChunkPlugin()
    ],
    assetsInclude: ['**/*.md'],
    resolve: {
      alias: {
        "@": resolve(__dirname, "./src"),
      },
    }
  };

  if (mode === 'client') {
    config.resolve!.alias!['virtual:pwa-register/vue'] = './src/pwa/fakePWARegister.ts';
  } else {
    config.plugins!.push(
      VitePWA({
        registerType: 'autoUpdate',
        workbox: {
          globPatterns: ['**/*.{ico,svg,jpg,jpeg,png,js,css,html,str}']
        }
      })
    );
  }

  return config;
});