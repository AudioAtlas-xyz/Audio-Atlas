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
<style  module>.genrepanel {
  width: 100%;
  position: relative;
  display: grid;
  box-sizing: border-box;
  grid-template-columns: 1fr;
  grid-template-rows: 306.1719970703125px;
  gap: 48px;
  text-align: left;
  font-size: 47.6px;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}
.divheroLeft {
  height: 306.2px;
  position: relative;
  grid-column: 1;
  grid-row: 1;
}
.h1heroTitle {
  position: absolute;
  width: 100%;
  top: 25px;
  right: 0px;
  left: 0px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  flex-shrink: 0;
}
.heroTitle {
  position: relative;
  letter-spacing: -1.44px;
  line-height: 48px;
}
.countrybadgerow {
  position: absolute;
  width: calc(100% + 21.2px);
  top: 121px;
  right: -21.2px;
  left: 0px;
  display: flex;
  align-items: flex-start;
  flex-wrap: wrap;
  align-content: flex-start;
  gap: 0px 5px;
  flex-shrink: 0;
  text-align: center;
  font-size: 10px;
  font-family: Space_Grotesk;
}
.countryBadge {
  border-radius: 999px;
  background-color: #07080f;
  border: 1px solid #7a84a8;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4px 6px;
}
.nigeria {
  position: relative;
  line-height: 12px;
  font-weight: 500;
}
.genrestats {
  position: absolute;
  width: calc(100% - 442.8px);
  top: 166px;
  right: 436.8px;
  left: 6px;
  border-radius: 4px;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  height: 80px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 30px;
  font-family: DM_Sans;
}
.prosecard {
  height: 100px;
  width: 192px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 24px;
  flex-shrink: 0;
}
.titleAndDescription {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 4px;
  flex-shrink: 0;
}
.title {
  align-self: stretch;
  position: relative;
  line-height: 36px;
}
.loremIpsumDolor {
  align-self: stretch;
  position: relative;
  font-size: 12px;
  line-height: 20px;
  font-family: Space_Grotesk;
  color: #252c48;
}
.divheroActions {
  position: absolute;
  top: 270.58px;
  left: 0px;
  width: 664.8px;
  display: flex;
  align-items: flex-start;
  flex-wrap: wrap;
  align-content: flex-start;
  flex-shrink: 0;
  text-align: center;
  font-size: 13.1px;
  color: #07080f;
}
.contributeAGenreButton {
  border-radius: 3px;
  background-color: #3de8c8;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 11.4px 24px;
}
.text {
  position: relative;
  letter-spacing: 0.39px;
  font-weight: 500;
}
.divnavCrumb {
  position: absolute;
  top: 86px;
  left: 0px;
  display: flex;
  align-items: center;
  flex-shrink: 0;
  font-size: 14px;
  color: #252c48;
  font-family: Space_Grotesk;
}
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 6px;
}
.link {
  display: flex;
  align-items: center;
  justify-content: center;
}
.label {
  position: relative;
  line-height: 20px;
  font-weight: 500;
}
.chevronRightIcon {
  height: 20px;
  width: 20px;
  position: relative;
}
.dropdownmenu {
  height: 24px;
  width: 24px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.buttonneutral {
  border-radius: 6px;
  display: flex;
  align-items: center;
  padding: 4px;
}
.ellipsisIcon {
  height: 16px;
  width: 16px;
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
  line-height: 20px;
}

</style>
