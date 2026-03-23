<template>
    <div ref="globeDiv"> 
    </div>
</template>

<script> 
import Globe from "globe.gl";
import * as THREE from "three";
import {ref, onMounted} from "vue";
export default {
    setup() {
        const globeDiv = ref(null);

        onMounted(() => {
            const myGlobe = Globe()(globeDiv.value)
             .globeImageUrl('//cdn.jsdelivr.net/npm/three-globe/example/img/earth-blue-marble.jpg')
             .bumpImageUrl('//cdn.jsdelivr.net/npm/three-globe/example/img/earth-topology.png')
             .backgroundImageUrl('//cdn.jsdelivr.net/npm/three-globe/example/img/night-sky.png');
 
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

