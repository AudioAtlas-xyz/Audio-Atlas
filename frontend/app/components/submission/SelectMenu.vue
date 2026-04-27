<script setup lang="ts">
import { reactive, useTemplateRef } from 'vue'
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'

const props =

const items = ref(['Backlog', 'Todo', 'In Progress', 'Done'])
const value = ref('Backlog')


const schema = z.object({
  selectMenuMultiple: z.any().refine(values => !!values?.find((option: any) => option.value === 'option-2'), {
    message: 'Option 2 must be included'
  })
})


</script>

<template>
  <USelectMenu v-model="value" :items="items" />
</template>

<style scoped>

</style>
