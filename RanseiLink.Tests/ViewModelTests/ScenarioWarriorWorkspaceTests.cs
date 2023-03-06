using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using RanseiLink.ViewModels;
using System.Collections.Generic;

namespace RanseiLink.Tests.ViewModelTests;

public  class ScenarioWarriorWorkspaceTests
{
    public ScenarioWarriorWorkspaceTests()
    {

    }

    [Fact]
    public void DragDropIntegrationTest()
    {
        var scenario = ScenarioId.TheLegendOfRansei;
        var sws = new Mock<IScenarioWarriorService>();
        var sps = new Mock<IScenarioPokemonService>();
        var csws = new Mock<IChildScenarioWarriorService>();
        var csps = new Mock<IChildScenarioPokemonService>();
        var sks = new Mock<IScenarioKingdomService>();
        var js = new Mock<IJumpService>();
        var ins = new Mock<IIdToNameService>();
        var ps = new Mock<IPokemonService>();

        var warriors = new List<ScenarioWarrior>
        {
            new ScenarioWarrior()
            {
                Warrior = WarriorId.PlayerMale_1,
                Kingdom = KingdomId.Aurora,
                Army = 0,
                Class = WarriorClassId.ArmyLeader,
            }
        };

        var pokemon = new List<ScenarioPokemon>
        {
            new ScenarioPokemon()
            {
                
            }
        };

        var sk = new ScenarioKingdom();
        sk.SetArmy(0, 0);
        sk.SetArmy(1, 2);

        csws.Setup(x => x.Enumerate()).Returns(warriors);
        sws.Setup(x => x.Retrieve((int)scenario)).Returns(csws.Object);
        sps.Setup(x => x.Retrieve((int)scenario)).Returns(csps.Object);
        sks.Setup(x => x.Retrieve((int)scenario)).Returns(sk);
        ins.Setup(x => x.GetComboBoxItemsExceptDefault<IPokemonService>()).Returns(new List<SelectorComboBoxItem>());
        ins.Setup(x => x.GetComboBoxItemsPlusDefault<IAbilityService>()).Returns(new List<SelectorComboBoxItem>());
        csps.Setup(x => x.Retrieve(0)).Returns(pokemon[0]);

        var vm = new ScenarioWarriorWorkspaceViewModel(
            () => new SwMiniViewModel(
                sps.Object,
                new Mock<IBaseWarriorService>().Object,
                new Mock<ICachedSpriteProvider>().Object,
                ps.Object,
                new Mock<IKingdomService>().Object
                ),
            () => new SwKingdomMiniViewModel(
                sks.Object,
                new Mock<IBaseWarriorService>().Object,
                sws.Object,
                sps.Object,
                new Mock<ICachedSpriteProvider>().Object,
                ps.Object
                ),
            () => new SwSimpleKingdomMiniViewModel(
                new Mock<ICachedSpriteProvider>().Object
                ),
            ins.Object,
            js.Object
            );

        vm.Init(new ScenarioPokemonViewModel(
            js.Object,
            sws.Object,
            ins.Object,
            ps.Object
            ));

        vm.SetModel(
            ScenarioId.TheLegendOfRansei,
            csws.Object,
            csps.Object
            );

        // make sure all kingdom's are loaded into items
        vm.Items.Should().HaveCount(18 + 1, because: "There is 17 kingdoms, the default kingdom, and one warrior");
        vm.WildItems.Should().HaveCount(18, because: "There is 17 kingdoms, the default kingdom");
        vm.UnassignedItems.Should().HaveCount(0, because: "There is not unassigned warriors");


        // assert initial warrior state
        var warrior = vm.Items[1].Should().BeOfType<SwMiniViewModel>().Which;
        warrior.Army.Should().Be(0);
        warrior.Kingdom.Should().Be(0);
        warrior.Class.Should().Be(WarriorClassId.ArmyLeader);


        // simulate drag-drop from kingdom0 to kingdom1
        vm.Items.Move(1, 2);
        // warrior should have had its army and kingdom updated
        warrior.Army.Should().Be(2);
        warrior.Kingdom.Should().Be(1);
        warrior.Class.Should().Be(WarriorClassId.ArmyLeader);


        // simulate drag-drop from army-warrior to wild-warrior
        vm.Items.RemoveAt(2);
        vm.WildItems.Insert(1, warrior);
        // warrior should have had its army and kingdom updated
        // and its class should have been changed to one compatible with wild warriors
        warrior.Army.Should().Be(17, because: "the default army is 17");
        warrior.Kingdom.Should().Be(0);
        warrior.Class.Should().Be(WarriorClassId.FreeWarrior_1);


        // change the class to a different valid value, this way we can ensure it isn't changed back
        warrior.Class = WarriorClassId.FreeWarrior_2;
        // simulate drag-drop from kingdom0 to kingdom2
        vm.WildItems.Move(1, 3);
        // warrior should have been moved kingdom but nothing else changed
        warrior.Army.Should().Be(17, because: "the default army is 17");
        warrior.Kingdom.Should().Be(2);
        warrior.Class.Should().Be(WarriorClassId.FreeWarrior_2);


        // simulate drag-drop from wild-warrior to army-warrior
        vm.WildItems.RemoveAt(3);
        vm.Items.Insert(1, warrior);
        // warrior should have had its army and kingdom updated
        // and its class should have been changed to one compatible with armies
        warrior.Army.Should().Be(0);
        warrior.Kingdom.Should().Be(0);
        warrior.Class.Should().Be(WarriorClassId.ArmyMember);


        // simulate drag-drop from army-warrior to unassigned-warrior
        vm.Items.RemoveAt(1);
        vm.UnassignedItems.Insert(0, warrior);
        // warrior should have had its army updated
        // and its class should be changed to default
        warrior.Army.Should().Be(17, because: "the default army is 17");
        warrior.Kingdom.Should().Be(0);
        warrior.Class.Should().Be(WarriorClassId.Default);

    }

}
