# S20 on BS_BetweenSus_Test: the main pawn (colonist 1, an impostor) picks Reactor meltdown in the Sabotage menu right after landing, before the 15000 tick cooldown: refused with the message.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus early sabotage

  Scenario: a sabotage before the cooldown is refused
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And I select "Zed"
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Sabotage"
    And Nelim's Pickle Tools: I pick the option "Reactor" of the open float menu
    Then Nelim's Pickle Tools: the last message shown contains "No sabotage possible right now."
    And no errors were logged
