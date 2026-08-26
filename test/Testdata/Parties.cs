using Altinn.Platform.Register.Models;

namespace Altinn.Platform.Receipt.Tests.Testdata
{
    public static class Parties
    {
        public static Party Party1 { get; set; } =
            new Party
            {
                PartyId = 50001,
                SSN = "12345678901",
                Name = "Ola Nordmann",
                PartyTypeName = Register.Enums.PartyType.Person,
            };

        public static Party Party2 { get; set; } =
            new Party
            {
                PartyId = 50002,
                OrgNumber = "910075918",
                Name = "Testbedrift AS",
                PartyTypeName = Register.Enums.PartyType.Organisation,
            };
    }
}
