<script setup lang="ts">


</script>

<template>
  <div :class="$style.sourcelist">
    <div :class="$style.divsideCardHead">
      <div :class="$style.span">
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
</template>
<style  module>.sourcelist {
  width: 100%;
  position: relative;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  text-align: left;
  font-size: 9px;
  color: #373d5a;
  font-family: 'Space Mono';
}
.divsideCardHead {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  align-items: center;
  padding: 9.6px 16px;
}
.span {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.cardTitle {
  position: relative;
  letter-spacing: 1.25px;
  text-transform: uppercase;
}
.divsideCardBody {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 8px 16px;
  font-size: 11.8px;
  color: #7a84a8;
  font-family: 'Space Grotesk';
}
.div {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 4.6px 0px 5.7px;
}
.source {
  position: relative;
  line-height: 20.13px;
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
  padding: 4.7px 0px 5.8px;
}
.div4 {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 4.7px 0px 5.8px;
}
.source4 {
  align-self: stretch;
  position: relative;
  line-height: 20.13px;
  font-weight: 300;
}

</style>
