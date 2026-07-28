#!/usr/bin/env node
/**
 * Fails the build if any text colour falls below WCAG AA contrast.
 *
 * This exists because six different hardcoded hexes had drifted into the
 * "secondary text" role across 21 files, four of them below AA and one at
 * 1.55:1 on 11px text — effectively invisible. Nothing was watching, so it
 * accumulated quietly. See --color-label / --color-meta in main.css.
 *
 * Colours are checked against every surface in the theme, because a given
 * class can appear on any of them and we cannot tell statically which one it
 * lands on. That is deliberately strict: it means a colour only passes if it
 * is readable everywhere it could plausibly be used.
 *
 * Run: npm run check:contrast
 */
import { readFileSync, readdirSync, statSync } from 'node:fs'
import { join, relative } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = join(fileURLToPath(new URL('.', import.meta.url)), '..')
const CSS = join(root, 'app/assets/css/main.css')
const SCAN_DIR = join(root, 'app')

/** WCAG AA for body text. Large text (>=24px, or >=18.66px bold) may use 3.0,
 *  but we cannot infer rendered size statically, so the stricter bar applies. */
const MIN_RATIO = 4.5

/** Background tokens text may sit on. */
const SURFACE_TOKENS = ['bg', 'surface', 'surface-2', 'surface-3']

/** Documented exceptions. Add an entry only with a reason — an empty list is
 *  the goal. Shape: { value: '#rrggbb', reason: '...' } */
const ALLOWLIST = []

function parseTokens(css) {
  const tokens = new Map()
  for (const m of css.matchAll(/--color-([\w-]+):\s*(#[0-9a-fA-F]{3,8})/g)) {
    tokens.set(m[1], m[2].toLowerCase())
  }
  return tokens
}

function expand(hex) {
  const h = hex.replace('#', '')
  return h.length === 3 ? h.split('').map(c => c + c).join('') : h.slice(0, 6)
}

function luminance(hex) {
  const h = expand(hex)
  const channels = [0, 2, 4].map((i) => {
    const c = parseInt(h.slice(i, i + 2), 16) / 255
    return c <= 0.03928 ? c / 12.92 : ((c + 0.055) / 1.055) ** 2.4
  })
  return 0.2126 * channels[0] + 0.7152 * channels[1] + 0.0722 * channels[2]
}

function contrast(a, b) {
  const [la, lb] = [luminance(a), luminance(b)]
  return (Math.max(la, lb) + 0.05) / (Math.min(la, lb) + 0.05)
}

function walk(dir, out = []) {
  for (const entry of readdirSync(dir)) {
    const full = join(dir, entry)
    if (statSync(full).isDirectory()) walk(full, out)
    else if (full.endsWith('.vue')) out.push(full)
  }
  return out
}

const css = readFileSync(CSS, 'utf8')
const tokens = parseTokens(css)

const surfaces = SURFACE_TOKENS
  .map(name => ({ name, value: tokens.get(name) }))
  .filter(s => s.value)

if (!surfaces.length) {
  console.error(`No surface tokens found in ${relative(root, CSS)} — expected ${SURFACE_TOKENS.join(', ')}`)
  process.exit(1)
}

// Utility patterns are static and the captured name is then looked up in the
// token map. Building a RegExp from the token names would work — they are
// parsed as [\w-]+ from our own stylesheet, so no metacharacters can appear —
// but a literal pattern plus a membership check is simpler and removes the
// dynamic construction entirely.
const tokenPattern = /\btext-([\w-]+)\b/g
const arbitraryPattern = /\btext-\[(#[0-9a-fA-F]{3,8})\]/g

const bgTokenPattern = /\bbg-([\w-]+)\b/g
const bgArbitraryPattern = /\bbg-\[(#[0-9a-fA-F]{3,8})\]/

/**
 * Text is checked against every theme surface by default, since we cannot tell
 * statically which one it lands on. But when a class string pairs the text
 * colour with its own background — `class="bg-aurora text-bg"` — that pairing
 * is known, so check against it instead. Without this, deliberately inverted
 * text on an accent background reads as a failure when it is in fact the
 * highest-contrast text on the page.
 */
function backgroundsFor(classString) {
  const arb = classString.match(bgArbitraryPattern)
  if (arb) return [{ name: `[${arb[1]}]`, value: arb[1].toLowerCase() }]
  for (const tok of classString.matchAll(bgTokenPattern)) {
    const value = tokens.get(tok[1])
    if (value) return [{ name: tok[1], value }]
  }
  return surfaces
}

const usage = new Map() // hex -> { count, files:Set, label, pairedOnly }
function record(hex, file, label, bgs) {
  const key = expand(hex).toLowerCase()
  if (key.length !== 6) return // skip alpha/oddities we cannot reason about
  const full = `#${key}`
  if (!usage.has(full)) usage.set(full, { count: 0, files: new Set(), label, worst: Infinity, worstBg: null })
  const e = usage.get(full)
  e.count++
  e.files.add(relative(root, file))
  if (label && !e.label) e.label = label
  for (const bg of bgs) {
    const r = contrast(full, bg.value)
    if (r < e.worst) {
      e.worst = r
      e.worstBg = bg.name
    }
  }
}

/**
 * Every text- occurrence in the file is counted, so nothing is missed. The
 * background is then *upgraded* from "any surface" to a known pairing only when
 * the occurrence sits inside a class attribute that also carries a bg- utility.
 *
 * Counting globally rather than walking quoted strings matters: pairing quotes
 * naively desynchronises on apostrophes in prose ("don't"), which silently
 * swallowed real occurrences in an earlier version of this script.
 */
const classAttrPattern = /(?::|v-bind:)?class\s*=\s*(?:"([^"]*)"|'([^']*)')/gs

function spansFor(src) {
  const spans = []
  for (const m of src.matchAll(classAttrPattern)) {
    const body = m[1] ?? m[2] ?? ''
    const bgs = backgroundsFor(body)
    if (bgs !== surfaces) spans.push({ start: m.index, end: m.index + m[0].length, bgs })
  }
  return spans
}

function bgsAt(spans, index) {
  return spans.find(s => index >= s.start && index < s.end)?.bgs ?? surfaces
}

for (const file of walk(SCAN_DIR)) {
  const src = readFileSync(file, 'utf8')
  const spans = spansFor(src)
  for (const m of src.matchAll(arbitraryPattern)) record(m[1], file, null, bgsAt(spans, m.index))
  for (const m of src.matchAll(tokenPattern)) {
    const value = tokens.get(m[1])
    if (value) record(value, file, m[1], bgsAt(spans, m.index))
  }
}

const allowed = new Set(ALLOWLIST.map(a => a.value.toLowerCase()))
const failures = []
const rows = []

for (const [hex, info] of [...usage.entries()].sort((a, b) => b[1].count - a[1].count)) {
  const worst = info.worst
  const ok = worst >= MIN_RATIO || allowed.has(hex)
  rows.push({ hex, info, worst, ok, exempt: allowed.has(hex) && worst < MIN_RATIO })
  if (!ok) failures.push({ hex, info, worst })
}

function pad(s, n) {
  return String(s).padEnd(n)
}
console.log(`Text contrast — WCAG AA ${MIN_RATIO}:1, worst case across ${surfaces.length} surfaces\n`)
console.log(`${pad('colour', 10)} ${pad('token', 12)} ${pad('uses', 6)} ${pad('worst', 8)} status`)
console.log('-'.repeat(52))
for (const r of rows) {
  const status = r.exempt ? 'ALLOWED' : r.ok ? 'ok' : 'FAIL'
  console.log(`${pad(r.hex, 10)} ${pad(r.info.label ?? '—', 12)} ${pad(r.info.count, 6)} ${pad(r.worst.toFixed(2), 8)} ${status}`)
}

if (failures.length) {
  console.error(`\n${failures.length} text colour(s) below WCAG AA (${MIN_RATIO}:1):\n`)
  for (const f of failures) {
    console.error(`  ${f.hex}${f.info.label ? ` (${f.info.label})` : ''} — ${f.worst.toFixed(2)}:1 against ${f.info.worstBg}, ${f.info.count} use(s)`)
    for (const file of [...f.info.files].sort().slice(0, 6)) console.error(`      ${file}`)
    if (f.info.files.size > 6) console.error(`      …and ${f.info.files.size - 6} more`)
  }
  console.error(`\nUse the --color-label (7.0:1) or --color-meta (4.6:1) tokens from main.css`)
  console.error(`rather than a new hardcoded hex. If an exception is genuinely warranted,`)
  console.error(`add it to ALLOWLIST in ${relative(root, fileURLToPath(import.meta.url))} with a reason.`)
  process.exit(1)
}

console.log(`\nAll ${rows.length} text colours pass (${[...usage.values()].reduce((n, e) => n + e.count, 0)} occurrences).`)
