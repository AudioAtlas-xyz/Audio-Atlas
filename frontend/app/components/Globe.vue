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
          const { scaleSequentialSqrt } = await import("d3-scale");
          const { interpolateYlOrRd } = await import("d3-scale-chromatic");
          const getVal = feat => feat.properties.GDP_MD_EST / Math.max(1e5, feat.properties.POP_EST);
          const result = await fetch('ne_110m_admin_0_countries.geojson');
          const countries = await result.json();
          const colorScale = scaleSequentialSqrt(interpolateYlOrRd);
          const maxVal = Math.max(...countries.features.map(getVal));
          colorScale.domain([0, maxVal]);

          const countryColors = new Map();
          const baseHue = 120;
          const range = 90;

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
             .hexPolygonsData(countries.features.filter(d => d.properties.ISO_A2 !== 'AQ'))
              .hexPolygonAltitude(0.06)
              .hexPolygonResolution(3)
              .hexPolygonMargin(0.2)
              .hexPolygonUseDots(true)
              .hexPolygonColor(feat => countryColors.get(feat.properties.ISO_A2))
              .hexPolygonLabel(({properties: d}) => `
                <b>${d.ADMIN} (${d.ISO_A2}):</b> <br />
                Population: <i>${d.POP_EST}</i>
              `)
              .onHexPolygonHover(hoverD => myGlobe
                .hexPolygonAltitude(d => d === hoverD ? 0.12 : 0.06)
                .hexPolygonColor(d => d === hoverD ? '#FF69B4' : countryColors.get(d.properties.ISO_A2))
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
