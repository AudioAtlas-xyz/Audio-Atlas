<template>
    <div :class="$style.nav">

      <NuxtLink to="/">
      <div :class="$style.audioatlasHomeButton">
        <div :class="$style.button">AudioAtlas</div>
      </div>
      </NuxtLink>

      <NuxtLink to = "/">
        <div :class="$style.exploreLink">
          <div :class="$style.label">EXPLORE</div>
        </div>
      </NuxtLink>

      <NuxtLink to="/">
      <div :class="$style.exploreLink">
        <div :class="$style.label">CONTRIBUTE</div>
      </div>
      </NuxtLink>

      <NuxtLink to="/">
      <div :class="$style.exploreLink">
        <div :class="$style.label">ABOUT</div>
      </div>
      </NuxtLink>



      <NuxtLink to="/">
      <div :class="$style.divnavRight">
        <div :class="$style.signInLink">
          <div :class="$style.button2">Sign In</div>
        </div>
      </div>
      </NuxtLink>

    </div>
</template>
<style  module>.nav {
  width: 100%;
  height: 56px;
  position: relative;
  backdrop-filter: blur(8px);
  background-color: rgba(7, 8, 15, 0.85);
  border-bottom: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0px 48px;
  gap: 20px;
  text-align: left;
  font-size: 10.1px;
  color: #373d5a;
  font-family: 'Space Mono';
}
.audioatlasHomeButton {
  border-radius: 6px;
  display: flex;
  align-items: center;
  padding: 8px 12px;
  font-size: 16px;
  color: #3de8c8;
  font-family: Space_Grotesk;
}
.button {
  position: relative;
  line-height: 24px;
  font-weight: 500;
}
.exploreLink {
  display: flex;
  align-items: center;
  justify-content: center;
}
.label {
  position: relative;
  letter-spacing: 0.61px;
  line-height: 16.13px;
}
.divnavRight {
  display: flex;
  align-items: center;
  font-size: 12px;
  color: #07080f;
  font-family: Space_Grotesk;
}
.signInLink {
  border-radius: 6px;
  background-color: #3de8c8;
  display: flex;
  align-items: center;
  padding: 4px 8px;
}
.button2 {
  position: relative;
  line-height: 16px;
  font-weight: 500;
}

</style>
<script setup lang="ts">
</script>
