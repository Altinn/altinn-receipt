using System;
using System.Collections.Generic;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Tests.Testdata
{
    public static class Instances
    {
        public static readonly Guid InstanceGuid = Guid.Parse("1c3a4b9d-cbbe-4146-b370-4164e925812b");

        /// <summary>
        /// An archived instance with a form, a generated PDF and three uploaded attachments.
        /// </summary>
        public static Instance ArchivedInstance =>
            new Instance
            {
                Id = $"{Parties.Party1.PartyId}/{InstanceGuid}",
                InstanceOwner = new InstanceOwner { PartyId = Parties.Party1.PartyId.ToString() },
                AppId = Applications.Application1.Id,
                Org = Applications.Application1.Org,
                Created = DateTime.Parse("2024-01-15T08:00:00Z").ToUniversalTime(),
                LastChanged = DateTime.Parse("2024-01-15T09:30:00Z").ToUniversalTime(),
                Process = new ProcessState
                {
                    Started = DateTime.Parse("2024-01-15T08:00:00Z").ToUniversalTime(),
                    Ended = DateTime.Parse("2024-01-15T09:30:00Z").ToUniversalTime(),
                },
                Status = new InstanceStatus
                {
                    IsArchived = true,
                    Archived = DateTime.Parse("2024-01-15T09:30:00Z").ToUniversalTime(),
                },
                Data = new List<DataElement>
                {
                    CreateDataElement("default", null),
                    CreateDataElement("ref-data-as-pdf", "skjema.pdf"),
                    CreateDataElement("vedlegg", "vedlegg.pdf"),
                    CreateDataElement("grouped", "gruppert.pdf"),
                    CreateDataElement("hiddengroup", "skjult.pdf"),
                },
                DataValues = new Dictionary<string, string> { { "dialog.id", "0194bd21-0000-7000-a000-000000000000" } },
            };

        private static DataElement CreateDataElement(string dataType, string filename)
        {
            string id = Guid.NewGuid().ToString();

            return new DataElement
            {
                Id = id,
                InstanceGuid = InstanceGuid.ToString(),
                DataType = dataType,
                Filename = filename,
                LastChanged = DateTime.Parse("2024-01-15T09:29:00Z").ToUniversalTime(),
                SelfLinks = new ResourceLinks
                {
                    Platform =
                        $"https://platform.at22.altinn.cloud/storage/api/v1/instances/{Parties.Party1.PartyId}/{InstanceGuid}/data/{id}",
                },
            };
        }
    }
}
