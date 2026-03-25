<template>
    <div ref="globeDiv">
    </div>
</template>

<script>
import * as THREE from "three";
import {ref, onMounted} from "vue";
export default {
    setup() {
        const globeDiv = ref(null);

        onMounted(async () => {
            const Globe = (await import("globe.gl")).default;

            const myGlobe = Globe()(globeDiv.value)
             .globeImageUrl('https://raw.githubusercontent.com/annanassen/Ricart-Argawala/main/discoballtext.jpg')
             .bumpImageUrl('https://unpkg.com/three-globe@2.45.1/example/img/earth-topology.png')
             .backgroundImageUrl('https://raw.githubusercontent.com/annanassen/Ricart-Argawala/main/partyscene.webp');

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
        });
    return {
      globeDiv,
    };
  },
};
</script>

