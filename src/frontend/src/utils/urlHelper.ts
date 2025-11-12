import type { IAltinnWindow } from '../types';
import { getReturnUrl } from './instance';

const { org, app } = window as Window as IAltinnWindow;
const origin = window.location.origin;

export const getApplicationMetadataUrl = (): string => {
  return `${origin}/designer/api/v1/${org}/${app}`;
};

const ALTINN_HOSTNAMES = {
  PROD: 'altinn.no',
  TEST: 'altinn.cloud',
  LOCAL: 'altinn3local.no',
} as const;

const INBOX_URLS = {
  PROD: 'https://af.altinn.no',
  TT: 'https://af.tt.altinn.no',
  AT: 'https://af.at.altinn.cloud',
  YT: 'https://af.yt.altinn.cloud',
  LOCAL: '/',
} as const;

const isATEnvironment = (url: string): boolean => url.includes('.at.') || url.includes('.at22.');
const isYTEnvironment = (url: string): boolean => url.includes('.yt.') || url.includes('.yt01.');
const isTTEnvironment = (url: string): boolean => url.includes('.tt.') || url.includes('.tt02.');
const isProdEnvironment = (url: string): boolean => url.includes(ALTINN_HOSTNAMES.PROD);
const isLocalEnvironment = (url: string): boolean => url.includes(ALTINN_HOSTNAMES.LOCAL);

const getInboxBaseUrl = (url: string): string | null => {
  if (isLocalEnvironment(url)) return INBOX_URLS.LOCAL;
  if (isATEnvironment(url)) return INBOX_URLS.AT;
  if (isYTEnvironment(url)) return INBOX_URLS.YT;
  if (isTTEnvironment(url)) return INBOX_URLS.TT;
  if (isProdEnvironment(url)) return INBOX_URLS.PROD;
  return null;
};

const addPartyIdToUrl = (baseUrl: string, partyId?: string): string => {
  if (!partyId) return baseUrl;
  return `${baseUrl}?partyId=${partyId}`;
};

export const returnUrlToMessagebox = (url: string, partyId?: string): string => {
  const customReturnUrl = getReturnUrl();
  if (customReturnUrl) {
    return customReturnUrl;
  }

  const inboxBaseUrl = getInboxBaseUrl(url);
  if (!inboxBaseUrl) {
    return null;
  }

  return addPartyIdToUrl(inboxBaseUrl, partyId);
};

const getEnvironmentFromUrl = (url: string): string => {
  const parts = url.split('.');
  return parts[parts.length - 3];
};

const buildAltinn2BaseUrl = (hostname: string, environment?: string): string => {
  if (environment) {
    return `https://${environment}.${hostname}/`;
  }
  return `https://${hostname}/`;
};

export const returnBaseUrlToAltinn2 = (url: string): string | null => {
  if (isLocalEnvironment(url)) {
    return '/';
  }

  if (isProdEnvironment(url)) {
    const environment = getEnvironmentFromUrl(url);
    if (environment === 'tt02') {
      return buildAltinn2BaseUrl(ALTINN_HOSTNAMES.PROD, environment);
    }
    return buildAltinn2BaseUrl(ALTINN_HOSTNAMES.PROD);
  }

  if (url.includes(ALTINN_HOSTNAMES.TEST)) {
    const environment = getEnvironmentFromUrl(url);
    return buildAltinn2BaseUrl(ALTINN_HOSTNAMES.TEST, environment);
  }

  return null;
};

export const logoutUrlAltinn = (url: string): string => {
  return `${returnBaseUrlToAltinn2(url)}ui/authentication/LogOut`;
};

// Storage is always returning https:// links for attachments.
// on localhost (without https) this is a problem, so we make links
// to the same domain as window.location.host relative.
// "https://domain.com/a/b" => "/a/b"
export const makeUrlRelativeIfSameDomain = (
  url: string,
  location: Location = window.location,
) => {
  try {
    const parsed = new URL(url);
    if (parsed.hostname === location.hostname) {
      return parsed.pathname + parsed.search + parsed.hash;
    }
  } catch (_e) {
    //ignore invalid (or dummy) urls
  }
  return url;
};
