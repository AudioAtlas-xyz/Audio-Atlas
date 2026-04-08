<template>
    <div ref="globeDiv">
    </div>
</template>

<script>
import * as THREE from "three";
import {ref, onMounted} from "vue";
export default {
    setup() {
       // globeDiv needs to be here or the compiler can't find the variable.
      const globeDiv = ref(null);

        onMounted(async () => {
          const Globe = (await import("globe.gl")).default;
          const result = await fetch('ne_110m_admin_0_countries.geojson');
          const countries = await result.json();

          const countryColors = new Map();
          const baseHue = 120;
          const range = 90;

          //randomly sets a color to each country, given a range and a basehue
          countries.features.forEach((feat) => {
            countryColors.set(
              feat.properties.ISO_A2, `hsl(${ baseHue + Math.random() * range}, 67%, 50%)`
            )
          })

          const myGlobe = Globe()(globeDiv.value)
             .globeImageUrl('https://unpkg.com/three-globe@2.45.2/example/img/earth-night.jpg')
             //.bumpImageUrl('https://unpkg.com/three-globe@2.45.1/example/img/earth-topology.png')
             .backgroundImageUrl('https://unpkg.com/three-globe@2.45.2/example/img/night-sky.png')
             .lineHoverPrecision(0)
             .polygonsData(countries.features.filter(d => d.properties.ISO_A2 !== 'AQ'))
              .polygonAltitude(0.06)
              .polygonCapColor(feat => countryColors.get(feat.properties.ISO_A2))
              .polygonSideColor(() => 'rgba(0, 100, 0, 0.15)')
              .polygonStrokeColor(() => '#111')
              .polygonLabel(({properties: d}) => `
                <b>${d.ADMIN} (${d.ISO_A2}):</b> <br />
                Population: <i>${d.POP_EST}</i>
              `)
              .onPolygonHover(hoverD => myGlobe
                .polygonAltitude(d => d === hoverD ? 0.12 : 0.06)
                .polygonCapColor(d => d === hoverD ? '#FF69B4' : countryColors.get(d.properties.ISO_A2))
              )
              .polygonsTransitionDuration(300)
              .onPolygonClick(({ properties: d }) => {
              console.log('Clicked country:', d.ADMIN);
              //needs to be a panel
              alert(`You clicked on ${d.ADMIN}`);
              });


        });
    return {
      globeDiv,
    };
  },
};
</script>
