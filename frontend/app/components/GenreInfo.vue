<script setup lang="ts">
const { isSensitive, sensitivityDescription, pageDescription } = defineProps<{
  isSensitive: boolean
  sensitivityDescription?: string
  pageDescription: string
}>()
</script>

<template>
  <div :class="$style.genreinfo">
    <div :class="$style.divcontent">
      <div v-if= isSensitive :class="$style.culturalsensitivitynotice">
        <div :class="$style.spansensIconmargin">
          <div :class="$style.symbol">◎</div>
        </div>
        <div :class="$style.divsensBody">
          <div :class="$style.divsensTitle">
            <div :class="$style.noticeTitle">Cultural SENSITIVITY notice</div>
          </div>
          <div :class="$style.divsensText">
            <div :class="$style.noticeText">{{sensitivityDescription}}
            </div>
          </div>
        </div>
      </div>
      <div :class="$style.genreoverview">
        <div :class="$style.overview">
          <div :class="$style.sectionTitle">Overview</div>
        </div>
        <div :class="$style.divsensTitle">
          <div :class="$style.sectionText">{{pageDescription}}
          </div>
        </div>
      </div>
      <div :class="$style.sectiondivider" />
      <div :class="$style.relatedgenres">
        <div :class="$style.parentgenrecard">
          <div :class="$style.parentgenre">
            <div :class="$style.span">
              <div :class="$style.cardTitle">PareNT GENRES</div>
            </div>
            <div :class="$style.a" />
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
        </div>
        <div :class="$style.parentgenrecard">
          <div :class="$style.parentgenre">
            <div :class="$style.span">
              <div :class="$style.cardTitle">SUB GENRES</div>
            </div>
            <div :class="$style.a" />
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
        </div>
        <div :class="$style.parentgenrecard">
          <div :class="$style.divsideCardHead2">
            <div :class="$style.span3">
              <div :class="$style.cardTitle">SIMILAR GENRES</div>
            </div>
            <div :class="$style.span4" />
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
          <div :class="$style.prosecard">
            <div :class="$style.titleAndDescription">
              <div :class="$style.title">Genre name</div>
              <div :class="$style.loremIpsumDolor">Genre Countries</div>
            </div>
          </div>
        </div>
      </div>
      <div :class="$style.contributorprofilelink">
        <div :class="$style.seemorerelatedgenres">See more...</div>
      </div>
    </div>
    <div v-if=false :class="$style.divsidebar">
      <div :class="$style.sourcelist">
        <div :class="$style.divsideCardHead3">
          <div :class="$style.span3">
            <div :class="$style.cardTitle">Sources</div>
          </div>
        </div>
        <div :class="$style.divsideCardBody">
          <div :class="$style.div">
            <div :class="$style.source">
              <span>Stewart, J. (2004). </span>
              <span :class="$style.afropop">Afropop!</span>
              <span> Heinemann.</span>
            </div>
          </div>
          <div :class="$style.div2">
            <div :class="$style.source">
              <span>Allen, T. (2013). </span>
              <span :class="$style.afropop">Tony Allen: An Autobiography.</span>
              <span> Duke<br/>UP.</span>
            </div>
          </div>
          <div :class="$style.div2">
            <div :class="$style.source">
              <span>Veal, M. (2000). </span>
              <span :class="$style.afropop">Fela: The Life and Times of an<br/>African Musical Icon.</span>
            </div>
          </div>
          <div :class="$style.div4">
            <div :class="$style.source4">
              <span>Moore, C. (1982). </span>
              <span :class="$style.afropop">Fela Fela: This Bitch of a Life.<br/></span>
              <span>Allison & Busby.</span>
            </div>
          </div>
        </div>
      </div>
      <div :class="$style.sourcelist">
        <div :class="$style.divsideCardHead3">
          <div :class="$style.span3">
            <div :class="$style.cardTitle">Instruments</div>
          </div>
        </div>
        <div :class="$style.divsideCardBody2">
          <div :class="$style.divsideCardBody3">
            <div :class="$style.div">
              <div :class="$style.source">Drums</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Saxaphone</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Electric Piano</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Violin</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Flute</div>
            </div>
            <div :class="$style.div">
              <div :class="$style.source">Trumpet</div>
            </div>
            <div :class="$style.div">
              <div :class="$style.source">Cello</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Clarinet</div>
            </div>
            <div :class="$style.div2">
              <div :class="$style.source">Guitar</div>
            </div>
          </div>
          <div :class="$style.div14" />
        </div>
      </div>
    </div>
  </div>
</template>
<style  module>
.genreinfo {
  width: 100%;
  height: 73.875rem;
  position: relative;
  box-sizing: border-box;
  grid-template-columns: 1fr 320px;
  gap: 3rem;
  padding-left: 2.5rem;
  padding-right: 2.5rem;
  padding-top: 2rem;
  padding-bottom: 3rem;
  text-align: left;
  font-size: 0.875rem;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}
.divcontent {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 2.5rem;
  gap: 2rem;
  grid-column: 1;
  grid-row: 1;
  flex-shrink: 0;
}
.culturalsensitivitynotice {
  align-self: stretch;
  background: linear-gradient(135deg, #0a1a3a 0%, #1a3a6e 100%);
  border-left: 3px solid #3de8c8;
  display: flex;
  align-items: flex-start;
  padding: 1rem 1.25rem;
  gap: 0.75rem;
  border-radius: 0px;
}
.spansensIconmargin {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.062rem 0rem 0rem;
}
.symbol {
  position: relative;
  line-height: 0.875rem;
  font-weight: 300;
}
.divsensBody {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.187rem;
  min-width: 32.727rem;
  font-size: 0.6rem;
  color: #60b8ff;
  font-family: 'Space Mono';
}
.divsensTitle {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.noticeTitle {
  position: relative;
  letter-spacing: 1.15px;
  text-transform: uppercase;
}
.divsensText {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  font-size: 0.8rem;
  color: #507090;
  font-family: 'Space Grotesk';
}
.noticeText {
  position: relative;
  line-height: 1.32rem;
  font-weight: 300;
}
.genreoverview {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 0.5rem;
  gap: 0.681rem;
  font-size: 0.863rem;
  color: #7a84a8;
}
.overview {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 0.5rem;
  font-size: 0.581rem;
  color: #373d5a;
  font-family: 'Space Mono';
}
.sectionTitle {
  position: relative;
  letter-spacing: 1.49px;
  text-transform: uppercase;
}
.sectionText {
  position: relative;
  line-height: 1.591rem;
  font-weight: 300;
}
.sectiondivider {
  align-self: stretch;
  height: 0.063rem;
  position: relative;
  border-top: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  flex-shrink: 0;
}
.relatedgenres {
  align-self: stretch;
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  font-family: DM_Sans;
}
.parentgenrecard {
  flex: 1;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.parentgenre {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  align-items: center;
  padding: 0.6rem 1rem;
  gap: 7.906rem;
  font-size: 0.563rem;
  color: #373d5a;
  font-family: 'Space Mono';
}
.span {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  flex-shrink: 0;
}
.cardTitle {
  position: relative;
  letter-spacing: 1.25px;
  text-transform: uppercase;
}
.a {
  height: 0.875rem;
  width: 4rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 0.062rem;
  box-sizing: border-box;
  flex-shrink: 0;
}
.prosecard {
  width: 12rem;
  height: 4.875rem;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 1.5rem;
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
  line-height: 1rem;
  font-weight: 600;
}
.loremIpsumDolor {
  align-self: stretch;
  position: relative;
  line-height: 1.25rem;
  font-family: Space_Grotesk;
  color: #252c48;
}
.divsideCardHead2 {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.6rem 1rem;
  gap: 1.25rem;
  font-size: 0.563rem;
  color: #373d5a;
  font-family: 'Space Mono';
}
.span3 {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.span4 {
  height: 0.813rem;
  width: 0rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.contributorprofilelink {
  height: 1.063rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.631rem;
  color: #3de8c8;
  font-family: 'Space Mono';
}
.seemorerelatedgenres {
  position: relative;
  letter-spacing: 0.61px;
  line-height: 1.008rem;
}
.divsidebar {
  width: 20rem;
  height: 58.313rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 1rem;
  grid-column: 2;
  grid-row: 1;
  flex-shrink: 0;
  font-size: 0.563rem;
  color: #373d5a;
  font-family: 'Space Mono';
}
.sourcelist {
  align-self: stretch;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.divsideCardHead3 {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  align-items: center;
  padding: 0.6rem 1rem;
}
.divsideCardBody {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.5rem 1rem;
  font-size: 0.738rem;
  color: #7a84a8;
  font-family: 'Space Grotesk';
}
.div {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.287rem 0rem 0.356rem;
}
.source {
  position: relative;
  line-height: 1.258rem;
  font-weight: 300;
}
.afropop {
  color: #e4e8f5;
}
.div2 {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.293rem 0rem 0.362rem;
}
.div4 {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.293rem 0rem 0.362rem;
}
.source4 {
  align-self: stretch;
  position: relative;
  line-height: 1.258rem;
  font-weight: 300;
}
.divsideCardBody2 {
  align-self: stretch;
  height: 20.625rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.5rem 1rem 1.687rem;
  box-sizing: border-box;
  font-size: 0.738rem;
  color: #7a84a8;
  font-family: 'Space Grotesk';
}
.divsideCardBody3 {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.5rem 1rem;
  flex-shrink: 0;
}
.div14 {
  align-self: stretch;
  height: 12.375rem;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.293rem 0rem 0.362rem;
  box-sizing: border-box;
  flex-shrink: 0;
}

</style>
