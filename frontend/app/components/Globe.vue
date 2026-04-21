<template>
  <div
    ref="globeDiv"
    class="globe"
  />
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import * as THREE from 'three'

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
  }
})

const emit = defineEmits(['country-click'])

const setControlsEnabled = (enabled) => {
  const controls = globeInstance?.controls?.()
  if (!controls) return

  controls.enabled = enabled

  // Spin slowly during intro, stop when controls unlock
  controls.autoRotate = !enabled
  controls.autoRotateSpeed = 0.4
}

const povKey = (pov) => `${pov.lat}|${pov.lng}|${pov.altitude}`

onMounted(async () => {
  const Globe = (await import('globe.gl')).default

  const [countriesRes, centroidsRes] = await Promise.all([
    fetch('/countries.geojson'),
    fetch('/tiny-country-markers.json')
  ])

  const countries = await countriesRes.json()
  const tinyCountries = await centroidsRes.json()

  const countryColors = new Map()
  const baseHue = 135
  const range = 55
  const polygonAltitude = 0.006
  const hoverPolygonAltitude = 0.016

  const getCountryIso = feature => feature.properties.iso_a2
  const getCountryName = feature => feature.properties.admin ?? feature.properties.name
  const getCountryPopulation = feature => feature.properties.pop_est

  const features = countries.features.filter((f) => {
    const iso = getCountryIso(f)
    return iso && iso !== 'AQ' && iso !== '-99'
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

  globeInstance = Globe()(globeDiv.value)
    .globeImageUrl('/2_no_clouds_8k.jpg')
    .backgroundImageUrl('https://unpkg.com/three-globe@2.45.2/example/img/night-sky.png')
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
      Population: <i>${getCountryPopulation(feature)?.toLocaleString?.() ?? getCountryPopulation(feature)}</i>
    `)
    .pointsData(tinyCountries)
    .pointLat(d => d.lat)
    .pointLng(d => d.lng)
    .pointAltitude(0.007)
    .pointRadius(0.26)
    .pointResolution(8)
    .pointColor(() => 'rgba(120, 216, 255, 0.42)')
    .pointLabel(d => `<b>${d.name} (${d.iso})</b>`)
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
      const isoCode = getCountryIso(feature)
      const name = getCountryName(feature)

      console.log('Clicked polygon:', name, isoCode)
      emit('country-click', { isoCode, name })
    })
    .onPointClick((d) => {
      console.log('Clicked tiny country marker:', d.name, d.iso)
      emit('country-click', { isoCode: d.iso, name: d.name })
    })
    .polygonsTransitionDuration(250)
    .pointsTransitionDuration(0)
    .pointOfView(initialPov)
    .globeOffset(props.globeOffset)

  setControlsEnabled(props.introComplete)
  
  const controls = globeInstance.controls()
  controls.autoRotate = !props.introComplete
  controls.autoRotateSpeed = 0.4

  const globeMaterial = globeInstance.globeMaterial()
  globeMaterial.color = new THREE.Color('#b7f3ff')
  globeMaterial.emissive = new THREE.Color('#061f2a')
  globeMaterial.emissiveIntensity = 0.18
  globeMaterial.specular = new THREE.Color('#7fd4ff')
  globeMaterial.shininess = 8
  globeMaterial.needsUpdate = true

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
    setControlsEnabled(introComplete)
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
</style>
