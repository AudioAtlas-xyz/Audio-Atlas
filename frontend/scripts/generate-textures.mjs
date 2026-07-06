#!/usr/bin/env node
/**
 * Generates earth-1k.webp and earth-4k.webp from the 8K source JPEG.
 * Run once after cloning or whenever the source texture changes:
 *   node scripts/generate-textures.mjs
 */
import { createRequire } from 'module'
import { fileURLToPath } from 'url'
import path from 'path'

const require = createRequire(import.meta.url)
const sharp = require('sharp')

const __dirname = path.dirname(fileURLToPath(import.meta.url))
const root = path.resolve(__dirname, '..')

const source = path.join(root, 'assets', 'source', '2_no_clouds_8k.jpg')
const outDir = path.join(root, 'public')

const targets = [
  { width: 1024, name: 'earth-1k.webp' },
  { width: 4096, name: 'earth-4k.webp' }
]

for (const { width, name } of targets) {
  const out = path.join(outDir, name)
  await sharp(source)
    .resize(width)
    .webp({ quality: 80 })
    .toFile(out)
  const { size } = (await import('fs')).statSync(out)
  console.log(`✓ ${name}  ${(size / 1024).toFixed(0)} KB`)
}
