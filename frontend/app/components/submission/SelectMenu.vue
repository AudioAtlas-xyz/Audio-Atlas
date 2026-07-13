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
  <USelectMenu
    :model-value="selectedItems"
    :items="itemList"
    multiple
    searchable
    search-attribute="label"
    class="w-full"
    @update:model-value="(items) => emit('update:modelValue', items.map((i: { value: string }) => i.value))"
  >
    <template #default>
      <div class="trigger">
        <div class="trigger__body">
          <span v-if="!selectedItems.length" class="trigger__placeholder">
            Search and select…
          </span>
          <template v-else>
            <span
              v-for="item in selectedItems"
              :key="item.value"
              class="chip"
              @click.stop
            >
              {{ item.label }}
              <span
                role="button"
                tabindex="0"
                class="chip__x"
                :aria-label="`Remove ${item.label}`"
                @click.stop="removeItem(item.value)"
                @keydown.enter.stop="removeItem(item.value)"
                @keydown.space.prevent.stop="removeItem(item.value)"
              >
                <UIcon name="i-lucide-x" class="chip__x-icon" />
              </span>
            </span>
          </template>
        </div>
        <UIcon name="i-lucide-chevrons-up-down" class="trigger__chevron" />
      </div>
    </template>
  </USelectMenu>
</template>

<style scoped>
.trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  min-height: 1.75rem;
  gap: 0.5rem;
}

.trigger__body {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  flex: 1;
  min-width: 0;
}

.trigger__placeholder {
  font-size: 0.875rem;
  color: #4a6070;
}

.trigger__chevron {
  width: 1rem;
  height: 1rem;
  color: #4a6070;
  flex-shrink: 0;
}

.chip {
  display: inline-flex;
  align-items: center;
  gap: 3px;
  padding: 0 4px 0 8px;
  height: 20px;
  border-radius: 10px;
  background: rgba(136, 153, 255, 0.12);
  border: 1px solid #8899ff;
  font-size: 11px;
  color: #8899ff;
  font-family: 'Space Grotesk', sans-serif;
  white-space: nowrap;
  user-select: none;
}

.chip__x {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: rgba(136, 153, 255, 0.2);
  cursor: pointer;
  flex-shrink: 0;
  transition: background 0.15s ease;
}

.chip__x:hover {
  background: rgba(136, 153, 255, 0.45);
}

.chip__x-icon {
  width: 8px;
  height: 8px;
}
</style>
