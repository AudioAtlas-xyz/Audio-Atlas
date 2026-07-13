<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  itemList: { label: string; value: string }[]
  modelValue: string[]
}>(), {
  modelValue: () => []
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>()

const selectedItems = computed(() =>
  props.modelValue
    .map(id => props.itemList.find(item => item.value === id))
    .filter((item): item is { label: string; value: string } => item !== undefined)
)

function removeItem(value: string) {
  emit('update:modelValue', props.modelValue.filter(id => id !== value))
}
</script>

<template>
  <div class="select-wrapper">
    <USelectMenu
      :model-value="selectedItems"
      :items="itemList"
      multiple
      searchable
      search-attribute="label"
      trailing-icon="i-lucide-chevrons-up-down"
      placeholder="Search and select…"
      class="w-full"
      @update:model-value="(items) => emit('update:modelValue', items.map((i: { value: string }) => i.value))"
    />

    <div v-if="selectedItems.length > 0" class="chips">
      <span
        v-for="item in selectedItems"
        :key="item.value"
        class="chip"
      >
        {{ item.label }}
        <button
          type="button"
          class="chip-x"
          :aria-label="`Remove ${item.label}`"
          @click="removeItem(item.value)"
        >
          <UIcon name="i-lucide-x" class="chip-x__icon" />
        </button>
      </span>
    </div>
  </div>
</template>

<style scoped>
.select-wrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.chips {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.chip {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: 0 6px 0 10px;
  height: 24px;
  border-radius: 12px;
  background: rgba(136, 153, 255, 0.12);
  border: 1px solid #8899ff;
  font-size: 11px;
  color: #8899ff;
  font-family: 'Space Grotesk', sans-serif;
  white-space: nowrap;
}

.chip-x {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: none;
  background: rgba(136, 153, 255, 0.2);
  color: #8899ff;
  cursor: pointer;
  padding: 0;
  flex-shrink: 0;
  transition: background 0.15s ease;
}

.chip-x:hover {
  background: rgba(136, 153, 255, 0.45);
}

.chip-x__icon {
  width: 9px;
  height: 9px;
}
</style>
