<script setup lang="ts">

</script>

<template>
  <div :class="$style.genrepanel">
    <div :class="$style.divheroLeft">
      <div :class="$style.h1heroTitle">
        <b :class="$style.heroTitle">Afrobeat</b>
      </div>
      <div :class="$style.countrybadgerow">
        <div :class="$style.countryBadge">
          <div :class="$style.nigeria">Nigeria</div>
        </div>
        <div :class="$style.countryBadge">
          <div :class="$style.nigeria">Ghana</div>
        </div>
      </div>
      <div :class="$style.genrestats">
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">15+</b>
            <div :class="$style.loremIpsumDolor">Related Genres</div>
          </div>
        </div>
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">3</b>
            <div :class="$style.loremIpsumDolor">Contributors</div>
          </div>
        </div>
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">12+ </b>
            <div :class="$style.loremIpsumDolor">Instruments</div>
          </div>
        </div>
      </div>
      <div :class="$style.divheroActions">
        <div :class="$style.contributeAGenreButton">
          <div :class="$style.text">Suggest Edits</div>
        </div>
      </div>
      <div :class="$style.divnavCrumb">
        <div :class="$style.breadcrumb">
          <div :class="$style.link">
            <div :class="$style.label">Explore</div>
          </div>
          <img :class="$style.chevronRightIcon" alt="" />
          <div :class="$style.dropdownmenu">
            <div :class="$style.buttonneutral">
              <img :class="$style.ellipsisIcon" alt="" />
            </div>
          </div>
          <img :class="$style.chevronRightIcon" alt="" />
          <div :class="$style.link">
            <div :class="$style.label">Nigeria</div>
          </div>
          <img :class="$style.chevronRightIcon" alt="" />
          <div :class="$style.link3">
            <b :class="$style.label3">Afrobeat</b>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<style module>.divheroLeft {
  width: 100%;
  height: 19.138rem;
  position: relative;
  text-align: left;
  font-size: 2.975rem;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}
.h1heroTitle {
  position: absolute;
  width: 100%;
  top: 1.563rem;
  right: 0rem;
  left: 0rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  flex-shrink: 0;
}
.heroTitle {
  position: relative;
  letter-spacing: -1.44px;
  line-height: 3rem;
}
.countrybadgerow {
  position: absolute;
  width: calc(100% + 21.2px);
  top: 7.563rem;
  right: -1.325rem;
  left: 0rem;
  display: flex;
  align-items: flex-start;
  flex-wrap: wrap;
  align-content: flex-start;
  gap: 0rem 0.312rem;
  flex-shrink: 0;
  text-align: center;
  font-size: 0.625rem;
  font-family: Space_Grotesk;
}
.countryBadge {
  border-radius: 999px;
  background-color: #07080f;
  border: 1px solid #7a84a8;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0.25rem 0.375rem;
}
.nigeria {
  position: relative;
  line-height: 0.75rem;
  font-weight: 500;
}
.genrestats {
  position: absolute;
  width: calc(100% - 442.8px);
  top: 10.375rem;
  right: 27.3rem;
  left: 0.375rem;
  border-radius: 4px;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  height: 5rem;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 1.875rem;
  font-family: DM_Sans;
}
.prosecard {
  height: 6.25rem;
  width: 12rem;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 1.5rem;
  flex-shrink: 0;
}
.titleAndDescription {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.25rem;
  flex-shrink: 0;
}
.title {
  align-self: stretch;
  position: relative;
  line-height: 2.25rem;
}
.loremIpsumDolor {
  align-self: stretch;
  position: relative;
  font-size: 0.75rem;
  line-height: 1.25rem;
  font-family: Space_Grotesk;
  color: #252c48;
}
.divheroActions {
  position: absolute;
  top: 16.911rem;
  left: 0rem;
  width: 41.55rem;
  display: flex;
  align-items: flex-start;
  flex-wrap: wrap;
  align-content: flex-start;
  flex-shrink: 0;
  text-align: center;
  font-size: 0.819rem;
  color: #07080f;
}
.contributeAGenreButton {
  border-radius: 3px;
  background-color: #3de8c8;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 0.712rem 1.5rem;
}
.text {
  position: relative;
  letter-spacing: 0.39px;
  font-weight: 500;
}
.divnavCrumb {
  position: absolute;
  top: 5.375rem;
  left: 0rem;
  display: flex;
  align-items: center;
  flex-shrink: 0;
  font-size: 0.875rem;
  color: #252c48;
  font-family: Space_Grotesk;
}
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}
.link {
  display: flex;
  align-items: center;
  justify-content: center;
}
.label {
  position: relative;
  line-height: 1.25rem;
  font-weight: 500;
}
.chevronRightIcon {
  height: 1.25rem;
  width: 1.25rem;
  position: relative;
}
.dropdownmenu {
  height: 1.5rem;
  width: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.buttonneutral {
  border-radius: 6px;
  display: flex;
  align-items: center;
  padding: 0.25rem;
}
.ellipsisIcon {
  height: 1rem;
  width: 1rem;
  position: relative;
}
.link3 {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #3de8c8;
}
.label3 {
  position: relative;
  line-height: 1.25rem;
}
</style>
