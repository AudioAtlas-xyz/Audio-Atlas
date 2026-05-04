import { ref } from 'vue'

type AsyncDataResult<T> = {
  data: T
  pending: boolean
  error: Error | null
}

let routeState = {
  query: {} as Record<string, unknown>,
  fullPath: '/CountryPage'
}

let asyncDataOverride: AsyncDataResult<unknown> | null = null
let fetchCalls: string[] = []
let headEntries: Array<Record<string, unknown>> = []
let fetchImplementation = async <T>(_url: string): Promise<T> => null as T
let runtimeConfig = {
  public: {
    apiBase: '/api'
  }
}

export function __resetNuxtMocks() {
  routeState = {
    query: {},
    fullPath: '/CountryPage'
  }
  asyncDataOverride = null
  fetchCalls = []
  headEntries = []
  fetchImplementation = async <T>(_url: string): Promise<T> => null as T
  runtimeConfig = {
    public: {
      apiBase: '/api'
    }
  }
}

export function __setRoute(query: Record<string, unknown>, fullPath = '/CountryPage') {
  routeState = { query, fullPath }
}

export function __setAsyncDataResult(result: AsyncDataResult<unknown> | null) {
  asyncDataOverride = result
}

export function __setFetchImplementation(fn: typeof fetchImplementation) {
  fetchImplementation = fn
}

export function __getFetchCalls() {
  return [...fetchCalls]
}

export function __getHeadEntries() {
  return [...headEntries]
}

export function __setRuntimeConfig(config: typeof runtimeConfig) {
  runtimeConfig = config
}

export function useRoute() {
  return routeState
}

export function useRuntimeConfig() {
  return runtimeConfig
}

export async function useAsyncData<T>(
  _key: string,
  handler: () => Promise<T>,
  options?: { default?: () => T }
) {
  if (asyncDataOverride) {
    return {
      data: ref(asyncDataOverride.data as T),
      pending: ref(asyncDataOverride.pending),
      error: ref(asyncDataOverride.error)
    }
  }

  try {
    const resolved = await handler()

    return {
      data: ref((resolved ?? options?.default?.()) as T),
      pending: ref(false),
      error: ref<Error | null>(null)
    }
  } catch (error) {
    return {
      data: ref(options?.default?.() as T),
      pending: ref(false),
      error: ref(error as Error)
    }
  }
}

export function useHead(input: Record<string, unknown> | (() => Record<string, unknown>)) {
  headEntries.push(typeof input === 'function' ? input() : input)
}

export async function $fetch<T>(url: string) {
  fetchCalls.push(url)
  return fetchImplementation<T>(url)
}
