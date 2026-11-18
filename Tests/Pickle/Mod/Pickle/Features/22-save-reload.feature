# S25, partial: after a save and reload the ship component and the roles component are back with their state (test hooks noKill and noAutoButton, game not ended). Running sabotage, ghosts and cooldowns are not asserted: no step reads them.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus save and load

  Scenario: the components survive a save and reload
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestSabotage"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 2000 ticks
    And I save and reload
    Then Nelim's Pickle Tools: the field "noKill" of the game component "GameComponent_Ship" is "True"
    And Nelim's Pickle Tools: the field "noAutoButton" of the game component "GameComponent_Ship" is "True"
    And Nelim's Pickle Tools: the field "ended" of the game component "GameComponent_Ship" is "False"
    And Nelim's Pickle Tools: the field "Active" of the game component "GameComponent_Roles" is "True"
    And no errors were logged
