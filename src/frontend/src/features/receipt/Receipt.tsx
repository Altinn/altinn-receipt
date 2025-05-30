import { createStyles, createTheme, Grid, WithStyles, withStyles } from '@material-ui/core';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import React from 'react';

import type { IAttachment, IParty } from 'src/types';

import { AltinnAppHeader, AltinnContentLoader, AltinnModal, AltinnReceipt, AltinnSubstatusPaper } from 'src/components';
import AltinnReceiptTheme from 'src/theme/altinnReceiptTheme';
import {
  filterAppData,
  getAttachmentGroupings,
  getInstancePdf,
  mapAppDataToAttachments,
} from 'src/utils/attachmentsUtils';
import { getAppName, getLanguageFromKey, getParsedLanguageFromKey, getTextResourceByKey } from 'src/utils/language';
import { getInstanceMetaDataObject } from 'src/utils/receipt';
import { returnUrlToMessagebox } from 'src/utils/urlHelper';

import { useFetchInitialData, useLanguageWithOverrides } from './hooks';

const theme = createTheme(AltinnReceiptTheme);

const styles = () =>
  createStyles({
    body: {
      '@media only print': {
        paddingLeft: '48px !important',
      },
    },
    substatus: {
      maxWidth: '875px',
      [theme.breakpoints.down('sm')]: {
        width: '95%',
      },
      [theme.breakpoints.up('md')]: {
        width: '80%',
      },
    },
  });

function Receipt(props: WithStyles<typeof styles>) {
  const [attachments, setAttachments] = React.useState<IAttachment[]>();
  const [pdf, setPdf] = React.useState<IAttachment[]>();

  const { application, textResources, party, instance, organisations, user } = useFetchInitialData();

  const { language } = useLanguageWithOverrides({
    textResources,
    instance,
    user,
  });

  const isPrint = useMediaQuery('print');

  const getTitle = (): React.ReactNode => {
    const applicationTitle = getAppName(textResources, application, user.profileSettingPreference.language);

    return (
      <>
        <span>{applicationTitle}</span>{' '}
        {instance.isA2Lookup ? '' : getParsedLanguageFromKey('receipt_platform.is_sent', language)}
      </>
    );
  };

  const handleModalClose = () => {
    window.location.href = returnUrlToMessagebox(window.location.origin);
  };

  const isLoading = !party || !instance || !organisations || !application || !language || !user || !textResources;

  React.useEffect(() => {
    if (instance && application) {
      const filteredAppData = filterAppData(instance.data, application.dataTypes);
      const attachments = mapAppDataToAttachments(filteredAppData, true);

      setAttachments(attachments);
      setPdf(getInstancePdf(instance.data, true));
    }
  }, [instance, application]);

  return (
    <Grid
      container={true}
      direction='column'
      justifyContent='center'
      alignItems='center'
    >
      <AltinnAppHeader
        logoColor={theme.altinnPalette.primary.blueDarker}
        headerBackgroundColor={theme.altinnPalette.primary.blue}
        party={party || ({} as IParty)}
        userParty={user ? user.party : ({} as IParty)}
        logoutText={getParsedLanguageFromKey('receipt_platform.log_out', language)}
        ariaLabelIcon={getLanguageFromKey('receipt_platform.profile_icon_aria_label', language)}
      />
      {instance?.status?.substatus && (
        <Grid
          item={true}
          className={props.classes.substatus}
          data-testid='receipt-substatus'
        >
          <AltinnSubstatusPaper
            label={getTextResourceByKey(instance.status.substatus.label, textResources)}
            description={getTextResourceByKey(instance.status.substatus.description, textResources)}
          />
        </Grid>
      )}
      <AltinnModal
        classes={{ body: props.classes.body }}
        isOpen={true}
        onClose={handleModalClose}
        hideBackdrop={true}
        hideCloseIcon={isPrint}
        printView={true}
        closeButtonOutsideModal={true}
        headerText={getParsedLanguageFromKey('receipt_platform.receipt', language)}
      >
        {isLoading ? (
          <AltinnContentLoader />
        ) : (
          <AltinnReceipt
            title={getTitle()}
            body={getParsedLanguageFromKey(
              instance.isA2Lookup ? 'receipt_platform.helper_text_a2lookup' : 'receipt_platform.helper_text',
              language,
            )}
            collapsibleTitle={getParsedLanguageFromKey('receipt_platform.attachments', language)}
            attachmentGroupings={getAttachmentGroupings(attachments, application, textResources)}
            instanceMetaDataObject={getInstanceMetaDataObject(
              instance,
              party,
              language,
              organisations,
              application,
              textResources,
              user.profileSettingPreference.language,
            )}
            titleSubmitted={
              instance.isA2Lookup ? '' : getParsedLanguageFromKey('receipt_platform.sent_content', language)
            }
            pdf={pdf || null}
          />
        )}
      </AltinnModal>
    </Grid>
  );
}

export default withStyles(styles)(Receipt);
