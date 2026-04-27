<script setup lang="ts">
import type { Country } from '~/types/country'
import type {Genre} from "~/types/genre";

const { data } = await useFetch<Country[]>('/api/countries/all')

const countryNames = computed(() =>
  data.value?.map(c => ({
    label: c.name,
    value: c.name
  })) ?? []
)

const form = useTemplateRef('form')
</script>

<template>
  <Stepper></Stepper>
  <div :class="$style.genreidentityformcard">
    <UForm ref="form" />
    <UContainer style="padding: 3rem;">
    <div :class="$style.identity">Identity</div>
    <h1>Name this genre and tell us where it comes from.</h1>


      <UFormField label="Genre name" required>
        <UInput placeholder="e.g Afrobeats" style="color: #7a84a8" class="w-full"/>
      </UFormField>

      <UFormField label="Aliases" hint="Optional">
        <UInput placeholder="Add an alias and press enter" class="w-full"></UInput>
      </UFormField>

      <UFormField label="COUNTRY / COUNTRIES OF ORIGIN" field="name" required>
        <SubmissionSelectMenu :itemList ="countryNames" class="w-full"/>
      </UFormField>

    </UContainer>
  </div>
</template>

<style module>
.genreidentityformcard {
  width: 50%;
  height: 37.5rem;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  text-align: left;
  font-size: 0.75rem;
  color: #7a84a8;
  font-family: Space_Grotesk;
  margin: 2rem auto 0 auto;
}
.identity {
  width: 4.813rem;
  position: relative;
  font-size: 1.25rem;
  font-weight: 500;
  font-family: 'Space Grotesk';
  color: #e4e8f5;
  text-align: left;
  display: inline-block;
}

h1 {
  color: #7a84a8;
}
UFormField{
  width: 100%;
}
UInput{
  width: 100%;
}
</style>
