import { createError, defineEventHandler, getRequestURL, proxyRequest } from 'h3'

export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig(event)
  const apiProxyTarget = config.apiProxyTarget?.replace(/\/+$/, '')

  if (!apiProxyTarget) {
    throw createError({
      statusCode: 500,
      statusMessage: 'Missing NUXT_API_PROXY_TARGET runtime config.'
    })
  }

  const requestUrl = getRequestURL(event)
  const targetUrl = `${apiProxyTarget}${requestUrl.pathname}${requestUrl.search}`

  return proxyRequest(event, targetUrl)
})
