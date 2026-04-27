<script setup lang="ts">
import type { Country } from '~/types/country'

const form = useTemplateRef('form')
</script>

<template>
  <Stepper></Stepper>
  <div :class="$style.genreidentityformcard"
       margin-left="auto"
       margin-right="auto">
    <UForm ref="form" />
      <h1>bleepbloop</h1>
    <SelectMenu></SelectMenu>
  </div>
</template>

<style module>
.genreidentityformcard {
  width: 50%;
  height: 37.5rem;
  position: absolute;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  text-align: left;
  font-size: 0.75rem;
  color: #e4e8f5;
  font-family: Space_Grotesk;
  margin-left: auto;
  margin-right: auto;
}
</style>
