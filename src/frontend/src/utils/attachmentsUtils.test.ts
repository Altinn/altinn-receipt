import { IAttachment, IData, IDataType } from '../types';
import { filterAppData, getInstancePdf, mapAppDataToAttachments } from './attachmentsUtils';

describe(mapAppDataToAttachments.name, () => {
  it('returns empty array when no data is provided', () => {
    const result = mapAppDataToAttachments([]);
    expect(result).toEqual([]);
  });

  it('returns empty array when data is null', () => {
    const result = mapAppDataToAttachments(null);
    expect(result).toEqual([]);
  });

  it('returns empty array when data is undefined', () => {
    const result = mapAppDataToAttachments(undefined);
    expect(result).toEqual([]);
  });

  it('returns correct attachment array when data is provided', () => {
    const data = [
      {
        id: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        dataType: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        filename: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1.xml',
        contentType: 'application/Xml',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        },
        size: 0,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:15.1480698Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:15.14807Z',
        lastChangedBy: '50001',
      },
      {
        id: '03e06136-88be-4866-a216-7959afe46137',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '4mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
        },
        size: 4194304,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:27.4719761Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:27.4719776Z',
        lastChangedBy: '50001',
      },
      {
        id: '11943e38-9fc4-43f6-84c4-12e529eebd28',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '8mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
        },
        size: 8388608,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:28.0099729Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:28.0099731Z',
        lastChangedBy: '50001',
      },
      {
        id: '092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '2mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
        },
        size: 2097152,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:30.3266993Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:30.3266995Z',
        lastChangedBy: '50001',
      },
      {
        id: '8698103b-fad1-4665-85c6-bf88a75ad708',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '4mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
        },
        size: 4194304,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:44.2017248Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:44.2017252Z',
        lastChangedBy: '50001',
      },
      {
        id: 'e950864d-e304-41ca-a60c-0c5019166df8',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '8mb.txt',
        contentType: 'text/plain',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/e950864d-e304-41ca-a60c-0c5019166df8',
        },
        size: 8388608,
        createdBy: '50001',
        lastChangedBy: '50001',
      },
      {
        id: '005d5bc3-a315-4705-9b06-3788fed86da1',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '2mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
        },
        size: 2097152,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:46.8968953Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:46.8968955Z',
        lastChangedBy: '50001',
      },
    ] as IData[];

    const attachmentsTestData = [
      {
        dataType: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        iconClass: 'reg reg-attachment',
        name: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1.xml',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '4mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/03e06136-88be-4866-a216-7959afe46137',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '8mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/11943e38-9fc4-43f6-84c4-12e529eebd28',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '2mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/092f032d-f54f-49c1-ae42-ebc0d10a2fcb',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '4mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/8698103b-fad1-4665-85c6-bf88a75ad708',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '8mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/e950864d-e304-41ca-a60c-0c5019166df8',
      },
      {
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        iconClass: 'reg reg-attachment',
        name: '2mb.txt',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
      },
    ] as IAttachment[];

    expect(mapAppDataToAttachments(data)).toEqual(attachmentsTestData);
  });
});

describe(getInstancePdf.name, () => {
  it('returns correct attachment', () => {
    const data = [
      {
        id: '585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        dataType: 'ref-data-as-pdf',
        filename: 'kvittering.pdf',
        contentType: 'application/pdf',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
        },
        size: 0,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:15.1480698Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:15.14807Z',
        lastChangedBy: '50001',
      },
      {
        id: '005d5bc3-a315-4705-9b06-3788fed86da1',
        dataType: 'cca36865-8f2e-4d29-8036-fa33bc4c3c34',
        filename: '2mb.txt',
        contentType: 'text/plain',
        storageUrl:
          'tjeneste-190814-1426/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
        selfLinks: {
          apps: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/005d5bc3-a315-4705-9b06-3788fed86da1',
        },
        size: 2097152,
        isLocked: false,
        createdDateTime: '2019-08-22T15:38:46.8968953Z',
        createdBy: '50001',
        lastChangedDateTime: '2019-08-22T15:38:46.8968955Z',
        lastChangedBy: '50001',
      },
    ];

    const expectedResult = [
      {
        dataType: 'ref-data-as-pdf',
        iconClass: 'reg reg-attachment',
        name: 'kvittering.pdf',
        url: 'https://altinn3.no/matsgm/tjeneste-190814-1426/instances/50001/c1572504-9fb6-4829-9652-3ca9c82dabb9/data/585b2f4e-5ecb-417b-9d01-82b6e889e1d1',
      },
    ];

    expect(getInstancePdf(data as unknown as IData[])).toEqual(expectedResult);
  });
});

describe(filterAppData.name, () => {
  it('should return empty array when appData is empty', () => {
    const dataTypes = [{ id: 'test-id', allowedContributers: ['app:owned'] }] as IDataType[];
    const appData: IData[] = [];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([]);
  });

  it('should return all appData when no dataTypes match exclusion criteria', () => {
    const dataTypes = [{ id: 'type1' }, { id: 'type2', allowedContributers: ['user'] }] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'type2', id: '2' },
      { dataType: 'type3', id: '3' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual(appData);
  });

  it('should exclude appData with dataType that has appLogic=true', () => {
    const dataTypes = [{ id: 'type1', appLogic: true }, { id: 'type2' }] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'type2', id: '2' },
      { dataType: 'type3', id: '3' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([
      { dataType: 'type2', id: '2' },
      { dataType: 'type3', id: '3' },
    ]);
  });

  it('should exclude appData with dataType that has allowedContributers including app:owned', () => {
    const dataTypes = [
      { id: 'type1', allowedContributers: ['user'] },
      { id: 'type2', allowedContributers: ['app:owned', 'user'] },
    ] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'type2', id: '2' },
      { dataType: 'type3', id: '3' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([
      { dataType: 'type1', id: '1' },
      { dataType: 'type3', id: '3' },
    ]);
  });

  it('should exclude appData with dataType id equal to ref-data-as-pdf', () => {
    const dataTypes = [{ id: 'type1' }, { id: 'ref-data-as-pdf' }] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'ref-data-as-pdf', id: '2' },
      { dataType: 'type3', id: '3' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([
      { dataType: 'type1', id: '1' },
      { dataType: 'type3', id: '3' },
    ]);
  });

  it('should exclude appData based on multiple exclusion criteria', () => {
    const dataTypes = [
      { id: 'type1', appLogic: true },
      { id: 'type2', allowedContributers: ['app:owned'] },
      { id: 'ref-data-as-pdf' },
      { id: 'type4' },
    ] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'type2', id: '2' },
      { dataType: 'ref-data-as-pdf', id: '3' },
      { dataType: 'type4', id: '4' },
      { dataType: 'type5', id: '5' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([
      { dataType: 'type4', id: '4' },
      { dataType: 'type5', id: '5' },
    ]);
  });

  it('should return empty array when all appData match exclusion criteria', () => {
    const dataTypes = [
      { id: 'type1', appLogic: true },
      { id: 'type2', allowedContributers: ['app:owned'] },
    ] as IDataType[];

    const appData = [
      { dataType: 'type1', id: '1' },
      { dataType: 'type2', id: '2' },
    ] as IData[];

    const result = filterAppData(appData, dataTypes);

    expect(result).toEqual([]);
  });
});
