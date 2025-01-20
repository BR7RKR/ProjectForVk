// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	site: 'https://br7rkr.github.io',
	base: '/ProjectForVk',
	integrations: [
		starlight({
			title: {
				en: 'ProjectForVk',
				ru: 'Проект для вконтакте',
			},
			defaultLocale: 'root',
			locales: {
				root: {
					label: 'English',
					lang: 'en',
				},
				ru: {
					label: 'Русский',
				},
			},
		}),
	],
});
