using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.GuiCore.ViewModels;
using RanseiLink.Windows.Services;
using RanseiLink.Windows.ViewModels;
using System.Collections.Generic;

namespace RanseiLink.Windows.Tests.ViewModelTests;

public  class ScenarioWarriorWorkspaceTests
{
    Mock<IStrengthService> ss = new();
    Mock<IScenarioWarriorService> sws = new();
    Mock<IScenarioPokemonService> sps = new();
    Mock<IChildScenarioWarriorService> csws = new();
    Mock<IChildScenarioPokemonService> csps = new();
    Mock<IScenarioKingdomService> sks = new();
    Mock<IJumpService> js = new();
    Mock<IIdToNameService> ins = new();
    Mock<IPokemonService> ps = new();
    ScenarioId scenario = ScenarioId.TheLegendOfRansei;
    List<ScenarioWarrior> warriors = new();
    List<ScenarioPokemon> pokemon = new();
    ScenarioKingdom sk = new();
    ScenarioWarriorWorkspaceViewModel vm;

    public ScenarioWarriorWorkspaceTests()
    {

    }

    private void Setup()
    {
        csws.Setup(x => x.Enumerate()).Returns(warriors);
        sws.Setup(x => x.Retrieve((int)scenario)).Returns(csws.Object);
        sps.Setup(x => x.Retrieve((int)scenario)).Returns(csps.Object);
        sks.Setup(x => x.Retrieve((int)scenario)).Returns(sk);
        ins.Setup(x => x.GetComboBoxItemsExceptDefault<IPokemonService>()).Returns(new List<SelectorComboBoxItem>());
        ins.Setup(x => x.GetComboBoxItemsPlusDefault<IAbilityService>()).Returns(new List<SelectorComboBoxItem>());
        csps.Setup(x => x.Retrieve(0)).Returns(pokemon[0]);

        vm = new ScenarioWarriorWorkspaceViewModel(
            () => new SwMiniViewModel(
                sps.Object,
                new Mock<IBaseWarriorService>().Object,
                new Mock<ICachedSpriteProvider>().Object,
                new Mock<IKingdomService>().Object,
                ss.Object
                ),
            () => new SwKingdomMiniViewModel(
                sks.Object,
                new Mock<IBaseWarriorService>().Object,
                sws.Object,
                new Mock<ICachedSpriteProvider>().Object,
                ss.Object
                ),
            () => new SwSimpleKingdomMiniViewModel(
                new Mock<ICachedSpriteProvider>().Object
                ),
            ins.Object,
            js.Object
            );

        vm.Init(new ScenarioPokemonViewModel(
            js.Object,
            ins.Object,
            ps.Object
            ));

        vm.SetModel(
            scenario,
            csws.Object,
            csps.Object
            );
    }

    [Fact]
    public void VerifyInitialState()
    {
        warriors.Add(new ScenarioWarrior()
        {
            Warrior = WarriorId.PlayerMale_1,
            Kingdom = KingdomId.Aurora,
            Army = 0,
            Class = WarriorClassId.ArmyLeader,
        });

        pokemon.Add(new ScenarioPokemon()
        {

        });

        sk = new ScenarioKingdom();
        sk.SetArmy(0, 0);
        sk.SetArmy(1, 2);

        Setup();

        // make sure all kingdom's are loaded into items
        using (new AssertionScope())
        {
            vm.Items.Should().HaveCount(18 + 1, because: "There is 17 kingdoms, the default kingdom, and one warrior");
            vm.WildItems.Should().HaveCount(18, because: "There is 17 kingdoms, the default kingdom");
            vm.UnassignedItems.Should().HaveCount(0, because: "There is not unassigned warriors");
        }
        
        using (new AssertionScope())
        {
            kingdom = vm.Items[0].Should().BeOfType<SwKingdomMiniViewModel>().Which;
            warrior = vm.Items[1].Should().BeOfType<SwMiniViewModel>().Which;
            kingdom2 = vm.Items[2].Should().BeOfType<SwKingdomMiniViewModel>().Which;
        }
        // assert initial warrior and kngdom state
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(0);
            warrior.Kingdom.Should().Be(0);
            warrior.Class.Should().Be(WarriorClassId.ArmyLeader);
            
            kingdom.Army.Should().Be(0);
            kingdom.Kingdom.Should().Be(KingdomId.Aurora);

            kingdom2.Army.Should().Be(2);
            kingdom2.Kingdom.Should().Be(KingdomId.Ignis);
        }

    }

    SwMiniViewModel warrior;
    SwKingdomMiniViewModel kingdom;
    SwKingdomMiniViewModel kingdom2;

    [Fact]
    public void DragDropIntegrationTest()
    {
        VerifyInitialState();

        // simulate drag-drop from kingdom0 to kingdom1
        vm.Items.Move(1, 2);
        // warrior should have had its army and kingdom updated
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(2);
            warrior.Kingdom.Should().Be(1);
            warrior.Class.Should().Be(WarriorClassId.ArmyLeader);
        }
        
        // simulate drag-drop from army-warrior to wild-warrior
        vm.Items.RemoveAt(2);
        vm.WildItems.Insert(1, warrior);
        // warrior should have had its army and kingdom updated
        // and its class should have been changed to one compatible with wild warriors
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(17, because: "the default army is 17");
            warrior.Kingdom.Should().Be(0);
            warrior.Class.Should().Be(WarriorClassId.FreeWarrior_1);
        }
        
        // change the class to a different valid value, this way we can ensure it isn't changed back
        warrior.Class = WarriorClassId.FreeWarrior_2;
        // simulate drag-drop from kingdom0 to kingdom2
        vm.WildItems.Move(1, 3);
        // warrior should have been moved kingdom but nothing else changed
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(17, because: "the default army is 17");
            warrior.Kingdom.Should().Be(2);
            warrior.Class.Should().Be(WarriorClassId.FreeWarrior_2);
        }
        
        // simulate drag-drop from wild-warrior to army-warrior
        vm.WildItems.RemoveAt(3);
        vm.Items.Insert(1, warrior);
        // warrior should have had its army and kingdom updated
        // and its class should have been changed to one compatible with armies
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(0);
            warrior.Kingdom.Should().Be(0);
            warrior.Class.Should().Be(WarriorClassId.ArmyMember);
        }
        
        // simulate drag-drop from army-warrior to unassigned-warrior
        vm.Items.RemoveAt(1);
        vm.UnassignedItems.Insert(0, warrior);
        // warrior should have had its army updated
        // and its class should be changed to default
        using (new AssertionScope())
        {
            warrior.Army.Should().Be(17, because: "the default army is 17");
            warrior.Kingdom.Should().Be(0);
            warrior.Class.Should().Be(WarriorClassId.Default);
        }
    }

    [Fact]
    public void ChangeKingdomArmyUpdatesWarriors()
    {
        VerifyInitialState();

        kingdom.Army = 1;
        kingdom.Army.Should().Be(1);
        warrior.Army.Should().Be(1);
        kingdom.Army = 2;
        kingdom.Army.Should().Be(2);
        warrior.Army.Should().Be(2);
    }

}
