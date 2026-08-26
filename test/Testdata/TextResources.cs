using System.Collections.Generic;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Tests.Testdata
{
    public static class TextResources
    {
        public static TextResource Norwegian =>
            new TextResource
            {
                Id = "tdd-apps-test-nb",
                Org = "tdd",
                Language = "nb",
                Resources = new List<TextResourceElement>
                {
                    new TextResourceElement { Id = "appName", Value = "Testskjema" },
                    new TextResourceElement { Id = "appReceiver", Value = "Testetaten" },
                    new TextResourceElement { Id = "group.other", Value = "Andre vedlegg" },
                },
            };
    }
}
