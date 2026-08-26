using System.Collections.Generic;
using System.Linq;
using Altinn.Platform.Receipt.Model;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Receipt.ViewModels;
using Altinn.Platform.Register.Models;
using Altinn.Platform.Storage.Interface.Models;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.ViewModels;

public class ReceiptViewModelFactoryTests
{
    [Fact]
    public void Create_ArchivedInstance_ShowsTheKeyInformation()
    {
        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext());

        Assert.Equal("Kvittering", model.Heading);
        Assert.Equal("Testetaten", model.Receiver);
        Assert.Equal("Testskjema er sendt inn", model.Title.Value);
        Assert.Equal(
            new[] { "Dato sendt", "Avsender", "Mottaker", "Referansenummer" },
            model.MetaData.Select(item => item.Label)
        );
        Assert.Equal("15.01.2024 / 10:30", model.MetaData[0].Value);
        Assert.Equal("12345678901-Ola Nordmann", model.MetaData[1].Value);
        Assert.Equal("Testetaten", model.MetaData[2].Value);
        Assert.Equal("4164e925812b", model.MetaData[3].Value);
    }

    [Fact]
    public void Create_OrganisationParty_ShowsOrganisationNumberAsSender()
    {
        ReceiptPageContext context = CreateContext(party: Parties.Party2);

        ReceiptViewModel model = ReceiptViewModelFactory.Create(context);

        Assert.Equal("910075918-Testbedrift AS", model.MetaData[1].Value);
    }

    [Fact]
    public void Create_A2LookupInstance_HidesSenderAndReceiver()
    {
        Instance instance = Instances.ArchivedInstance;
        instance.DataValues["A2ServiceType"] = "Lookup";

        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext(instance));

        Assert.Equal(new[] { "Dato arkivert", "Referansenummer" }, model.MetaData.Select(item => item.Label));
        Assert.Equal("Testskjema", model.Title.Value);
        Assert.Null(model.SubmittedHeading);
        Assert.Contains("Informasjonen som ble hentet ut", model.Body.Value);
    }

    [Fact]
    public void Create_Substatus_IsResolvedFromTheTextResources()
    {
        Instance instance = Instances.ArchivedInstance;
        instance.Status.Substatus = new Substatus { Label = "group.other", Description = "unknown.key" };

        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext(instance));

        Assert.Equal("Andre vedlegg", model.Substatus.Label);
        Assert.Equal("unknown.key", model.Substatus.Description);
    }

    [Fact]
    public void Create_NoSubstatus_LeavesItOut()
    {
        Assert.Null(ReceiptViewModelFactory.Create(CreateContext()).Substatus);
    }

    [Fact]
    public void Create_NoApplicationMetadataOrTexts_StillShowsTheReceipt()
    {
        ReceiptPageContext context = new()
        {
            Instance = Instances.ArchivedInstance,
            InstanceGuid = Instances.InstanceGuid,
            Party = Parties.Party1,
            User = UserProfiles.User1,
            Organisations = CreateOrganisations(),
            Language = "nb",
            Host = "platform.at22.altinn.cloud",
        };

        ReceiptViewModel model = ReceiptViewModelFactory.Create(context);

        Assert.Equal("er sendt inn", model.Title.Value);
        Assert.Empty(model.AttachmentGroups);
        Assert.Equal("Testdirektoratet", model.Receiver);
        Assert.Equal("15.01.2024 / 10:30", model.MetaData[0].Value);
    }

    [Fact]
    public void Create_UserIsInstanceOwner_HasNoOnBehalfOfName()
    {
        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext());

        Assert.Equal("Ola Nordmann", model.UserName);
        Assert.Null(model.OnBehalfOfName);
    }

    [Fact]
    public void Create_UserIsNotInstanceOwner_HasOnBehalfOfName()
    {
        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext(party: Parties.Party2));

        Assert.Equal("Testbedrift AS", model.OnBehalfOfName);
    }

    [Fact]
    public void Create_AppNameWithMarkup_IsEncoded()
    {
        TextResource textResource = new()
        {
            Resources = new List<TextResourceElement>
            {
                new TextResourceElement { Id = "appName", Value = "<script>alert('x')</script>" },
            },
        };

        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext(textResource: textResource));

        Assert.DoesNotContain("<script>", model.Title.Value);
    }

    [Fact]
    public void Create_DialogId_SendsTheUserBackToTheDialog()
    {
        ReceiptViewModel model = ReceiptViewModelFactory.Create(CreateContext(host: "platform.tt02.altinn.no"));

        Assert.Contains("inbox%2F0194bd21-0000-7000-a000-000000000000", model.ReturnUrl);
    }

    private static ReceiptPageContext CreateContext(
        Instance instance = null,
        Party party = null,
        Application application = null,
        TextResource textResource = null,
        string host = "platform.at22.altinn.cloud"
    )
    {
        return new ReceiptPageContext
        {
            Instance = instance ?? Instances.ArchivedInstance,
            InstanceGuid = Instances.InstanceGuid,
            Party = party ?? Parties.Party1,
            User = UserProfiles.User1,
            Application = application ?? Applications.Application1,
            TextResource = textResource ?? TextResources.Norwegian,
            Organisations = CreateOrganisations(),
            Language = "nb",
            Host = host,
            AttachmentGroupsToHide = Applications.HiddenGrouping,
        };
    }

    private static AltinnOrgs CreateOrganisations()
    {
        return new AltinnOrgs
        {
            Orgs = new Dictionary<string, AltinnOrg>
            {
                ["tdd"] = new AltinnOrg { Name = new Dictionary<string, string> { ["nb"] = "Testdirektoratet" } },
            },
        };
    }
}
