<template>
  <div
    ref="globeDiv"
    class="globe"
    :class="{ 'globe--locked': !introComplete }"
  />
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'

const { api } = useApi()

defineOptions({
  name: 'AudioGlobe'
})

const globeDiv = ref(null)
let globeInstance = null
let resizeObserver = null
let lastPovKey = ''
let lastWidth = 0
let lastHeight = 0

const props = defineProps({
  introComplete: {
    type: Boolean,
    default: true
  },
  initialPointOfView: {
    type: Object,
    default: () => ({
      lat: 0,
      lng: 0,
      altitude: 2.5
    })
  },
  globeOffset: {
    type: Array,
    default: () => [0, 0]
  },
  pointOfView: {
    type: Object,
    default: null
  },
  introSpinSpeed: {
    type: Number,
    default: 0.4
  }
})

const emit = defineEmits(['country-click'])

const setInteractionEnabled = (enabled) => {
  const controls = globeInstance?.controls?.()
  if (controls) {
    controls.enabled = true
    controls.autoRotate = !enabled
    controls.autoRotateSpeed = props.introSpinSpeed
  }

  globeInstance?.enablePointerInteraction?.(enabled)
}

const povKey = pov => `${pov.lat}|${pov.lng}|${pov.altitude}`

const RETRY_DELAYS_MS = [2000, 4000, 8000]

async function fetchGlobeData() {
  const [countriesRes, centroidsRes, genreCounts] = await Promise.all([
    fetch('/countries.geojson'),
    fetch('/tiny-country-markers.json'),
    api('/countries')
  ])
  const countries = await countriesRes.json()
  const tinyCountries = await centroidsRes.json()
  return { countries, tinyCountries, genreCounts }
}

onMounted(async () => {
  // Fire both in parallel: module load + data fetch
  const globeImport = import('globe.gl')

  let data = null
  for (let attempt = 0; attempt <= RETRY_DELAYS_MS.length; attempt++) {
    try {
      data = await fetchGlobeData()
      break
    }
    catch (err) {
      if (attempt < RETRY_DELAYS_MS.length) {
        await new Promise(resolve => setTimeout(resolve, RETRY_DELAYS_MS[attempt]))
        if (!globeDiv.value) return
      }
      else {
        console.error('[Globe] Failed to load data after retries:', err)
        return
      }
    }
  }

  const [Globe, THREE] = await Promise.all([
    globeImport.then(m => m.default),
    import('three')
  ])

  const { countries, tinyCountries, genreCounts } = data

  const countryColors = new Map()
  const baseHue = 135
  const range = 55
  const polygonAltitude = 0.006
  const hoverPolygonAltitude = 0.016

  const getCountryIso = feature => feature.properties.iso_a3
  const getCountryName = feature => feature.properties.admin ?? feature.properties.name
  const getCountryGenreCount = (feature) => {
    const iso = getCountryIso(feature)
    return countryGenreCount.get(iso) ?? 0
  }

  const countryGenreCount = new Map(Object.entries(genreCounts))

  const features = countries.features.filter((f) => {
    const iso = getCountryIso(f)
    return iso && iso !== 'ATA' && iso !== 'ATF' && iso !== '-99'
  })

  features.forEach((feat) => {
    const iso = getCountryIso(feat)
    const hue = baseHue + Math.random() * range

    countryColors.set(
      iso,
      {
        base: `hsla(${hue}, 72%, 49%, 0.58)`,
        hover: `hsla(${hue}, 92%, 64%, 0.84)`
      }
    )
  })

  const initialPov = props.pointOfView ?? props.initialPointOfView
  lastPovKey = povKey(initialPov)

  const emitCountryClick = (country) => {
    emit('country-click', country)
  }

  globeInstance = Globe()(globeDiv.value)
    .globeImageUrl('/earth-1k.webp')
    .backgroundImageUrl('/night-sky.png')
    .showAtmosphere(true)
    .atmosphereColor('#78d8ff')
    .atmosphereAltitude(0.24)
    .lineHoverPrecision(0)
    .polygonsData(features)
    .polygonAltitude(polygonAltitude)
    .polygonCapCurvatureResolution(4)
    .polygonCapColor(feat => countryColors.get(getCountryIso(feat))?.base ?? 'rgba(34, 197, 94, 0.58)')
    .polygonSideColor(() => 'rgba(11, 95, 54, 0.08)')
    .polygonStrokeColor(() => 'rgba(216, 255, 234, 0.55)')
    .polygonLabel(feature => `
      <b>${getCountryName(feature)} (${getCountryIso(feature)}):</b><br/>
          Genres: <i>${getCountryGenreCount(feature)}</i>
    `)
    .pointsData(tinyCountries)
    .pointLat(d => d.lat)
    .pointLng(d => d.lng)
    .pointAltitude(0.007)
    .pointRadius(0.26)
    .pointResolution(8)
    .pointColor(() => 'rgba(120, 216, 255, 0.42)')
    .pointLabel(d => `<b>${d.name} (${d.iso_a3})</b>`)
    .onPolygonHover((hoverD) => {
      globeInstance
        .polygonAltitude(d => (d === hoverD ? hoverPolygonAltitude : polygonAltitude))
        .polygonCapColor(d =>
          d === hoverD
            ? countryColors.get(getCountryIso(d))?.hover ?? 'rgba(255, 229, 120, 0.84)'
            : countryColors.get(getCountryIso(d))?.base ?? 'rgba(34, 197, 94, 0.58)'
        )
    })
    .onPolygonClick((feature) => {
      emitCountryClick({
        name: getCountryName(feature),
        isoA3: getCountryIso(feature)
      })
    })
    .onPointClick((d) => {
      emitCountryClick({
        name: d.name,
        isoA3: d.iso_a3
      })
    })
    .polygonsTransitionDuration(250)
    .pointsTransitionDuration(0)
    .pointOfView(initialPov)
    .globeOffset(props.globeOffset)

  setInteractionEnabled(props.introComplete)

  const globeMaterial = globeInstance.globeMaterial()
  globeMaterial.color = new THREE.Color('#b7f3ff')
  globeMaterial.emissive = new THREE.Color('#061f2a')
  globeMaterial.emissiveIntensity = 0.18
  globeMaterial.specular = new THREE.Color('#7fd4ff')
  globeMaterial.shininess = 8
  globeMaterial.needsUpdate = true

  // Upgrade to 4k texture in the background once the globe is painted
  const img = new Image()
  img.onload = () => {
    globeInstance?.globeImageUrl('/earth-4k.webp')
  }
  img.src = '/earth-4k.webp'

  // Track initial size
  lastWidth = globeDiv.value.clientWidth
  lastHeight = globeDiv.value.clientHeight

  // Only resize when dimensions actually changed to avoid infinite loops.
  // ResizeObserver fires when we call .width()/.height(), so without
  // this guard it would trigger itself endlessly.
  resizeObserver = new ResizeObserver(() => {
    if (!globeInstance || !globeDiv.value) return

    const w = globeDiv.value.clientWidth
    const h = globeDiv.value.clientHeight

    if (w === lastWidth && h === lastHeight) return

    lastWidth = w
    lastHeight = h
    globeInstance.width(w)
    globeInstance.height(h)
  })
  resizeObserver.observe(globeDiv.value)
})

onBeforeUnmount(() => {
  resizeObserver?.disconnect()
  globeInstance?.pauseAnimation?.()
})

watch(
  () => props.introComplete,
  (introComplete) => {
    setInteractionEnabled(introComplete)
  }
)

watch(
  () => props.globeOffset,
  (globeOffset) => {
    globeInstance?.globeOffset?.(globeOffset)
  },
  { deep: true }
)

watch(
  () => props.pointOfView,
  (pointOfView) => {
    if (!pointOfView || !globeInstance) {
      return
    }

    const key = povKey(pointOfView)

    if (key === lastPovKey) {
      return
    }

    lastPovKey = key

    const transitionMs = props.introComplete ? 600 : 0
    globeInstance.pointOfView(pointOfView, transitionMs)
  },
  { deep: true }
)
</script>

<style scoped>
.globe {
  width: 100%;
  height: 100%;
}

.globe--locked {
  pointer-events: none;
}
</style>
