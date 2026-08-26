// Copies the Designsystemet stylesheets into the folder the Razor views serve them from.
// Runs automatically after `npm install`, and is run explicitly by the Dockerfile.
import { copyFileSync, mkdirSync } from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const packageDir = dirname(fileURLToPath(import.meta.url));

const defaultOutputDir = join(
  packageDir,
  '..',
  'backend',
  'Altinn.Receipt',
  'wwwroot',
  'receipt',
  'css',
  'designsystemet',
);

const outputDir = process.argv[2] ? resolve(process.argv[2]) : defaultOutputDir;

const stylesheets = [
  ['@digdir/designsystemet-theme/brand/altinn.css', 'theme.css'],
  ['@digdir/designsystemet-css/dist/src/index.css', 'components.css'],
];

mkdirSync(outputDir, { recursive: true });

for (const [source, target] of stylesheets) {
  const sourcePath = join(packageDir, 'node_modules', source);
  const targetPath = join(outputDir, target);
  copyFileSync(sourcePath, targetPath);
  console.log(`Copied ${source} to ${targetPath}`);
}
