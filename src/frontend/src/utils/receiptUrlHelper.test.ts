import { getInstanceId, getInstanceOwnerId } from './instance';

import { mockLocation } from 'testConfig/testUtils';

import {
  altinnAt22Url,
  altinnAt22PlatformUrl,
  getAltinnUrl,
  getTextResourceUrl,
  getUserUrl,
  getApplicationMetadataUrl,
  getAltinnCloudUrl,
  getExtendedInstanceUrl,
} from './receiptUrlHelper';

const originalLocation = window.location;

describe('receiptUrlHelper', () => {
  beforeEach(() => {
    mockLocation(originalLocation);
  });

  describe('getAltinnUrl', () => {
    it('should return altinnAt22Url when hostname is localhost', () => {
      mockLocation({ hostname: 'localhost' });

      expect(getAltinnUrl()).toEqual(altinnAt22Url);
    });

    it('should return window.location.origin followed by trailing slash when hostname is not localhost', () => {
      mockLocation({ hostname: 'not-localhost', origin: 'https://origin' });

      expect(getAltinnUrl()).toEqual('https://origin/');
    });
  });

  describe('getTextResourceUrl', () => {
    it('should return correct path with args', () => {
      expect(getTextResourceUrl('org-name', 'app-name', 'nb')).toEqual(
        'https://localhost/storage/api/v1/applications/org-name/app-name/texts/nb',
      );
    });
  });

  describe('getUserUrl', () => {
    it('should return correct path with args', () => {
      expect(getUserUrl()).toEqual(
        'https://localhost/receipt/api/v1/users/current',
      );
    });
  });

  describe('getApplicationMetadataUrl', () => {
    it('should return correct path when running on localhost', () => {
      mockLocation({ hostname: 'localhost' });

      expect(getApplicationMetadataUrl('org-name', 'app-name')).toEqual(
        `${altinnAt22PlatformUrl}storage/api/v1/applications/org-name/app-name`,
      );
    });

    it('should return correct path when not running on localhost', () => {
      mockLocation({ hostname: 'not-localhost', origin: 'https://origin' });

      expect(getApplicationMetadataUrl('org-name', 'app-name')).toEqual(
        `https://origin/storage/api/v1/applications/org-name/app-name`,
      );
    });
  });

  describe('getAltinnCloudUrl', () => {
    it('should return altinnAt22PlatformUrl when running on localhost', () => {
      mockLocation({ hostname: 'localhost' });

      expect(getAltinnCloudUrl()).toEqual(altinnAt22PlatformUrl);
    });

    it('should return altinnAt22PlatformUrl when running on 127.0.0.1', () => {
      mockLocation({ hostname: '127.0.0.1' });

      expect(getAltinnCloudUrl()).toEqual(altinnAt22PlatformUrl);
    });

    it('should return altinnAt22PlatformUrl when running on altinn3.no', () => {
      mockLocation({ hostname: 'altinn3.no' });

      expect(getAltinnCloudUrl()).toEqual(altinnAt22PlatformUrl);
    });

    it('should return window.location.origin followed by trailing slash when running on altinn3.no', () => {
      mockLocation({ hostname: 'not-localhost', origin: 'https://origin' });

      expect(getAltinnCloudUrl()).toEqual('https://origin/');
    });
  });

  describe('getExtendedInstanceUrl', () => {
    it('should return correct path when running on localhost', () => {
      mockLocation({ hostname: 'localhost' });

      expect(getExtendedInstanceUrl()).toEqual(
        `${altinnAt22PlatformUrl}receipt/api/v1/instances/${getInstanceOwnerId()}/${getInstanceId()}?includeParty=true`,
      );
    });

    it('should return correct path when not running on localhost', () => {
      mockLocation({ hostname: 'not-localhost', origin: 'https://origin' });

      expect(getExtendedInstanceUrl()).toEqual(
        `https://origin/receipt/api/v1/instances/${getInstanceOwnerId()}/${getInstanceId()}?includeParty=true`,
      );
    });
  });
});
