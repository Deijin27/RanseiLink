using Core.Services;
using Core.Enums;
using Xunit;
using System;
using System.IO;

namespace Tests.ServiceTests
{
    public class ServiceTests
    {
        [Fact]
        void ServiceRetrieveAndSavePokemonTest()
        {
            var service = new DataService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Ransei/Tests/{nameof(ServiceRetrieveAndSavePokemonTest)}"));

            var raichu = service.Retrieve(PokemonId.Raichu);

            var startMove = raichu.Move;

            raichu.Move = MoveId.BodySlam;
            service.Save(PokemonId.Raichu, raichu);
            Assert.Equal(MoveId.BodySlam, service.Retrieve(PokemonId.Raichu).Move);

            raichu.Move = MoveId.ElectroBall;
            service.Save(PokemonId.Raichu, raichu);
            Assert.Equal(MoveId.ElectroBall, service.Retrieve(PokemonId.Raichu).Move);

            raichu.Move = startMove;
            service.Save(PokemonId.Raichu, raichu);
        }
    }
}
