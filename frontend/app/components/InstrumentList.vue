<script setup lang="ts">
import type {Instrument} from "~/types/instrument";

const props = defineProps<{
  instrument: Instrument[]
}>()

</script>

<template>
  <UCard
    class="border border-border bg-surface shadow-none"
    :ui="{
      header: 'border-b border-border bg-surface-2 px-4 py-3',
      body: 'px-0 py-0',
      footer: 'hidden'
    }"
  >
    <template #header>
      <div class="flex items-center justify-between">
        <p class="text-sm text-space-50">
          Instruments
        </p>
        <p class="font-mono text-[11px] text-[#373d5a]">
          {{ props.instrument.length }}
        </p>
      </div>
    </template>

    <div v-if="instrument.length" class="divide-y divide-border">
      <div
        v-for="it in instrument"
        :key="it.name"
        class="flex items-center justify-between gap-3 px-4 py-3"
      >
        <div class="flex min-w-0 items-center gap-3">

          <div class="min-w-0">
            {{ it.name }}
          </div>
        </div>
      </div>
    </div>

    <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
      Currently no instruments included for this genre.
    </div>
  </UCard>
</template>
<style  module>.instrumentlist {
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
