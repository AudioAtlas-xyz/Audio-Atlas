import { defineComponent, h, nextTick } from 'vue'
import { flushPromises, mount } from '@vue/test-utils'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import type { Country } from '~/types/country'

import {
  __getHeadEntries,
  __resetNuxtMocks,
  __setAsyncDataResult,
  __setRoute,
  __setMockApiFunction
} from './support/nuxt-imports'

import CountryPage from '~/pages/CountryPage.vue'

const HeroStub = defineComponent({
  name: 'CountryHeroSection',
  props: ['locationBadges', 'countryName', 'description', 'breadCrumbItems'],
  setup(props) {
    return () => h('pre', { 'data-test': 'hero-props' }, JSON.stringify(props))
  }
})

const GenreCardStub = defineComponent({
  name: 'CountryGenreCard',
  props: ['genre'],
  setup(props: any) {
    return () => h('div', { 'data-test': 'genre-card' }, props.genre?.name ?? '')
  }
})

const ContributorsCardStub = defineComponent({
  name: 'CountryContributorsCard',
  props: ['contributors'],
  setup(props) {
    return () => h('pre', { 'data-test': 'contributors-props' }, JSON.stringify(props.contributors))
  }
})

const AlertStub = defineComponent({
  name: 'UAlert',
  props: ['title', 'description', 'color', 'variant'],
  setup(props) {
    return () => h('div', { 'data-test': 'alert' }, `${props.title}|${props.description}`)
  }
})

const SkeletonStub = defineComponent({
  name: 'USkeleton',
  setup() {
    return () => h('div', { 'data-test': 'skeleton' })
  }
})

async function renderPage() {
  const TestHarness = defineComponent({
    components: { CountryPage },
    template: '<Suspense><CountryPage /></Suspense>'
  })

  const wrapper = mount(TestHarness, {
    global: {
      stubs: {
        UContainer: {
          setup(_, { slots }) {
            return () => h('div', { 'data-test': 'container' }, slots.default?.())
          }
        },
        UAlert: AlertStub,
        USkeleton: SkeletonStub,
        CountryHeroSection: HeroStub,
        CountryGenreCard: GenreCardStub,
        CountryContributorsCard: ContributorsCardStub
      }
    }
  })

  await flushPromises()
  await nextTick()
  await flushPromises()

  return wrapper
}

function createCountry(overrides: Partial<Country> = {}): Country {
  return {
    id: 'country-1',
    name: 'Brazil',
    description: 'Deep rhythmic traditions across multiple regions.',
    continent: 'South America',
    region: 'Americas',
    genres: [
      {
        id: 'genre-1',
        name: 'Samba',
        description: 'Percussive dance music.',
        region: 'Rio de Janeiro',
        aliases: [],
        status: 'Verified'
      }
    ],
    contributors: [
      {
        id: 'c1',
        username: 'ana',
        displayName: 'Ana',
        genresCount: 2
      }
    ],
    ...overrides
  }
}

let mockApi: any

beforeEach(() => {
  __resetNuxtMocks()
  // Create a fresh mock for each test
  mockApi = vi.fn()
  __setMockApiFunction(mockApi)
})

describe('CountryPage', () => {
  it('fetches country data and renders overview', async () => {
    const country = createCountry()
    mockApi.mockResolvedValue(country)
    
    __setRoute({ countryId: country.id }, `/CountryPage?countryId=${country.id}`)

    const wrapper = await renderPage()
    await flushPromises()

    expect(mockApi).toHaveBeenCalledWith(`/countries/${country.id}`)

    const heroProps = JSON.parse(wrapper.get('[data-test="hero-props"]').text())
    expect(heroProps.countryName).toBe('Brazil')
    expect(wrapper.findAll('[data-test="genre-card"]').length).toBe(1)
    expect(__getHeadEntries()).toEqual([{ title: 'Brazil | Audio Atlas' }])
  })

  it('handles missing countryId', async () => {
    __setRoute({}, '/CountryPage')

    const wrapper = await renderPage()
    await flushPromises()

    const alerts = wrapper.findAll('[data-test="alert"]').map(n => n.text())
    expect(mockApi).not.toHaveBeenCalled()
    expect(alerts.some(a => a.includes('Missing countryId'))).toBe(true)
  })

  it('handles API error', async () => {
    mockApi.mockRejectedValue(new Error('Backend unavailable'))
    
    __setRoute({ countryId: 'broken' }, '/CountryPage?countryId=broken')

    const wrapper = await renderPage()
    await flushPromises()

    expect(mockApi).toHaveBeenCalledWith('/countries/broken')

    const alerts = wrapper.findAll('[data-test="alert"]').map(n => n.text())
    expect(alerts.some(a => a.includes('Could not load country data'))).toBe(true)
  })

  it('renders skeleton state', async () => {
    __setRoute({ countryId: 'loading' }, '/CountryPage?countryId=loading')

    __setAsyncDataResult({
      data: null,
      pending: true,
      error: null
    })

    const wrapper = await renderPage()
    await flushPromises()

    expect(wrapper.findAll('[data-test="skeleton"]').length).toBeGreaterThan(0)
  })
})