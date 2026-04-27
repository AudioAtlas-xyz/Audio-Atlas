<script setup lang="ts">
const props = defineProps({
  currentState: {
    type: Number,
    default: 1
  }
})

const getColor = (stateNumber: number) => {
  return props.currentState >= stateNumber ? '#3de8c8' : '#e4e8f5'
}

const getLineColor = (stateNumber: number) => {
  return props.currentState > stateNumber ? '#3de8c8' : '#b0b8d4'
}
</script>

<template>
  <div :class="$style.stepper">
    <div :class="$style.step">
      <div :class="$style.stepperItem" :style="{ backgroundColor: getColor(1) }">
        <div :class="$style.number">1</div>
      </div>
      <div :class="$style.address" :style="{ color: props.currentState >= 1 ? '#ffffff' : '#e4e8f5' }">
        Identity
      </div>
    </div>

    <div :class="$style.seperator" :style="{ backgroundColor: getLineColor(1) }" />

    <div :class="$style.step">
      <div :class="$style.stepperItem" :style="{ backgroundColor: getColor(2) }">
        <div :class="$style.number">2</div>
      </div>
      <div :class="$style.address" :style="{ color: props.currentState >= 2 ? '#ffffff' : '#e4e8f5' }">
        About
      </div>
    </div>

    <div :class="$style.seperator" :style="{ backgroundColor: getLineColor(2) }" />

    <div :class="$style.step">
      <div :class="$style.stepperItem" :style="{ backgroundColor: getColor(3) }">
        <div :class="$style.number">3</div>
      </div>
      <div :class="$style.address" :style="{ color: props.currentState >= 3 ? '#ffffff' : '#e4e8f5' }">
        Connections
      </div>
    </div>
  </div>
</template>


<style  module>.stepper {
    position: relative;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: flex-start;
    isolation: isolate;
    text-align: center;
    font-size: 12px;
    font-family: 'Space Grotesk';
}
    .step {
        width: 100px;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 8px;
        z-index: 5;
    }
    .stepperItem {
        width: 24px;
        height: 24px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .div {
        position: relative;
        line-height: 16px;
        font-weight: 500;
    }
    .titleDescription {
        align-self: stretch;
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        text-align: center;
        color: #e4e8f5;
    }
    .address {

        line-height: 16px;
        font-weight: 500;
    }
    .number {
      font-size: 12px;
      font-weight: 600;
      color: #07080f;


    }
    .seperator {
        flex-grow: 1;
        max-width: 200px;
        height: 2px;
        margin-top: 11px;
        min-width: 20px;
        border-radius: 999px;
  }
</style>
