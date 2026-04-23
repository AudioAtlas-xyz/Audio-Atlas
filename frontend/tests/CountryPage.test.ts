import { defineComponent, h, nextTick } from 'vue'
import { flushPromises, mount } from '@vue/test-utils'
import { beforeEach, describe, expect, it } from 'vitest'
import CountryPage from '~/pages/CountryPage.vue'
import type { Country } from '~/types/country'
import {
  __getFetchCalls,
  __getHeadEntries,
  __resetNuxtMocks,
  __setAsyncDataResult,
  __setFetchImplementation,
  __setRoute
} from './support/nuxt-imports'

const HeroStub = defineComponent({
  name: 'CountryHeroSection',
  props: {
    locationBadges: { type: Array, required: true },
    countryName: { type: String, required: true },
    description: { type: String, required: true },
    breadCrumbItems: { type: Array, required: true }
  },
  setup(props) {
    return () => h('pre', { 'data-test': 'hero-props' }, JSON.stringify(props))
  }
})

const GenreCardStub = defineComponent({
  name: 'CountryGenreCard',
  props: {
    genre: { type: Object, required: true }
  },
  setup(props) {
    return () => h('div', { 'data-test': 'genre-card' }, (props.genre as { name: string }).name)
  }
})

const ContributorsCardStub = defineComponent({
  name: 'CountryContributorsCard',
  props: {
    contributors: { type: Array, required: true }
  },
  setup(props) {
    return () => h('pre', { 'data-test': 'contributors-props' }, JSON.stringify(props.contributors))
  }
})

const AlertStub = defineComponent({
  name: 'UAlert',
  props: {
    title: { type: String, default: '' },
    description: { type: String, default: '' }
  },
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
        UContainer: defineComponent({
          name: 'UContainer',
          setup(_, { slots }) {
            return () => h('div', { 'data-test': 'container' }, slots.default?.())
          }
        }),
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
        aliases: ['Samba urbano'],
        status: 'Verified'
      },
      {
        id: 'genre-2',
        name: 'Bossa Nova',
        description: 'Soft and syncopated.',
        region: 'Southeast',
        aliases: ['Bossa'],
        status: 'Documented'
      }
    ],
    contributors: [
      {
        id: 'contributor-1',
        username: 'ana',
        displayName: 'Ana Silva',
        genresCount: 2
      },
      {
        id: 'contributor-2',
        username: 'joao',
        displayName: 'Joao Costa',
        genresCount: 1
      }
    ],
    ...overrides
  }
}

beforeEach(() => {
  __resetNuxtMocks()
})

describe('CountryPage', () => {
  it('fetches country data from the query string and renders the country overview', async () => {
    const country = createCountry()

    __setRoute({ countryId: country.id }, `/CountryPage?countryId=${country.id}`)
    __setFetchImplementation(async () => country)

    const wrapper = await renderPage()
    const heroProps = JSON.parse(wrapper.get('[data-test="hero-props"]').text())
    const contributors = JSON.parse(wrapper.get('[data-test="contributors-props"]').text())

    expect(__getFetchCalls()).toEqual([
      `/api/countries/${country.id}`
    ])
    expect(heroProps).toEqual({
      locationBadges: ['Americas', 'South America'],
      countryName: 'Brazil',
      description: 'Deep rhythmic traditions across multiple regions.',
      breadCrumbItems: [
        { label: 'Explore', to: '/' },
        { label: 'Americas', to: '/' },
        { label: 'Brazil', to: `/CountryPage?countryId=${country.id}`, active: true }
      ]
    })
    expect(wrapper.findAll('[data-test="genre-card"]')).toHaveLength(2)
    expect(contributors).toEqual(country.contributors)
    expect(wrapper.text()).toContain('2 Genres documented')
    expect(wrapper.text()).toContain('2 genres from Brazil documented in Audio Atlas')
    expect(__getHeadEntries()).toEqual([{ title: 'Brazil | Audio Atlas' }])
  })

  it('uses the fallback description and filters empty location metadata', async () => {
    const country = createCountry({
      name: 'Iceland',
      description: '   ',
      region: '',
      continent: 'Europe',
      genres: [],
      contributors: []
    })

    __setRoute({ countryId: country.id }, `/CountryPage?countryId=${country.id}`)
    __setFetchImplementation(async () => country)

    const wrapper = await renderPage()
    const heroProps = JSON.parse(wrapper.get('[data-test="hero-props"]').text())

    expect(heroProps.description).toBe(
      'Country context from the Audio Atlas API will appear here once the backend payload is wired up.'
    )
    expect(heroProps.locationBadges).toEqual(['Europe'])
    expect(heroProps.breadCrumbItems).toEqual([
      { label: 'Explore', to: '/' },
      { label: 'Iceland', to: `/CountryPage?countryId=${country.id}`, active: true }
    ])
    expect(wrapper.text()).toContain('No genres documented yet')
  })

  it('shows a missing-country warning and avoids fetching when countryId is absent', async () => {
    __setRoute({}, '/CountryPage')

    const wrapper = await renderPage()
    const alerts = wrapper.findAll('[data-test="alert"]').map(node => node.text())

    expect(__getFetchCalls()).toEqual([])
    expect(alerts).toContain(
      'Missing countryId|Open this page with a ?countryId=... query so the page can request country data from the backend.'
    )
    expect(alerts).toContain(
      'No genres documented yet|No genre data has been returned yet.'
    )
    expect(__getHeadEntries()).toEqual([{ title: 'Country | Audio Atlas' }])
  })

  it('shows an error alert when the backend request fails', async () => {
    __setRoute({ countryId: 'broken-country' }, '/CountryPage?countryId=broken-country')
    __setFetchImplementation(async () => {
      throw new Error('Backend unavailable')
    })

    const wrapper = await renderPage()
    const alerts = wrapper.findAll('[data-test="alert"]').map(node => node.text())

    expect(__getFetchCalls()).toEqual([
      '/api/countries/broken-country'
    ])
    expect(alerts).toContain('Could not load country data|Backend unavailable')
    expect(alerts).toContain('No genres documented yet|No genre data has been returned yet.')
  })

  it('renders loading skeletons while async data is pending', async () => {
    __setRoute({ countryId: 'pending-country' }, '/CountryPage?countryId=pending-country')
    __setAsyncDataResult({
      data: null,
      pending: true,
      error: null
    })

    const wrapper = await renderPage()

    expect(__getFetchCalls()).toEqual([])
    expect(wrapper.findAll('[data-test="skeleton"]')).toHaveLength(10)
    expect(wrapper.find('[data-test="hero-props"]').exists()).toBe(false)
    expect(wrapper.text()).toContain('0 Genres documented')
  })
})
