using System.Collections.Generic;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Tests.Testdata
{
    public static class Applications
    {
        public const string HiddenGrouping = "group.formdatahtml";

        public static Application Application1 =>
            new Application
            {
                Id = "tdd/apps-test",
                Org = "tdd",
                Title = new Dictionary<string, string> { { "nb", "Testapp" }, { "en", "Test app" } },
                DataTypes = new List<DataType>
                {
                    new DataType { Id = "default", AppLogic = new ApplicationLogic() },
                    new DataType { Id = "ref-data-as-pdf" },
                    new DataType { Id = "vedlegg" },
                    new DataType { Id = "grouped", Grouping = "group.other" },
                    new DataType { Id = "hiddengroup", Grouping = HiddenGrouping },
                    new DataType
                    {
                        Id = "appowned",
                        AllowedContributors = new List<string> { "app:owned" },
                    },
                },
            };
    }
}
