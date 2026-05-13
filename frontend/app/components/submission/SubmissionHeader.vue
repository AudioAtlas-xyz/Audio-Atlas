
<template>
  <div :class="$style.submitgenreheadings">
    		<div :class="$style.submitAGenre">Submit a genre</div>
    		<div :class="$style.helpGrowThe">Help grow the Audio Atlas. Your submission will be reviewed by a curator before it appears on the map.</div>
  </div>
</template>

<style module>
.submitgenreheadings {
  width: 50%;
  margin: 2rem auto;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  text-align: left;
  gap: 4px;
  font-size: 26px;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}

.submitAGenre {
  /*we can change the font size if it is to big*/
  font-size: 30px;
  font-weight: 500;
}

.helpGrowThe {
  font-size: 13px;
  line-height: 175%;
  font-weight: 300;
  color: #7a84a8;
  display: inline-block;
}

</style>
