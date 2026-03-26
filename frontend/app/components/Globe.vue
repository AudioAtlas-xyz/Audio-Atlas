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

          const result = await fetch('../assets/data/ne_110m_admin_0_countries.geojson')
          const countries = await result.json();

          const colorScale = scaleSequentialSqrt(interpolateYlOrRd);
          const maxVal = Math.max(...countries.features.map(getVal));
          colorScale.domain([0, maxVal]);

            const myGlobe = Globe(globeDiv.value)
              .globeImageUrl('https://raw.githubusercontent.com/annanassen/Ricart-Argawala/main/discoballtext.jpg')
              //.bumpImageUrl('https://unpkg.com/three-globe@2.45.1/example/img/earth-topology.png')
              .backgroundImageUrl('https://raw.githubusercontent.com/annanassen/Ricart-Argawala/main/discoballtext.jpg')
              .lineHoverPrecision(0)
              .polygonsData(countries.features.filter(d => d.properties.ISO_A2 !== 'AQ'))
              .polygonAltitude(0.06)
              .polygonCapColor(feat => colorScale(getVal(feat)))
              .polygonSideColor(() => 'rgba(0, 100, 0, 0.15)')
              .polygonStrokeColor(() => '#111')
              .polygonLabel(({properties: d}) => `
                <b>${d.ADMIN} (${d.ISO_A2}):</b> <br />
                GDP: <i>${d.GDP_MD_EST}</i> M$<br/>
                Population: <i>${d.POP_EST}</i>
              `)
              .onPolygonHover(hoverD => myGlobe
                .polygonAltitude(d => d === hoverD ? 0.12 : 0.06)
                .polygonCapColor(d => d === hoverD ? 'steelblue' : colorScale(getVal(d)))
              )
              .polygonsTransitionDuration(300);

            /*
            const globeMaterial = myGlobe.globeMaterial();
            globeMaterial.bumpScale = 10;

            new THREE.TextureLoader().load('//cdn.jsdelivr.net/npm/three-globe/example/img/earth-water.png',
              texture => {
                globeMaterial.specularMap = texture;
                globeMaterial.specular = new THREE.Color('grey');
                globeMaterial.shininess = 15;
              });

            const directionalLight = myGlobe.lights().find(light => light.type === 'DirectionalLight');
            if (directionalLight) {
              directionalLight.position.set(1, 1, 1);
            }
             */
          });
    return {
      globeDiv,
    };
  },
};
</script>

