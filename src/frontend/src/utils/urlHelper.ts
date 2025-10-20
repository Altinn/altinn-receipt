import type { IAltinnWindow } from '../types';
import { getReturnUrl } from './instance';

const { org, app } = window as Window as IAltinnWindow;
const origin = window.location.origin;

export const getApplicationMetadataUrl = (): string => {
  return `${origin}/designer/api/v1/${org}/${app}`;
};

const baseHostnameAltinnProd = 'altinn.no';
const baseHostnameAltinnTest = 'altinn.cloud';
const baseHostnameAltinnLocal = 'altinn3local.no';
const pathToMessageBox = 'ui/messagebox';
const prodRegex = new RegExp(baseHostnameAltinnProd);
const testRegex = new RegExp(baseHostnameAltinnTest);
const localRegex = new RegExp(baseHostnameAltinnLocal);

export const returnUrlToMessagebox = (
  url: string,
  partyId?: string | undefined,
): string => {
  let returnUrl = getReturnUrl();

  if (returnUrl) {
    return returnUrl;
  }

  const isAltinn3 = url.includes('/af.');
  const generatedReturnUrl = isAltinn3 ? returnUrlToA2Messagebox(url) : returnUrlToA3Messagebox(url);
  return generatedReturnUrl;
};

export const returnUrlToA2Messagebox = (
  url: string,
  partyId?: string | undefined,
): string => {
  const baseUrl = returnBaseUrlToAltinn2(url);
  if (!baseUrl) {
    return null;
  }

  if (partyId === undefined) {
    return baseUrl + pathToMessageBox;
  }

  return `${baseUrl}ui/Reportee/ChangeReporteeAndRedirect?goTo=${baseUrl}${pathToMessageBox}&R=${partyId}`;
};

export const returnUrlToA3Messagebox = (
  url: string,
  partyId?: string | undefined,
): string => {
  // Inbox in Altinn 3 is located at baseUrl
  const baseUrl = returnBaseUrlToAltinn3(url);
  return baseUrl;
};

export const returnBaseUrlToAltinn2 = (url: string): string => {
  let result: string;
  if (url.search(prodRegex) >= 0) {
    const split = url.split('.');
    const env = split[split.length - 3];
    if (env === 'tt02') {
      result = `https://${env}.${baseHostnameAltinnProd}/`;
    } else {
      result = `https://${baseHostnameAltinnProd}/`;
    }
  } else if (url.search(testRegex) >= 0) {
    const split = url.split('.');
    const env = split[split.length - 3];
    result = `https://${env}.${baseHostnameAltinnTest}/`;
  } else if (url.search(localRegex) >= 0) {
    result = '/';
  } else {
    result = null;
  }
  return result;
};

export const returnBaseUrlToAltinn3 = (url: string): string => {
  let result: string;
  if (url.search(prodRegex) >= 0) {
    const split = url.split('.');
    const env = split[split.length - 3];
    if (env === 'tt02') {
      result = `https://af.tt.${baseHostnameAltinnProd}/`;
    } else {
      result = `https://af.${baseHostnameAltinnProd}/`;
    }
  } else if (url.search(testRegex) >= 0) {
    const split = url.split('.');
    const env = split[split.length - 3];
    result = `https://af.${env}.${baseHostnameAltinnTest}/`;
  } else if (url.search(localRegex) >= 0) {
    result = '/';
  } else {
    result = null;
  }
  return result;
};

export const logoutUrlAltinn = (url: string): string => {
  const isAltinn3 = url.includes('/af.');
  if (isAltinn3) {
    return `${returnBaseUrlToAltinn3(url)}logout`;
  }
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
