// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import pdf from "astro-pdf";

// https://astro.build/config
export default defineConfig({
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
					label: 'Русская',
				},
			},
			sidebar: [
				{ slug: 'index' },
			],
		}),
		pdf({
			baseOptions: {
				path: '/pdf/[pathname].pdf',
				waitUntil: 'networkidle2',
				maxRetries: 2,
			},
			maxConcurrent: 2,
			pages: {
				'/en/index': true,
				'/ru/index': true,
			}
		})
	],
});
