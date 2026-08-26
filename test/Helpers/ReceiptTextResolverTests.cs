using System.Collections.Generic;
using Altinn.Platform.Receipt.Helpers;
using Altinn.Platform.Receipt.Tests.Testdata;
using Altinn.Platform.Storage.Interface.Models;
using Xunit;

namespace Altinn.Platform.Receipt.Tests.Helpers;

public class ReceiptTextResolverTests
{
    [Fact]
    public void Resolve_NoTextResources_ReturnsTheDefaultTexts()
    {
        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve("nb", null, null);

        Assert.Equal("Kvittering", texts["receipt"]);
    }

    [Fact]
    public void Resolve_UnknownLanguage_FallsBackToBokmaal()
    {
        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve("de", null, null);

        Assert.Equal("Kvittering", texts["receipt"]);
    }

    [Fact]
    public void Resolve_EnglishLanguage_ReturnsEnglishTexts()
    {
        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve("en", null, null);

        Assert.Equal("Receipt", texts["receipt"]);
    }

    [Fact]
    public void Resolve_AppOverride_ReplacesTheDefaultText()
    {
        TextResource textResource = new()
        {
            Resources = new List<TextResourceElement>
            {
                new TextResourceElement { Id = "receipt_platform.receipt", Value = "Bekreftelse" },
                new TextResourceElement { Id = "appName", Value = "Testskjema" },
            },
        };

        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve("nb", textResource, null);

        Assert.Equal("Bekreftelse", texts["receipt"]);
        Assert.Equal("Avsender", texts["sender"]);
    }

    [Fact]
    public void Resolve_AppOverrideWithInstanceContextVariable_ReplacesTheVariable()
    {
        TextResource textResource = new()
        {
            Resources = new List<TextResourceElement>
            {
                new TextResourceElement
                {
                    Id = "receipt_platform.helper_text",
                    Value = "Referanse {0}",
                    Variables = new List<TextResourceVariable>
                    {
                        new TextResourceVariable { Key = "instanceId", DataSource = "instanceContext" },
                    },
                },
            },
        };

        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve(
            "nb",
            textResource,
            Instances.ArchivedInstance
        );

        Assert.Equal($"Referanse {Instances.ArchivedInstance.Id}", texts["helper_text"]);
    }

    [Fact]
    public void Resolve_AppOverrideWithUnknownVariable_KeepsTheVariableKey()
    {
        TextResource textResource = new()
        {
            Resources = new List<TextResourceElement>
            {
                new TextResourceElement
                {
                    Id = "receipt_platform.helper_text",
                    Value = "Verdi {0}",
                    Variables = new List<TextResourceVariable>
                    {
                        new TextResourceVariable { Key = "Model.Name", DataSource = "dataModel" },
                    },
                },
            },
        };

        IReadOnlyDictionary<string, string> texts = ReceiptTextResolver.Resolve("nb", textResource, null);

        Assert.Equal("Verdi Model.Name", texts["helper_text"]);
    }
}
